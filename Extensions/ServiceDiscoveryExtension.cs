using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Consul;
using ECPLibrary.Core.Attributes;
using ECPLibrary.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ECPLibrary.Extensions;

public static class ServiceDiscoveryExtension
{
    private const string Localhost = "127.0.0.1";
    private const string ConsulHost = "http://consul:8500";
    
    /// <summary>
    /// Retrieves the first available IPv4 address of the current machine. 
    /// If no valid IPv4 address is found, returns the specified prefix.
    /// </summary>
    /// <param name="prefix">The default value to return if no IPv4 address is found.</param>
    /// <returns>The first available IPv4 address as a string, or the provided prefix if no address is found.</returns>
    public static string GetDns(string prefix)
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList
            .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)
            ?.ToString() ?? prefix;
    }
    
    /// <summary>
    /// Retrieves the protocol (HTTP or HTTPS) based on the ASP.NET Core configuration.
    /// </summary>
    /// <param name="config">The application configuration instance.</param>
    /// <returns>
    /// Returns "https://" if the `ASPNETCORE_URLS` setting starts with "https://", otherwise returns "http://".
    /// </returns>
    public static string GetProtocol(this IConfiguration config)
    {
        var url = config["ASPNETCORE_URLS"];
        return !string.IsNullOrEmpty(url) && url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            ? "https://"
            : "http://";
    }

    /// <summary>
    /// Retrieves the port number from the ASP.NET Core configuration.
    /// </summary>
    /// <param name="config">The application configuration instance.</param>
    /// <returns>
    /// The extracted port number from the `ASPNETCORE_URLS` setting, or the default port (8080) if unavailable.
    /// </returns>
    public static int GetPort(this IConfiguration config)
    {
        var url = config["ASPNETCORE_URLS"];
        if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return uri.Port;
        }
        return 8080;
    }

    /// <summary>
    /// Constructs a base URL using the configured protocol and the first available IPv4 address of the host.
    /// </summary>
    /// <param name="config">The application configuration instance.</param>
    /// <param name="prefix">A fallback value if the DNS resolution fails.</param>
    /// <returns>The fully constructed base URL (e.g., "http://192.168.1.10").</returns>
    public static string Url(this IConfiguration config, string prefix = Localhost) 
        => $"{config.GetProtocol()}{GetDns(prefix)}";

    /// <summary>
    /// Retrieves the deployment type from the application configuration.
    /// </summary>
    /// <param name="config">The application configuration instance.</param>
    /// <returns>
    /// <c>true</c> if the application is deployed in a Docker environment; otherwise, <c>false</c>.
    /// </returns>
    public static bool GetDeployType(this IConfiguration config)
        => config.GetValue<bool>("DeployType:Docker");
    
    /// <summary>
    /// Configures the web host URL based on the deployment type.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> used to configure the application.</param>
    public static void HostUrl(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        var host = config.Url(config.GetDeployType() ? "0.0.0.0" : Localhost);
        builder.WebHost.UseUrls($"{host}:{config.GetPort()}");
    }
    
    /// <summary>
    /// Adds core service discovery and health check functionality to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="config">The application configuration instance.</param>
    public static void AddCoreServiceDiscovery(this IServiceCollection services, IConfiguration config)
    {
        services.AddHealthChecks()
            .AddCheck("SERVICE", () => HealthCheckResult.Healthy());

        services.AddSingleton<IConsulClient>(new ConsulClient(options =>
        {
            var consulUrl = config.GetDeployType() ? ConsulHost : $"{config.Url()}:8500";
            options.Address = new Uri(consulUrl);
        }));
        
        services.EnsureAgentRegister();
    }

    /// <summary>
    /// Registers all classes annotated with <see cref="UseAgentAttribute"/> as singleton services.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    private static void EnsureAgentRegister(this IServiceCollection services)
    {
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.GetCustomAttribute<UseAgentAttribute>() != null && typeof(IAgentServiceHandler).IsAssignableFrom(type))
            .ToList()
            .ForEach(type => services.AddSingleton(typeof(IAgentServiceHandler), type));
    }
}