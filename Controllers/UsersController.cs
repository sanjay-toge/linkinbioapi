using Microsoft.AspNetCore.Mvc;

namespace LinkBioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(new[] { "User1", "User2" });
        }
    }
}
