using ECPLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECPLibrary.Core.Controllers;

[ApiController]
[Route("api")]
public class EcpServiceDiscoveryController(IEcpServiceDiscovery discovery) : ControllerBase
{

    [HttpGet("call-service")]
    public async Task<IActionResult> CallService([FromQuery] string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return BadRequest("Service name must be provided.");
        }

        var serviceUrl = await discovery.GetServiceUrlAsync(serviceName);
        if (serviceUrl == null)
        {
            return NotFound("Service not found");
        }

        try
        {
            return Ok(serviceUrl);

        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    [HttpGet("call-load-balancer-service")]
    public async Task<IActionResult> CallAndLoadBalancingService([FromQuery] string serviceName)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return BadRequest("Service name must be provided.");
        }

        var serviceUrl = await discovery.GetServiceAndLoadBalancingUrlAsync(serviceName);
        if (serviceUrl == null)
        {
            return NotFound("Service not found");
        }

        try
        {
            return Ok(serviceUrl);

        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

}