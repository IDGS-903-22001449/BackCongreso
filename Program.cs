using Microsoft.EntityFrameworkCore;
using app_congreso.Data;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// ------------------------
// Configurar CORS para tu frontend
// ------------------------
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://congresoexamen.netlify.app") // tu frontend
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// ------------------------
// Parsear DATABASE_URL de Render
// ------------------------
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(databaseUrl))
{
    throw new Exception("No se encontró la variable de entorno DATABASE_URL");
}

var uri = new Uri(databaseUrl);
var userInfo = uri.UserInfo.Split(':');

var npgsqlBuilder = new NpgsqlConnectionStringBuilder
{
    Host = uri.Host,
    Port = uri.Port,
    Username = userInfo[0],
    Password = userInfo[1],
    Database = uri.AbsolutePath.TrimStart('/'),
    SslMode = SslMode.Require,
    TrustServerCertificate = true
};

string connectionString = npgsqlBuilder.ToString();

// ------------------------
// Configurar DbContext
// ------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Agregar controladores
builder.Services.AddControllers();

// ------------------------
// Swagger solo en desarrollo
// ------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ------------------------
// Aplicar migraciones automáticamente al iniciar
// ------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // crea las tablas automáticamente si no existen
}

// ------------------------
// Middleware
// ------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Importante: CORS antes de Authorization y MapControllers
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
