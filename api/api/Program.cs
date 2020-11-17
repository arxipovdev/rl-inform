using System.Threading.Tasks;
using api.Data;
using api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync();

                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleExist = await roleManager.RoleExistsAsync("Admin");
                if (!roleExist) await roleManager.CreateAsync(new IdentityRole("Admin"));
                
                roleExist = await roleManager.RoleExistsAsync("Editor");
                if (!roleExist) await roleManager.CreateAsync(new IdentityRole("Editor"));
                
                roleExist = await roleManager.RoleExistsAsync("Customer");
                if (!roleExist) await roleManager.CreateAsync(new IdentityRole("Customer"));
                
                var user = new User
                {
                    Login = "andrey",
                    UserName = "andrey",
                    Email = "andrey@mail.ru",
                    Name = "andrey"
                };
                var userExist = await userManager.FindByEmailAsync(user.Email);
                if (userExist is null)
                {
                    await userManager.CreateAsync(user, "Andrey12345!");
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}