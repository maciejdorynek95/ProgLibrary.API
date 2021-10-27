using Microsoft.AspNetCore.Authorization;
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
        [HttpPost("User")]
        public async Task<JsonResult> Get()
        => Json(await _userService.GetAccountAsync(UserId));

        [Authorize(Policy = "HasUserRole")]
        [HttpPost("GetById")]
        public async Task<JsonResult> Get(Guid userId)
        => Json(await _userService.GetAccountAsync(userId));

        [Authorize(Policy = "HasUserRole")]
        [HttpPost("GetByEmail")]
        public async Task<JsonResult> Get([FromBody] string userEmail)
       => Json(await _userService.GetAccountAsync(userEmail));

        [Authorize(Policy = "HasUserRole")]
        [HttpPost("UserBooks")]
        public async Task<JsonResult> GetBooks() //pobiorę z tokenu JWT w sesji.
        => Json(await _userService.GetUserReservations(UserId));

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<JsonResult> Register([FromBody]Register command)
        {    
             await _userService.RegisterAsync(Guid.NewGuid(), command.Email, command.Name, command.Password);           
            return Json(Created($"/Account/{command.Email}", "Utworzono"));
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
