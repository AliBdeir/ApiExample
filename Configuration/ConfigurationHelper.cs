using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public static class ConfigurationHelper
    {
        public static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static string GetConnectionString(string name)
        {
            var configuration = GetConfiguration();
            string connectionString = configuration.GetConnectionString(name) ?? throw new Exception($"{name} ConnectionString not specified");
            return connectionString;
        }
    }
}
