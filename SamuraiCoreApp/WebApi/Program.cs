using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApi.Contexts;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //build our websever host server
            var host = CreateWebHostBuilder(args).Build();
            
            // migrate the database seeding data.  Best practice = in Main, using service scope
            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetService<SamuraiContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            };

            host.Run();
        }


        //Basic
        //https://weblog.west-wind.com/posts/2018/Dec/31/Dont-let-ASPNET-Core-Default-Console-Logging-Slow-your-App-down
        //Why level can not control
        //https://stackoverflow.com/questions/45781873/is-net-core-2-0-logging-broken
        //Logging registration/filtering changes: 
        //https://github.com/aspnet/Announcements/issues/255
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
