using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Commands.User;
using ProgLibrary.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace ProgLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ApiControllerBase
    {
        private IUserService _userService;             
        public AccountController(IUserService userService)
        {
            _userService = userService;          
        }

        [Authorize(Policy = "HasUserRole")]
        [HttpGet("User")]
        public async Task<JsonResult> Get()
        => Json(await _userService.GetAccountAsync(UserId));

        [Authorize(Policy = "HasUserRole")]
        [HttpGet("GetById")]
        public async Task<JsonResult> Get([FromBody]Guid userId)
        => Json(await _userService.GetAccountAsync(userId));

        [Authorize(Policy = "HasUserRole")]
        [HttpGet("GetByEmail")]
        public async Task<JsonResult> Get([FromBody] string userEmail)
       => Json(await _userService.GetAccountAsync(userEmail));

        [Authorize(Policy = "HasUserRole")]
        [HttpGet("UserBooks")]
        public async Task<JsonResult> GetBooks()
        => Json(await _userService.GetUserReservations(UserId));

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<JsonResult> Register([FromBody]Register command)
        {    
             await _userService.RegisterAsync(Guid.NewGuid(), command.Email, command.UserName, command.Password);           
            return Json(Created($"/Account/{command.Email}", "Utworzono"));
        }
        
        [AllowAnonymous]       
        [HttpPost("login")]
        public async Task<JsonResult> Login([FromBody]Login command)
        {
            var user = await _userService.LoginAsync(command.Email, command.Password);
            return Json(user);
           
        }
        
        
    }
   
}
