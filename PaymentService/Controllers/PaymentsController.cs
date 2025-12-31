using Microsoft.AspNetCore.Mvc;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Payment Service Works!");
}
