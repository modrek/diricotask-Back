using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using diricoAPIs.Domain.Models;
using diricoAPIs.Domain.Repositories;
using diricoAPIs.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace diricoAPIs
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
            string DBConnectionString = Configuration.GetConnectionString("diricoConnectionString");
            services.AddDbContext<diricoDBContext>(options => options.UseSqlServer(DBConnectionString));


            services.AddSingleton<IBlobConnectionFactory, AzureBlobConnectionFactory>();
            services.AddSingleton<IImageAnalyzer, AzureImageAnalyzer>();
            services.AddScoped<IBlobService, AzureBlobService>();            
           // services.AddScoped<ISocialNetwork, FaceBook>();
            services.AddScoped<IVideoConverter, AzureVideoConverter>();
            services.AddScoped<IImageConverter, AzureImageConverter>();
            services.AddScoped<IAssetRepository, AssetRepository>();

           
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "dirico Task", Version = "v0.01" });
            });
            //services.AddLogging();
            //// Add our repository type
            //services.AddSingleton<ITodoRepository, TodoRepository>();
            //// Inject an implementation of ISwaggerProvider with defaulted settings applied
            //services.AddSwaggerGen();

            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = new QueryStringApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "dirico Task V0.01");
            });
           

            app.UseMvc();
        }
    }
}
