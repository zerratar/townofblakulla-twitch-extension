using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TownOfBlakulla.Core;
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

            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddSingleton<ITwitchAuth, TwitchAuth>();
            services.AddSingleton<IActionQueue, ActionQueue>();
            services.AddSingleton<IPlayerHandler, PlayerHandler>();
            services.AddSingleton<IGame, Game>();

            services
                .AddAuthentication()
                .AddTwitchExtensionAuth();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("TownofBlakullaAuth",
                    policy => policy.RequireClaim("extension_id",
                        "4bsfmhaxm72zd5izc8dj2ru7mqpmi0"
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

            //app.UseStaticFiles(); // For the wwwroot folder
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider("G:\\git\\townofblakulla\\extension-getting-started\\public"),
            //    RequestPath = ""
            //});

            app.UseDefaultFiles();
            app.UseMvc();

            app.UseTwitchExtensionManager(app.ApplicationServices, new Dictionary<string, ExtensionBase>
            {
                {
                    "4bsfmhaxm72zd5izc8dj2ru7mqpmi0",
                    new StaticSecretExtension( new ExtensionConfiguration {
                        Id = "4bsfmhaxm72zd5izc8dj2ru7mqpmi0",
                        OwnerId= "zerratar",
                        VersionNumber ="0.0.1",//e.g. 0.0.1
                        StartingSecret = "jRNt+mbviSB3BB2K6Vf3mTra+DarsARd58NuQxz0ekM="
                    })
                }
            });

        }
    }
}
