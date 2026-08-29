using e_violenciagen.Data;
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

var app = builder.Build();

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
