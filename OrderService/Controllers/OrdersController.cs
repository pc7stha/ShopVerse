using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class OrdersController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Order Service Works!");
}
