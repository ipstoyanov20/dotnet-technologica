using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TL.AspNet.Security.Abstractions.Enums;
using TL.AspNet.Security.Abstractions.Models;
using TL.AspNet.Security.Controllers;

namespace TL.TravelAPI.Controllers
{
    [AllowAnonymous]
    [Route("[controller]/[action]")]
    public class SecurityController : BaseSecurityController<decimal>
    {
        public SecurityController()
            : base()
        { }

        [HttpPost]
        public IActionResult SignIn([FromBody] AuthCredentials credentials)
        {
            return base.Login(credentials, type: LoginTypes.UsernamePassword);
        }

    }
}
