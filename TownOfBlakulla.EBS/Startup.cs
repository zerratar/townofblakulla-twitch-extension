using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TownOfBlakulla.Core;
using TownOfBlakulla.Core.Db;
using TownOfBlakulla.Core.Handlers;
using TwitchLib.Extension;
using TwitchLib.Extension.Core.Authentication;
using TwitchLib.Extension.Core.ExtensionsManager;

namespace TownOfBlakulla.EBS
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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder => builder.AllowAnyOrigin());
                options.AddPolicy("AllowAllMethods", builder => builder.AllowAnyMethod());
                options.AddPolicy("AllowAllHeaders", builder => builder.AllowAnyHeader());
            });


            services.AddSingleton<IDbConnectionFactory, MSSQLDbConnectionFactory>();
            services.AddSingleton<IDbConnectionSettings, MSSQLDbConnectionSettings>();
            //services.AddSingleton<IPropertyRepository, JsonFileBasedPropertyRepository>();
            services.AddSingleton<IPropertyRepository, MSSQLBasedPropertyRepository>();
            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddSingleton<ITwitchAuth, TwitchAuth>();
            services.AddSingleton<IActionQueue, ActionQueue>();
            services.AddSingleton<IPlayerHandler, PlayerHandler>();
            services.AddSingleton<IChatHandler, ChatHandler>();
            services.AddSingleton<IGame, Game>();

            var configurationSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(configurationSection);


            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services
                .AddAuthentication()
                .AddTwitchExtensionAuth();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("TownofBlakullaAuth",
                    policy => policy.RequireClaim("extension_id",
                        configurationSection["ExtensionOwnerId"]
                    )
                );
            });

            services.AddTwitchExtensionManager();

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin());

            app.UseSession();
            app.UseDefaultFiles();
            app.UseMvc();

            var settings = app.ApplicationServices.GetService<IOptions<AppSettings>>()?.Value ?? new AppSettings();


            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    var propRepo = app.ApplicationServices.GetService<IPropertyRepository>();
                    if (e.ExceptionObject is Exception exc)
                        propRepo.Save("last-error", JsonConvert.SerializeObject(exc));
                };

            app.UseTwitchExtensionManager(app.ApplicationServices, new Dictionary<string, ExtensionBase>
            {
                {
                    settings.ExtensionId,
                    new StaticSecretExtension( new ExtensionConfiguration {
                        Id = settings.ExtensionId,
                        OwnerId = settings.ExtensionOwnerId,
                        VersionNumber = settings.ExtensionVersionNumber,
                        StartingSecret = settings.ExtensionSecret
                    })
                }
            });

        }
    }
}
