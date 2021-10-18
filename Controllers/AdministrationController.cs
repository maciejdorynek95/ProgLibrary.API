using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Core.Domain;
using ProgLibrary.Infrastructure.Commands.Roles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgLibrary.API.Controllers
{
    [Route("[controller]")]  
    public class AdministrationController : ApiControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
       
        public AdministrationController(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        [Authorize("HasAdminRole")]
        [HttpGet]
        public async Task<IActionResult> GetRules()
        {
             List<Role> Roles = new List<Role>();
             Roles = _roleManager.Roles.ToList();
             return Json(Roles);
        }



        [AllowAnonymous]
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] AddRole command)
        {
            if (await _roleManager.RoleExistsAsync(command.Name))
            {
                throw new Exception($"Taka rola o nazwie '{command.Name}' już istnieje");
            }
            var identityRole = new Role(Guid.NewGuid(), command.Name);
            IdentityResult identityResult = await _roleManager.CreateAsync(identityRole);
            return Json(identityResult);

        }


        [Authorize("HasAdminRole")]
        [HttpDelete("DeleteRole")]
        public async Task<IActionResult> DeleteRole([FromBody] Role command)
        {
            if (!await _roleManager.RoleExistsAsync(command.Name))
            {
                throw new Exception($"Rola o nazwie '{command.Name}' nie istnieje");
            }
            var role = _roleManager.Roles.Where(r => r.Name == command.Name).FirstOrDefault();

            IdentityResult identityResult = await _roleManager.DeleteAsync(role);
            return Json(identityResult);

        }
    }
}
