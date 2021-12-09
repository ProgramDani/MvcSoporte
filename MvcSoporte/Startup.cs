using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcSoporte.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcSoporte
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            // Deshabilitar confirmación de usuario. Configurar Identity para utilizar roles
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            // Registro del contexto de la base de datos
            services.AddDbContext<MvcSoporteContexto>(options =>
            options.UseSqlServer(
            Configuration.GetConnectionString("DefaultConnection")));

            // Configurar las opciones de los servicios de ASP.NET Core identity
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings. Configuración de las características de las contraseñas
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                //options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireNonAlphanumeric = false;
                //options.Password.RequireUppercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services) // Se añade el parámetro services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            // Llamada al método que crea los roles y usuarios predeterminados
            crearRolesyUsuariosPredeterminados(services).Wait();
        }
        private async Task crearRolesyUsuariosPredeterminados(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            IdentityResult roleResult, userResult;
            // Si no existe, se crea el rol “Administrador” y el administrador predeterminado
            var roleCheck = await RoleManager.RoleExistsAsync("Administrador");
            if (!roleCheck)
            {
                // Se crea el rol de Administrador
                var role = new IdentityRole();
                role.Name = "Administrador";
                roleResult = await RoleManager.CreateAsync(role);
                // Se crea el usuario administrador predeterminado. Se debe recomendar que se
                // cambie la contraseña al entrar en la aplicación web por primera vez
                var user = new IdentityUser();
                user.UserName = "admin@empresa.com";
                user.Email = "admin@empresa.com";
                string userPWD = "admin123";
                userResult = await UserManager.CreateAsync(user, userPWD);
                // Se Agrega el administrador predeterminado al rol de Administrador
                if (userResult.Succeeded)
                {
                    userResult = await UserManager.AddToRoleAsync(user, "Administrador");
                }
            }
            // Si no existe, se crea el rol “Usuario”
            roleCheck = await RoleManager.RoleExistsAsync("Usuario");
            if (!roleCheck)
            {
                var role = new IdentityRole();
                role.Name = "Usuario";
                roleResult = await RoleManager.CreateAsync(role);
            }
        }
    }
}
