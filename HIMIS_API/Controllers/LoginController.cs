using HIMIS_API.Models;
using HIMIS_API.Services.LoginServices.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginRepository _loginRepository;
        public LoginController(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        [HttpGet("getAdminLoginDDL")]
        public async Task<ActionResult<IEnumerable<LoginAdmin>>> GetAdminLoginDDL()
        {
            var myList = await _loginRepository.GetAdminLoginDDLAsync();
            return Ok(myList);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginAdmin model)
        {
            var (success, user) = await _loginRepository.LoginAsync(model.AGENCYID, model.PASS);

            if (success)
            {
                return Ok(new { Message = "Successfully Login", UserInfo = user });
            }

            return BadRequest("Invalid credentials.");
        }
    }
}
