using Microsoft.AspNetCore.Mvc;
//using JwtAuth.Common;
using JwtAuth.Entities;
using JwtAuth.Models;
using JwtAuth.Services;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuth.Controllers
{
    //[Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

//        [AllowAnonymous]
        [HttpPost("authenticate")]        
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        //[Authorize]
        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    var users = _userService.GetAll();
        //    return Ok(users);
        //}
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // only allow admins to access other user records
            var currentUserId = int.Parse(User.Claims.First(x => x.Type == "id").Value);
            if (id != currentUserId && !User.IsInRole(Role.Admin))
                return Forbid();

            var user = _userService.GetById(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }
        [HttpPost]
      
        public IActionResult Post(UserRequest model)
        {
            var resp = _userService.AddUser(model);
            return Ok(resp);    
        }
    }
}
