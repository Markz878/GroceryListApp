using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GroceryListHelper.Server.Installers;

public static class InstallerExtensions
{
    public static void InstallAssemblyServices(this IServiceCollection services, IConfiguration configuration)
    {
        IEnumerable<IInstaller> installers = typeof(Startup).Assembly.ExportedTypes
            .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IInstaller>();
        foreach (IInstaller installer in installers)
        {
            installer.Install(services, configuration);
        }
    }
}
