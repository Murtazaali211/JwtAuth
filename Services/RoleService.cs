using JwtAuth.Common;
using JwtAuth.Entities;
using Microsoft.Extensions.Options;

namespace JwtAuth.Services
{
    public interface IRoleService
    {
       
        IEnumerable<Role> GetAll();
        Role GetById(int id);
    }
    public class RoleService : IRoleService
    {
        private List<Role> _roles = new List<Role>
        {
           new Role { Id = 1, Name = "Admin", },
           new Role { Id = 2, Name = "User" }
        };

        private readonly AppSettings _appSettings;

        public RoleService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public IEnumerable<Role> GetAll()
        {
            return _roles;
        }

        public Role GetById(int id)
        {
            return _roles.FirstOrDefault(x => x.Id == id);
        }
    }
}
