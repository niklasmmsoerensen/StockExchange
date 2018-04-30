using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared;
using Swashbuckle.AspNetCore.Swagger;
using ILogger = Shared.Abstract.ILogger;

namespace HTTPGateway
{
    public class Startup
    {
        Logger myLog = new Logger("HTTPGateway");
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
                services.AddScoped<ILogger>(t => myLog);
                myLog.Info("ConfigureServices called ");
                services.AddMvc();

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "HTTPGateway", Version = "v1" });
                });
            }
            catch(Exception e)
            {
                myLog.Error("Error in configureServices: ", e);
            }
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try
            {
                myLog.Info("Configure called ");
                app.UseMiddleware(typeof(ErrorHandlingMiddleware));

                app.UseSwagger();

                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseMvc();
            }
            catch (Exception e)
            {
                myLog.Error("Error in Configure: ", e);
            }
        }
    }
}
