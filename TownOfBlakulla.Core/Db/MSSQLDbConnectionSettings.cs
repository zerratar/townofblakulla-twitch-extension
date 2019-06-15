using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using TownOfBlakulla.EBS;

namespace TownOfBlakulla.Core.Db
{
    public class MSSQLDbConnectionSettings : IDbConnectionSettings
    {
        private readonly AppSettings settings;

        public MSSQLDbConnectionSettings(IServiceProvider services)
        {
            this.settings = services.GetService<IOptions<AppSettings>>()?.Value ?? new AppSettings();
        }

        public string ConnectionString => settings.MSSQLDbConnectionString;
    }
}