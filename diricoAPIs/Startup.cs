using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using diricoAPIs.Domain.Models;
using diricoAPIs.Domain.Repositories;
using diricoAPIs.Extensions;
using diricoAPIs.Logger;
using diricoAPIs.Services;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using Swashbuckle.AspNetCore.Swagger;

namespace diricoAPIs
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string DBConnectionString = Configuration.GetConnectionString("diricoConnectionString");
            services.AddDbContext<diricoDBContext>(options => options.UseSqlServer(DBConnectionString));

            // Inject Azure Service --Singleton
            services.AddSingleton<IBlobConnectionFactory, AzureBlobConnectionFactory>();
            services.AddSingleton<IImageAnalyzer, AzureImageAnalyzer>();

            // Inject Loger service
            services.AddSingleton<ILoggerManager, LoggerManager>();

            // Inject Image and Video Services and Repository 
            services.AddScoped<IBlobService, AzureBlobService>();
            services.AddScoped<IVideoConverter, AzureVideoConverter>();
            services.AddScoped<IImageConverter, AzureImageConverter>();
            services.AddScoped<IAssetRepository, AssetRepository>();

            //VariantAsset Config
            FaceBook faceBook = new FaceBook(null, null);
            Twitter twitter = new Twitter(null, null);
            VariantAssetConfig.Instance
                        .AddSocialNetwork(faceBook)
                        .AddSocialNetwork(twitter);

                        

            // Allow All region in CORS
            services.AddCors(o => o.AddPolicy("AllowOrigin", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));


            //services.ConfigureLoggerService();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add Swager Service
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "dirico Task", Version = "v1" });
            });
           
            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = new QueryStringApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // For Global Error Handling in ASP.NET Core Web API
            //https://code-maze.com/global-error-handling-aspnetcore/
            app.ConfigureExceptionHandler(logger);

            // API Development Documnets
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "dirico Task V1");
            });

            app.UseCors();
            app.UseMvc();
        }
    }
}
