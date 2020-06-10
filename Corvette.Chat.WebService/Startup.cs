using System.Threading.Tasks;
using Corvette.Chat.Data;
using Corvette.Chat.Logic.IoC;
using Corvette.Chat.WebService.HostedServices;
using Corvette.Chat.WebService.Settings;
using Corvette.Chat.WebService.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;

namespace Corvette.Chat.WebService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // settings
            var settings = Configuration.Get<AppSettings>();
            services.AddSingleton(settings);
            var connection = Configuration.GetConnectionString("DefaultConnection");
            
            // common
            services.AddMvc();
            services.Configure<AppSettings>(Configuration);
            
            // logging
            services.AddLogging(opt =>
                {
                    opt.ClearProviders();
                    opt.SetMinimumLevel(LogLevel.Debug);
                    opt.AddNLog();
                })
                .AddSingleton(provider => NLog.LogManager.GetLogger("application"));
            
            // chat services
            services.AddChatServices(connection);
            services.AddSignalR();
            services.AddSingleton<ChatHub>();
            
            // add hosted services
            services.AddHostedService<DbMigrator>();
            
            // cors
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(settings.AllowedUrls)
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
                    ValidIssuer = settings.AuthSettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = settings.AuthSettings.Audience,

                    IssuerSigningKey = settings.AuthSettings.SymmetricSecurityKey,
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
                        if (!string.IsNullOrEmpty(accessToken) 
                            && (path.StartsWithSegments("/signalr")))
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
            
            app.UseCors("CorsPolicy");

            app.UseRouting();
            
            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
                routes.MapHub<ChatHub>("/chat/hub");
            });
        }
    }
}