using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;
using WebApi.Contexts;
using WebApi.Services;

namespace WebApi
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
            services
                .AddMvc()
                .AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // register the DbContext on the container, getting the connection string from
            // appSettings (note: use this during development; in a production environment,
            // it's better to store the connection string in an environment variable)
            // Everytime I use SamuraiContext the commands that it execute on the database will be output to the console windows
            var connection = Configuration.GetConnectionString("SamuraiConnection");
            services.AddDbContext<SamuraiContext>(optionsBuilder =>
            {
                optionsBuilder
                .EnableSensitiveDataLogging(true)
                .UseSqlServer(connection); // use "SqlServer" provider with connection string
            });

            services.AddScoped<ISamuraiRepository, SamuraiRepository>();
            services.AddScoped<IQuoteRepository, QuoteRepository>();

            services.AddAutoMapper();

            services.AddHttpClient();

            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc("SamuraiOpenAPISpecification",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "Samurai API",
                        Version = "1.0",
                        Description = "Through this API you can access samurais and their quotes",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact() {
                            Email = "shawn.zhang@avanade.com",
                            Name = "Shawn Zhang",
                            Url = new Uri("https://www.linkedin.com/in/shawnzxx/")
                        },
                        License = new Microsoft.OpenApi.Models.OpenApiLicense() {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/MIT")
                        }
                    });

                var xmlCommentsFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFileFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFileName);

                setupAction.IncludeXmlComments(xmlCommentsFileFullPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            //we add at back of UseHttpsRedirection, so that all link to swagger website using http will redirect to https site
            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint(
                    "/swagger/SamuraiOpenAPISpecification/swagger.json", 
                    "Samurai API");
                setupAction.RoutePrefix = "";
            });
            
            app.UseMvc();
        }
    }
}
