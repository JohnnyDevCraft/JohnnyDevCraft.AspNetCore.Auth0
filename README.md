# Introduction 
This library is designed to allow the developer to easily configure Auth0 for advanced authorization scinarios in 
.Net 5 WebAPIs.  I have explained how to use the library in the video listed below. 

# Getting Started

1. Install Nuget Package
2. Bin Config Object to Auth0 Config from IConfiguration
3. use services.AddAuthenticationWithAuth0(AuthConfig); in ConfigureServices to add Auth0 to your application.
4. use services.AddPermissionBasedAuthorizationWithPermissions(List of Permissions); in ConfigureServices to add Permission based authorization or services.AddRoleBasedAuthorizationWithRoles(List of Role Policies); in ConfigureServices to add Role based authorization.
5. Add IRoleValidator or IPermissionsValidator to services collection to enable Roles or Permission based on use case.
6. In your Application Pipeline add UseAthentication.
7. In your Application Pipeline add UsePermissions or UseRoles based on your needs.
8. In your Application Pipeline add UseAuthorization to enable the use of policies in the Authorize attribute.

```csharp
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

            services.AddAuthenticationWithAuth0(AuthConfig); //Step 2

            services.AddScoped<CurrentUserService>();

            // services.AddScoped<IRoleValidator, CurrentUserService>(); //Step 4
            services.AddScoped<IPermissionValidator, CurrentUserService>(); //Step 4

            var provider = services.BuildServiceProvider();
            currentUserService = (CurrentUserService) provider.GetService(typeof(CurrentUserService));

            services.AddPermissionBasedAuthorizationWithPermissions(currentUserService.GetPermissions()); //Step 3
            //services.AddRoleBasedAuthorizationWithRoles(currentUserService.GetRolePolicies()); //Step 3

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

            app.UseAuthentication(); //Step 5

            app.UseCustomEmailClaim("http://test.johnnydevcraft.com/email");

            app.UsePermissions(currentUserService.GetPermissions()); //Step 6

            app.UseAuthorization(); //Step 7

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }

}

```

# Build and Test
You can build and test this project using dotnet build & dotnet run. 

# Contribute
If you would like to contribute changes to this repository, pleae contact me at [John@JohnnyDevCraft.com](email://john@johnnydevcraft.com).
