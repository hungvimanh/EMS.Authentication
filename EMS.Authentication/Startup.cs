using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.Authentication.Repositories.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TF.Authentication.Commons;

namespace EMS.Authentication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string AllowSpecificOrigins = "_AllowSpecificOrigins";
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                  options.SerializerSettings.ContractResolver =
                     new DefaultContractResolver());
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string[] corsList = Configuration.GetSection("CORSes").Value.Split(";").Where(c => !string.IsNullOrEmpty(c)).ToArray();
            services.AddCors(options =>
            {
                options.AddPolicy(AllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins(corsList.ToArray()).WithMethods("POST").AllowAnyHeader();
                });
            });
            services.AddDbContext<EMSContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("EMSContext")));

            services.Scan(scan => scan
               .FromAssemblyOf<IDateTimeService>()
                   .AddClasses(classes => classes.AssignableTo<IDateTimeService>())
                       .AsImplementedInterfaces()
                       .WithScopedLifetime());

            services.Scan(scan => scan
               .FromAssemblyOf<IServiceScoped>()
                   .AddClasses(classes => classes.AssignableTo<IServiceScoped>())
                       .AsImplementedInterfaces()
                       .WithScopedLifetime());
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
