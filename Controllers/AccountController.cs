using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Commands;
using ProgLibrary.Infrastructure.Commands.User;
using ProgLibrary.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace ProgLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ApiControllerBase // 'controller' brany z kontrolera bazowego w ApiControllerBase
    {
        private IUserService _userService;             
        public AccountController(IUserService userService)
        {
            _userService = userService;
            
        }

        //[Authorize(Policy = "HasUserRole")]
        [HttpGet("User")]
        public async Task<IActionResult> Get()
        => Json(await _userService.GetAccountAsync(UserId));

        [Authorize(Policy = "HasUserRole")]
        [HttpGet("UserId/{userId}")]
        public async Task<IActionResult> Get(Guid userId)
        => Json(await _userService.GetAccountAsync(userId));

        [Authorize(Policy = "HasUserRole")]
        [HttpGet("UserEmail/{userEmail}")]
        public async Task<IActionResult> Get(string userEmail)
       => Json(await _userService.GetAccountAsync(userEmail));

        [Authorize(Policy = "HasUserRole")]
        [HttpGet("UserBooks")]
        public async Task<IActionResult> GetBooks() //pobiorę z tokenu JWT w sesji.
        => Json(await _userService.GetUserReservations(UserId));

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]Register command)
        {    
             await _userService.RegisterAsync(Guid.NewGuid(), command.Email, command.Name, command.Password);           
            return Created($"/account/{command.Email}", "Utworzono");
        }
        
        [AllowAnonymous]
        
        [HttpPost("login")]
        public async Task<JsonResult> Login([FromBody]Login command)
        {
            var json = Json(await _userService.LoginAsync(command.Email, command.Password));
            return json;
        }
        
        
    }
   
}
