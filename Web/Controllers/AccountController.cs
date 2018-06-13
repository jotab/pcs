using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authentication;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly UserManager<User> _userManager;

        public AccountController(IAuthenticationService authentication, UserManager<User> userManager, ITokenGenerator tokenGenerator)
        {
            _authentication = authentication;
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (_authentication.ValidateCredentials(model.Login, model.Password))
            {
                var appUser = await _userManager.FindByNameAsync(model.Login);
                if (appUser == null)
                {
                    appUser = new User
                    {
                        UserName = model.Login,
                        Roles = new List<Role>()
                    };
                    var result = await _userManager.CreateAsync(appUser);
                    if (result != IdentityResult.Success)
                    {
                        BadRequest("Can't create user!");
                    }
                }

                return Ok(_tokenGenerator.GenerateJwtToken(appUser));
            }

            return BadRequest("Wrong login or password");
        }
    }

    public class LoginDto
    {
        [Required] public string Login { get; set; }
        [Required] public string Password { get; set; }
    }
}