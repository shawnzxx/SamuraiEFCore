using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
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
                .AddMvc(setupAction =>
                {
                    setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                    setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                    setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));

                    //by default if asp.net core api not received support media type, application/json will be returned.
                    //good api should block for this and return 406 status code
                    setupAction.ReturnHttpNotAcceptable = true;

                    //set up Accept: application/xml media type
                    setupAction.OutputFormatters.Add(new XmlSerializerOutputFormatter());

                    //set up Accept: application/json media type
                    var jsonOutputFormatter = setupAction.OutputFormatters
                        .OfType<JsonOutputFormatter>().FirstOrDefault();

                    if (jsonOutputFormatter != null)
                    {
                        // remove text/json as it isn't the approved media type
                        // for working with JSON at API level
                        if (jsonOutputFormatter.SupportedMediaTypes.Contains("text/json"))
                        {
                            jsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
                        }
                    }
                })
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

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var actionExecutingContext =
                        actionContext as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                    // if there are modelstate errors & all keys were correctly
                    // found/parsed we're dealing with validation errors
                    if (actionContext.ModelState.ErrorCount > 0
                        && actionExecutingContext?.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
                    {
                        return new UnprocessableEntityObjectResult(actionContext.ModelState);
                    }

                    // if one of the keys wasn't correctly found / couldn't be parsed
                    // we're dealing with null/unparsable input
                    return new BadRequestObjectResult(actionContext.ModelState);
                };
            });

            services.AddScoped<ISamuraiRepository, SamuraiRepository>();
            services.AddScoped<IQuoteRepository, QuoteRepository>();

            services.AddHttpClient();

            services.AddAutoMapper();

            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc("SamuraiOpenAPISpecification",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "Samurai API",
                        Version = "1.0",
                        Description = "Through this API you can access samurais and their quotes",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                        {
                            Email = "shawn.zhang@avanade.com",
                            Name = "Shawn Zhang",
                            Url = new Uri("https://www.linkedin.com/in/shawnzxx/")
                        },
                        License = new Microsoft.OpenApi.Models.OpenApiLicense()
                        {
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
            Console.WriteLine(env.EnvironmentName);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                //we add at back of UseHttpsRedirection, so that all link to swagger website using http will redirect to https site
                app.UseSwagger();
                app.UseSwaggerUI(setupAction =>
                {
                    setupAction.SwaggerEndpoint(
                        "/swagger/SamuraiOpenAPISpecification/swagger.json",
                        "Samurai API");
                    setupAction.RoutePrefix = "";
                });
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseMvc();
        }
    }
}
