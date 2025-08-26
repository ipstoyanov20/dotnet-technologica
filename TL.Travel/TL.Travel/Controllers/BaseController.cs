using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TL.AspNet.Security.Abstractions.Models;
using TL.TravelAPI.Helpers;
using TL.WebHelpers.Controllers;

namespace TL.TravelAPI.Controllers
{
    [Route("[controller]/[action]")]
    public abstract class BaseController : BaseController<ErrorCodesEnum, UserSecurityModel<int>, int>, IExceptionFilter
    {
        protected override string ValueLabel => "Стойност";

        protected override void CheckForImpersonationToken(ActionExecutingContext context)
        {

        }


        [NonAction]
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            var response = new
            {
                Error = exception.Message,
                ExceptionType = exception.GetType().Name
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;
        }

    }
}
