using ECPLibrary.Core.Configuration;
using ECPLibrary.Services;

namespace ECPLibrary.Extensions;

public static class CoreServiceCollections
{

    public static void AddCoreEcpLibrary(this IServiceCollection services)
    {
        services.AddScoped<IConfigurationModeling, ConfigurationModeling>();
    }
}