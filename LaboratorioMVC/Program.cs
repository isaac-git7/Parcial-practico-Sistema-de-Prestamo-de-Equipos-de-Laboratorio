using LaboratorioMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios de MVC
builder.Services.AddControllersWithViews();

// Obtener la URL base de la API
string apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7123/";

// Registro de los servicios HTTP tipados
builder.Services.AddHttpClient<ICategoriaService, CategoriaService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IEquipoService, EquipoService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IReporteService, ReporteService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();