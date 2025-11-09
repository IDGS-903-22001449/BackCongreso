using app_congreso.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Configurar servicios
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2️⃣ Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder
            .WithOrigins(
                "https://congresoexamen.netlify.app", // dominio de tu frontend
                "http://localhost:3000" // dominio local para pruebas
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// 3️⃣ Construir la app
var app = builder.Build();

// 4️⃣ Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ⚡ Importante: CORS antes de Authorization
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
