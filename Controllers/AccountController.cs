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

        [Authorize("HasUserRole")]
        [HttpGet("User")]
        public async Task<JsonResult> Get()
        => Json(await _userService.GetAccountAsync(UserId));

        [Authorize("HasAdminRole")]
        [HttpGet("Browse")]
        public async Task<JsonResult> Browse()
        => Json(await _userService.BrowseAsync("user"));

        [Authorize("HasUserRole")]
        [HttpGet("GetById")]
        public async Task<JsonResult> Get(Guid userId)
        => Json(await _userService.GetAccountAsync(userId));

        [Authorize("HasUserRole")]
        [HttpGet("GetByEmail")]
        public async Task<JsonResult> Get(string userEmail)
        => Json(await _userService.GetAccountAsync(userEmail));

        [Authorize("HasUserRole")]
        [HttpGet("GetBooks")]
        public async Task<JsonResult> GetBooks()
        => Json(await _userService.GetUserReservations(UserId));

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<JsonResult> Register([FromBody] Register command)
        {
            await _userService.RegisterAsync(Guid.NewGuid(), command.Email, command.UserName, command.Password);
            return Json(Created($"/Account/{command.Email}", "Utworzono"));
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<JsonResult> Login([FromBody] Login command)
        {
            var user = await _userService.LoginAsync(command.Email, command.Password);
            return Json(user);

        }


    }

}
