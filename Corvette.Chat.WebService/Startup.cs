using System;
using System.Threading.Tasks;
using Corvette.Chat.Logic.IoC;
using Corvette.Chat.WebService.Configuration;
using Corvette.Chat.WebService.Helpers;
using Corvette.Chat.WebService.HostedServices;
using Corvette.Chat.WebService.Middleware;
using Corvette.Chat.WebService.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Corvette.Chat.WebService
{
    public class Startup
    {
        private readonly IConfiguration _rawConfig;
        private WebServiceConfiguration? _typedConfig;
        private WebServiceConfiguration Configuration => _typedConfig ??= _rawConfig.Get<WebServiceConfiguration>();

        public Startup(IConfiguration rawConfig)
        {
            _rawConfig = rawConfig ?? throw new ArgumentNullException(nameof(rawConfig));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // settings
            services.AddSingleton(Configuration);
            
            // common
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ModelFilter));
            });
            
            // chat services
            services.AddCorvetteChat(Configuration.DbOptions);
            services.AddSignalR();
            services.AddScoped<ChatHub>();
            services.AddSingleton<AuthHelper>();
            
            services.AddRazorPages()
                .AddRazorRuntimeCompilation();
            
            // add hosted services
            services.AddHostedService<DbMigrator>();
            
            // cors
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(Configuration.AllowedUrls.ToArray())
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));
            
            // authorization
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Configuration.AuthOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = Configuration.AuthOptions.Audience,

                    IssuerSigningKey = Configuration.AuthOptions.SymmetricSecurityKey,
                    ValidateIssuerSigningKey = true,

                    ValidateLifetime = true,
                };

                // for signalR
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("chat/hub"))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }
        
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMiddleware<ExceptionFilterMiddleware>();
            
            app.UseCors("CorsPolicy");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
                routes.MapHub<ChatHub>("chat/hub");
            });
        }
    }
}