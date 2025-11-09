using Microsoft.EntityFrameworkCore;
using app_congreso.Data;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// ------------------------
// CORS: permitir tu frontend
// ------------------------
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://congresoexamen.netlify.app")
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

var app = builder.Build();

// ------------------------
// Middleware
// ------------------------
app.UseCors(MyAllowSpecificOrigins);

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
