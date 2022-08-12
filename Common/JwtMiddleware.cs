using JwtAuth.Common;
using JwtAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using JwtAuth.Entities;

namespace JwtAuth
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            string Path, endpoint;
            var endpoints = context.GetEndpoint();
            Path = endpoint = context?.Request?.Path.Value.ToLower();
            Path = Path.Substring(Path.LastIndexOf("/") + 1).ToLower();
            
            context.Request.EnableBuffering();

             switch (Path){
                 case "authenticate":
                     //context.Response.StatusCode = (int)HttpStatusCode.OK;
                     //await LogRequestResponse(context, endpoint);
                     break;
                 default:
                     {
                         if (token != null)
                         {
                            if (IsAuthorizedRole())
                            {
                                var permissions = new RolePermissions();
                            }
                            attachUserToContext(context, userService, token);
                        }
                        else
                         {
                           await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = (int)HttpStatusCode.Unauthorized,
                                Message = ("Authoization failed").ToString(),
                            }.ToString());
                            
                            return;
                         }

                     break;
                     }
             }

            await _next(context);
        }

        private bool IsAuthorizedRole()
        {
            bool isAuthorized = false;

            return isAuthorized;
        }

        private void attachUserToContext(HttpContext context, IUserService userService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);


                var jwtToken = (JwtSecurityToken)validatedToken;

                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // attach user to context on successful jwt validation
                context.Items["User"] = userService.GetById(userId);
            }
            catch(Exception ex)
            {
                context.Response.WriteAsync(new ErrorDetails()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "." + ex.Message.ToString()
                }.ToString());               
            }
        }
        [ExcludeFromCodeCoverage]
        private async Task<bool> SkipAuthorization(AuthorizationFilterContext filterContext)
        {

            return filterContext.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        }
        [ExcludeFromCodeCoverage]
        private async Task<bool> LogRequestResponse(HttpContext httpContext, string endpoint)
        {
            try
            {

                string request = await LogRequest(httpContext.Request, endpoint);
                string response = string.Empty;

                var originalBodyStream = httpContext.Response.Body;


                using (var responseBody = new System.IO.MemoryStream())
                {

                    httpContext.Response.Body = responseBody;
                    //await next.Invoke(httpContext).ConfigureAwait(true);
                    response = await LogResponse(httpContext.Response, endpoint);
                    await responseBody.CopyToAsync(originalBodyStream);
                }

            }
            catch (Exception ex)
            {
                return true;
            }
            return true;
        }

        [ExcludeFromCodeCoverage]
        private async Task<string> LogRequest(Microsoft.AspNetCore.Http.HttpRequest request, string endpoint)
        {
            try
            {
                var body = request.Body;

                //This line allows us to set the reader for the request back at the beginning of its stream.
                request.EnableBuffering();

                //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
                var buffer = new byte[Convert.ToInt32(request.ContentLength)];

                //...Then we copy the entire request stream into the new buffer.
                await request.Body.ReadAsync(buffer, 0, buffer.Length);

                //We convert the byte[] into a string using UTF8 encoding...
                var bodyAsText = System.Text.Encoding.UTF8.GetString(buffer);

                //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
                request.Body = body;

                request.Body.Seek(0, System.IO.SeekOrigin.Begin);

                return bodyAsText;
            }
            catch (Exception)
            {
                return "";
            }
        }
        [ExcludeFromCodeCoverage]
        private async Task<string> LogResponse(HttpResponse response, string endpoint)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, System.IO.SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new System.IO.StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, System.IO.SeekOrigin.Begin);


            return text;
        }



    }
    public static class APIMiddleApp
    {
        [ExcludeFromCodeCoverage]
        public static IApplicationBuilder APIKeyBuilder(this IApplicationBuilder builder)
        {
            
            return builder.UseMiddleware<JwtMiddleware>();
            //return builder.MapControllers().RequireAuthorization();
        }
    }
    public class ErrorDetails
    {
        [ExcludeFromCodeCoverage]
        public int StatusCode { get; set; }
        [ExcludeFromCodeCoverage]
        public string Message { get; set; }
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
