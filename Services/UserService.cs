using JwtAuth.Common;
using JwtAuth.Entities;
using JwtAuth.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuth.Services
{
        public interface IUserService
        {
            AuthenticateResponse Authenticate(AuthenticateRequest model);
            IEnumerable<User> GetAll();
            User GetById(int id);
            int AddUser(UserRequest model);
    }

        public class UserService : IUserService
        {
            // users hardcoded for simplicity, store in a db with hashed passwords in production applications
            private List<User> _users = new List<User>
            {
                new User { Id = 1, FirstName = "Admin", LastName = "User", Username = "admin", Password = "admin", Role = Role.Admin },
                new User { Id = 2, FirstName = "Normal", LastName = "User", Username = "user", Password = "user", Role = Role.User }
            };

            private readonly AppSettings _appSettings;

            public UserService(IOptions<AppSettings> appSettings)
            {
                _appSettings = appSettings.Value;
            }

            public AuthenticateResponse Authenticate(AuthenticateRequest model)
            {
                var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);

                // return null if user not found
                if (user == null) return null;

                // authentication successful so generate jwt token
                var token = generateJwtToken(user);
                user.Token = token;
                return new AuthenticateResponse(user, token);
            }

            public IEnumerable<User> GetAll()
            {
                return _users;
            }

            public User GetById(int id)
            {
                return _users.FirstOrDefault(x => x.Id == id);
            }
            
        public int AddUser(UserRequest model)
        {
            User user = new User() { Id = _users.Count + 1, FirstName = model.FirstName, LastName = model.LastName, Password = model.Password, Role = model.Role, Username = model.Username };
            _users.Add(user);
            return 1;
        }
        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}