using Consul;

namespace ECPLibrary.Services;

public interface IEcpServiceDiscovery
{
    Task<string?> GetServiceUrlAsync(string serviceName);
    
    Task<string?> GetServiceAndLoadBalancingUrlAsync(string serviceName);
}

public class EcpServiceDiscovery(IConsulClient consulClient) : IEcpServiceDiscovery
{
    public async Task<string?> GetServiceUrlAsync(string serviceName)
    {
        var services = await consulClient.Agent.Services();
        
        var service = services
            .Response
            .Values
            .FirstOrDefault(s => s.Service.Equals(serviceName, StringComparison.OrdinalIgnoreCase));

        return service != null ? $"http://{service.Address}:{service.Port}" : null;
    }
    
    public async Task<string?> GetServiceAndLoadBalancingUrlAsync(string serviceName)
    {
        var services = await consulClient.Agent.Services();
        var serviceEntries = services.Response
            .Where(s => s.Value.Service.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
            .Select(s => s.Value)
            .ToArray();

        if (serviceEntries.Length == 0)
            throw new Exception("No services registered with Consul.");

        // Simple round-robin load balancing
        var service = serviceEntries[new Random().Next(serviceEntries.Length)];
        var url = $"http://{service.Address}:{service.Port}/api/endpoint";

        return await Task.FromResult(url);
    }
    
}