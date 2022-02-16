﻿namespace GroceryListHelper.Server.Installers;

public static class InstallerExtensions
{
    public static void InstallAssemblyServices(this IServiceCollection services, IConfiguration configuration)
    {
        IEnumerable<IInstaller> installers = typeof(Program).Assembly.ExportedTypes
            .Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IInstaller>();
        foreach (IInstaller installer in installers)
        {
            installer.Install(services, configuration);
        }
    }
}
