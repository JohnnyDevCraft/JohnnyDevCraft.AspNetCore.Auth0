using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JohnnyDevCraft.AspNetCore.Auth0.Abstractions;
using JohnnyDevCraft.AspNetCore.Auth0.ConfigObjects;
using JohnnyDevCraft.AspNetCore.Auth0.Harness.Services;
using Swashbuckle.AspNetCore.Filters;

namespace JohnnyDevCraft.AspNetCore.Auth0.Harness
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AuthConfig = Configuration.GetSection("Auth0").Get<Auth0Config>();
        }

        public Auth0Config AuthConfig { get; set; }

        public IConfiguration Configuration { get; }

        private CurrentUserService currentUserService;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAuthenticationWithAuth0(AuthConfig);

            services.AddScoped<CurrentUserService>();

            services.AddScoped<IRoleValidator, CurrentUserService>();
            services.AddScoped<IPermissionValidator, CurrentUserService>();

            var provider = services.BuildServiceProvider();
            currentUserService = (CurrentUserService) provider.GetService(typeof(CurrentUserService));

            services.AddPermissionBasedAuthorizationWithPermissions(currentUserService.GetPermissions());

            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Testing Harness", Version = "v1", Description = "OovhFfGz44xuafgVLG6uTjWFkacq6syD" });

                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl =
                                    new Uri(
                                        $"{AuthConfig.Authority}authorize?audience={AuthConfig.Audience}"),
                                Scopes = new Dictionary<string, string>()
                                {
                                    {"openid profile email", "Get all required info from Auth0" }
                                }
                            }
                        }
                    });

                    c.OperationFilter<SecurityRequirementsOperationFilter>();
                })
                .AddSwaggerGenNewtonsoftSupport();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JohnnyDevCraft.AspNetCore.Auth0.Harness v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseCustomEmailClaim("http://test.johnnydevcraft.com/email");

            app.UsePermissions(currentUserService.GetPermissions());

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }

}
