using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace Schedule.Api.Attributes
{
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute(params string[] claim) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { claim };
        }
    }

    public class AuthorizeFilter : IAuthorizationFilter
    {
        private readonly string[] _claim;

        public AuthorizeFilter(params string[] claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            if (isAuthenticated)
            {
                var roleClaim = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                if (roleClaim == null)
                {
                    context.Result = new ForbidResult();
                    return;
                }

                var list = _claim.ToList();
                var hasClaim = false;

                foreach (var unused in from l in list
                                       let role = roleClaim.Value.Split(",")
                                       from s in role
                                       where l == s
                                       select l)
                    hasClaim = true;

                if (!hasClaim) context.Result = new ForbidResult();
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
