using System;
using System.Collections.Generic;
using System.Fabric.Query;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Shared.Abstract;
using Swashbuckle.AspNetCore.Swagger;

namespace Frontend
{
    public class Startup
    {
        private Logger _myLog = new Logger("Frontend");

        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                _myLog.Info("Adding service");
                services.AddScoped<ILogger>(t => _myLog);
                services.AddMvc();

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "Frontend", Version = "v1" });
                });
            }
            catch(Exception e)
            {
                _myLog.Error("Error in adding service: ", e);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try
            {
                _myLog.Info("Configure called ");
                app.UseMiddleware(typeof(ErrorHandlingMiddleware));

                app.UseSwagger();

                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

                if (env.IsDevelopment())
                {
                    app.UseBrowserLink();
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                }

                app.UseStaticFiles();

                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });

            }
            catch(Exception e)
            {
                _myLog.Error("Error in Configure: ", e);
            }
        }
    }
}
