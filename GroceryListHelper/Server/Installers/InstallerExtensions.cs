using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace GroceryListHelper.Server.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallAssemblyServices(this IServiceCollection services, IConfiguration configuration)
        {
            var installers = typeof(Startup).Assembly.ExportedTypes
                .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IInstaller>();
            foreach (var installer in installers)
            {
                installer.Install(services, configuration);
            }
        }
    }
}
