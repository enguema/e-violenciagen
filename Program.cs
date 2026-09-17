using e_violenciagen.Aplication.Casos;
using e_violenciagen.Aplication.Personas;
using e_violenciagen.Data;
using e_violenciagen.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Db 1.

string ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se ha configurado la cadena seleccionada");

//Db 2. egistramos el contexto en el contenedor de inyeccion de dependencias de ASP.NET Core
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(ConnectionString);
});

//----------Registramos los servicios--------------

builder.Services.AddScoped<ICasoService, CasoService>();
builder.Services.AddScoped<IPersonaService, PersonaService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())//Aquí cargamos datos de prueba
{
    using IServiceScope scope = app.Services.CreateScope();

    AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await CatalogoSeeder.SeedAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); //En producción evitamos mostrar información interna de las excepciones al usuario final.
    app.UseHsts();// HSTS obliga al navegador a utilizar HTTPS.
}

app.UseHttpsRedirection();
app.UseStaticFiles();//Permite servir CSS, JavaScript, imágenes y librerías almacenadas dentro de wwwroot.
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(//Ruta MVC Predeterminada
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
