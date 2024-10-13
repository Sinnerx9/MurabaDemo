using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace MurabaDemo.Controllers.Infrastructure;

[ApiController]
[Route("api/[controller]")]
[EnableCors("local")]
public class BaseController : ControllerBase
{
    
}