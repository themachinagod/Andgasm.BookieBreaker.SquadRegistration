using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Andgasm.BB.SquadRegistration.API
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
            services.AddLogging(loggingBuilder => loggingBuilder
                                .AddConsole()
                                .SetMinimumLevel(LogLevel.Debug));
            services.AddDbContext<SquadRegistrationDb>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors();
            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseHsts();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseHttpsRedirection();
            app.UseMvc();
            InitialiseData(app.ApplicationServices);
        }

        public static async void InitialiseData(IServiceProvider svcs)
        {
            // but of a hack here but this just ensure that the db exists and that the basic data is configured
            // helps support basic ui tests as well as expected config for the admin web help instructions

            using (var servicescope = svcs.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = servicescope.ServiceProvider.GetService<SquadRegistrationDb>();
                await context.Database.EnsureCreatedAsync();
            }
        }
    }
}
