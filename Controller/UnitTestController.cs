using ECPLibrary.Core.UnitOfWork;
using ECPLibrary.Persistent;
using Microsoft.AspNetCore.Mvc;

namespace ECPLibrary.Controller;

[ApiController]
[Route("unit/test")]
public class UnitTestController(IUnitOfWork<EcpDatabase> _) : ControllerBase
{
    private static readonly List<dynamic> Users =
    [
        new { Id = 1, Name = "Alice", Email = "alice@example.com" },
        new { Id = 2, Name = "Bob", Email = "bob@example.com" }
    ];

    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(Users);
    }
}