using Microsoft.Extensions.DependencyInjection;
using ServiceDefinitions;
using ServicesImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFunctionApp
{
    public static class ServiceInjection
    {
        public static void InjectAppServices(this IServiceCollection services)
        {
            services.AddScoped<IPersonService, PersonService>();
        }
    }
}
