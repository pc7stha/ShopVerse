using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Inventory Service Works!");
}
