using HIMIS_API.Models;
using HIMIS_API.Services.LoginServices.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldLoginController : ControllerBase
    {

        private readonly ILoginRepository _loginRepository;
        public FieldLoginController(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }


        [HttpPost("LoginField")]
        public async Task<IActionResult> LoginField(FieldLogin model)
        {
            var (success, user) = await _loginRepository.FieldLoginAsync(model.DivisionID, model.PASS);

            if (success)
            {
                return Ok(new { Message = "Successfully Login", UserInfo = user });
            }

            return BadRequest("Invalid credentials.");
        }

    }
}
