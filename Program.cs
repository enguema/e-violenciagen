using e_violenciagen.Aplication.Casos;
using e_violenciagen.Aplication.Personas;
using e_violenciagen.Data;
using Microsoft.AspNetCore.Identity;
using e_violenciagen.Models;
using Microsoft.EntityFrameworkCore;
using e_violenciagen.Seguridad;
using Microsoft.AspNetCore.Authorization;
using e_violenciagen.Aplication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Db 1.

string ConnectionString = builder.Configuration.GetConnectionString("HomeConnection")
    ?? throw new InvalidOperationException("No se ha configurado la cadena seleccionada");

//Db 2. egistramos el contexto en el contenedor de inyeccion de dependencias de ASP.NET Core
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(ConnectionString);
});

//----------Registramos los servicios--------------

builder.Services.AddScoped<ICasoService, CasoService>();
builder.Services.AddScoped<IPersonaService, PersonaService>();
builder.Services.AddScoped<IAuthorizationHandler, PermisoAuthorizationHandler>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// =========================================================
// IDENTITY
// =========================================================
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // =====================================================
    // CONTRASEÑAS
    // =====================================================

    options.Password.RequiredLength = 8;

    options.Password.RequireDigit = true;

    options.Password.RequireLowercase = true;

    options.Password.RequireUppercase = true;

    options.Password.RequireNonAlphanumeric = false;


    // =====================================================
    // BLOQUEO
    // =====================================================

    /*
     * Permitimos que las cuentas sean bloqueables
     * por intentos fallidos.
     */
    options.Lockout.AllowedForNewUsers = true;


    /*
     * Cinco intentos consecutivos incorrectos.
     */
    options.Lockout.MaxFailedAccessAttempts = 5;


    /*
     * Bloqueo temporal.
     */
    options.Lockout.DefaultLockoutTimeSpan =
        TimeSpan.FromMinutes(15);


    // =====================================================
    // USUARIO
    // =====================================================

    options.User.RequireUniqueEmail = false;
}).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// =========================================================
// POLICIES BASADAS EN PERMISOS
// =========================================================
builder.Services.AddAuthorization(options =>
{
    /*
     * Creamos automáticamente una Policy
     * por cada permiso definido en PermisosSistema.
     *
     * Así evitamos registrar manualmente:
     *
     * CASOS_VER
     * CASOS_CREAR
     * CASOS_EDITAR
     * etc.
     */
    foreach (var permiso in PermisosSistema.Todos)
    {
        options.AddPolicy(
            permiso.Codigo,
            policy =>
            {
                /*
                 * El usuario debe estar autenticado
                 * antes de evaluar su permiso.
                 */
                policy.RequireAuthenticatedUser();


                /*
                 * Añadimos nuestro requisito personalizado.
                 */
                policy.AddRequirements(
                    new PermisoRequirement(
                        permiso.Codigo));
            });
    }
});

builder.Services.Configure<AdministradorInicialOptions>(
    builder.Configuration.GetSection(
        AdministradorInicialOptions.SectionName));

builder.Services.ConfigureApplicationCookie(options =>
{
    /*
     * Cuando un usuario no autenticado intente acceder
     * a una acción protegida, Identity lo enviará aquí.
     */
    options.LoginPath = "/Cuenta/Login";


    /*
     * Si está autenticado pero no tiene autorización,
     * usaremos posteriormente esta ruta.
     */
    options.AccessDeniedPath = "/Cuenta/AccesoDenegado";


    /*
     * La cookie no será accesible desde JavaScript.
     */
    options.Cookie.HttpOnly = true;


    /*
     * Duración de la sesión persistente
     * cuando el usuario marca "Recordarme".
     */
    options.ExpireTimeSpan =
        TimeSpan.FromHours(8);


    /*
     * Renueva la expiración mientras el usuario
     * continúa utilizando activamente el sistema.
     */
    options.SlidingExpiration = true;
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(//Ruta MVC Predeterminada
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


// =========================================================
// SEED DE IDENTITY
// =========================================================

/*
 * Identity se ejecuta después porque
 * ApplicationUser necesita una Institucion existente.
 */

await IdentitySeeder.SeedAsync(app.Services);


app.Run();
