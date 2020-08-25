using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Service
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(
            IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(
            IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseEndpoints(
                options =>
                {
                    options.MapControllers();
                });
        }
    }
}