using JwtAuth.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JwtAuth.Common
{
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (User)context.HttpContext.Items["User"];
            if (user == null)
            {
                // not logged in
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }

       
            //private readonly IList<Role> _roles;

            //public AuthorizeAttribute(params Role[] roles)
            //{
            //    _roles = roles ?? new Role[] { };
            //}

            //public void OnAuthorization(AuthorizationFilterContext context)
            //{
            //    // skip authorization if action is decorated with [AllowAnonymous] attribute
            //    var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            //    if (allowAnonymous)
            //        return;

            //    // authorization
            //    var user = (User)context.HttpContext.Items["User"];
            //    if (user == null || (_roles.Any() && !_roles.Contains(user.Role)))
            //    {
            //        // not logged in or role not authorized
            //        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            //    }
            //}


        }
}
