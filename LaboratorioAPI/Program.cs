using Microsoft.EntityFrameworkCore;
using LaboratorioAPI.Data;
using LaboratorioAPI.Mappings;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro del DbContext conectado a SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Controladores y Swagger (OpenAPI)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar AutoMapper y su perfil de mapeo
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

var app = builder.Build();

// Configuración de la canalización de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// En la Fase 2 la Persona B agregará aquí app.UseAuthentication()
app.UseAuthorization();

app.MapControllers();


app.Run();