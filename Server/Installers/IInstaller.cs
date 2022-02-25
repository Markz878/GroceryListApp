namespace GroceryListHelper.Server.Installers;

public interface IInstaller
{
    public void Install(IServiceCollection services, ConfigurationManager configuration);
}
