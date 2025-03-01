using Consul;
using ECPLibrary.Core.Abstraction;
using ECPLibrary.Core.Attributes;
// using ECPLibrary.Extensions;

namespace ECPLibrary.Implement;

[UseAgent]
public class AgentServiceRegisterHandler(IHost host) : AgentServiceContext(host)
{
    private readonly IHost _host = host;

    protected override AgentServiceRegistration Register()
    {
        var config = _host.Services.GetRequiredService<IConfiguration>();
        // var deployType = config.GetDeployType();

        // var container = deployType
        //     ? Environment.GetEnvironmentVariable("APPLICATION_NAME")
        //     : _host.Services
        //         .GetRequiredService<IWebHostEnvironment>()
        //         .ApplicationName.ToLower();
        //
        // var instanceId = $"service-{Dns.GetHostName()}";
        //
        // var address = deployType 
        //     ? container 
        //     : ServiceDiscoveryExtension.GetDns("127.0.0.1");
        
        // var http = deployType 
        //     ? $"{config.GetProtocol()}{container}:{config.GetPort()}/health" 
        //     : $"{config.Url()}:{config.GetPort()}/health";
        
        return new AgentServiceRegistration
        {
            ID = Environment.GetEnvironmentVariable("SERVICE_ID") ?? Guid.NewGuid().ToString(),
            Name = Environment.GetEnvironmentVariable("APPLICATION_NAME") ?? "service1",
            Address = config["ASPNETCORE_URLS"]?.Replace("http://+", "localhost") ?? "localhost",
            Port = int.Parse(Environment.GetEnvironmentVariable("SERVICE_PORT") ?? "5268"),
            // ID = $"service-{Guid.NewGuid()}",
            // Name = "test-service",
            // Address = "test-service",  
            // Port = ServiceDiscoveryExtension.GetPort(),
            // Check = new AgentServiceCheck
            // {
            //     HTTP = http,
            //     Timeout = TimeSpan.FromSeconds(5),
            //     Interval = TimeSpan.FromSeconds(10)
            // }
        };
    }
}