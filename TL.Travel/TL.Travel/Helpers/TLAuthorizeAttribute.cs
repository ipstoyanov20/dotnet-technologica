using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TL.AspNet.Security.Abstractions.Attributes;
using TL.AspNet.Security.Abstractions.Enums;

namespace TL.TravelAPI.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class TLAuthorizeAttribute : BaseTLAuthorizeAttribute, IAuthorizationFilter
    {
     
    }
}
