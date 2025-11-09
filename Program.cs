using app_congreso.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// ================================
// 1️⃣ Configurar la cadena de conexión
// ================================
string connectionString;

// Obtener la variable de entorno DATABASE_URL de Render
var databaseUrl = Environment.GetEnvironmentVariable("postgresql://congresodb_qb0w_user:Zx9qjumL8Jv53lbBXDddrUzvCQ3Mlsv7@dpg-d4817pndiees739lr940-a.oregon-postgres.render.com/congresodb_qb0w\r\n");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Convertir DATABASE_URL (postgres://usuario:pass@host:port/db) a Npgsql
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
        TrustServerCertificate = true,
        Pooling = true
    };

    connectionString = npgsqlBuilder.ToString();
}
else
{
    // Fallback para desarrollo local usando appsettings.json
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

// Registrar DbContext con Npgsql
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ================================
// 2️⃣ Configurar servicios
// ================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ================================
// 3️⃣ Configurar CORS
// ================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://congresoexamen.netlify.app", // frontend desplegado
                "http://localhost:3000"               // frontend local
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ================================
// 4️⃣ Construir la app
// ================================
var app = builder.Build();

// ================================
// 5️⃣ Pipeline HTTP
// ================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ⚡ Importante: CORS antes de Authentication/Authorization
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
