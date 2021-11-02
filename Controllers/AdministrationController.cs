using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Core.Domain;
using ProgLibrary.Infrastructure.Commands.Roles;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProgLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AdministrationController : ApiControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;


        public AdministrationController(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        
        [HttpGet("GetUsers")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<JsonResult> GetUsers()
        => Json (await _userManager.GetUsersInRoleAsync("user"));

        [HttpGet("GetRoles")]
        [Authorize("HasAdminRole")]
        public async Task<JsonResult> GetRoles()
        {
            var roles = await Task.FromResult(_roleManager.Roles.ToArray());
            return Json(roles);
        }

        [HttpPost]
        [Authorize("HasSuperAdminRole")]
        [HttpPost("AddRoleToUser")]
        public async Task<JsonResult> AddRoleToUser([FromBody] AddToUser command)
        {
            var role = _roleManager.Roles.Where(r => r.Name == command.roleName).FirstOrDefault();
            if (role == null)
            {
                throw new Exception($"Podana rola o nazwie { command.roleName } nie istenieje");
            }
            var user = _userManager.Users.Where(u => u.Email == command.UserEmail).FirstOrDefault();
            if (user == null)
            {
                throw new Exception($"użytkownik o podanym emailu { command.UserEmail } nie istenieje");
            }
            IdentityResult identityResult = await _userManager.AddToRoleAsync(user, role.Name);
            return Json(identityResult);
        }

        [Authorize("HasSuperAdminRole")]
        [HttpPost("CreateRole")]
        public async Task<JsonResult> CreateRole([FromBody] AddRole command)
        {
            if (await _roleManager.RoleExistsAsync(command.Name))
            {
                throw new Exception($"Taka rola o nazwie '{command.Name}' już istnieje");
            }
            var identityRole = new Role(Guid.NewGuid(), command.Name);
            IdentityResult identityResult = await _roleManager.CreateAsync(identityRole);
            return Json(identityResult);

        }


        [Authorize("HasSuperAdminRole")]
        [HttpDelete("{roleName}")]
        public async Task<JsonResult> DeleteRole(string roleName)
        {
    
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                throw new Exception($"Rola o nazwie '{roleName}' nie istnieje");
            }
            var role = _roleManager.Roles.Where(r => r.Name == roleName).FirstOrDefault();

            IdentityResult identityResult = await _roleManager.DeleteAsync(role);
            return Json(identityResult);

        }
    }
}
