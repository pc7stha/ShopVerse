using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Inventory Service Works!");
    } //=> Ok("Inventory Service Works!");
}
