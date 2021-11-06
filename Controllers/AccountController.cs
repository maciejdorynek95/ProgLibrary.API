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
        private readonly IUserService _userService;
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
        [HttpGet("GetById/{userId}")]
        public async Task<JsonResult> GetById(Guid userId)
        => Json(await _userService.GetAccountAsync(userId));

        [Authorize("HasUserRole")]
        [HttpGet("GetByEmail")]
        public async Task<JsonResult> GetByEmail(string userEmail)
        => Json(await _userService.GetAccountAsync(userEmail));

        [Authorize("HasUserRole")]
        [HttpGet("GetBooks")]
        public async Task<JsonResult> GetBooks()
        => Json(await _userService.GetUserReservations(UserId));

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<JsonResult> Register([FromBody] Register command)
        {
            var result = await _userService.RegisterAsync(Guid.NewGuid(), command.Email, command.Password);
            return Json(result);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<JsonResult> Login([FromBody] Login command)
        {
            var user = await _userService.LoginAsync(command.Email, command.Password);
            return Json(user);

        }

        [Authorize("HasAdminRole")]
        [HttpDelete("Delete/{userId}")]
        public async Task<JsonResult> Delete(Guid userId)
        {
            var result = await _userService.DeleteAsync(userId);
            return Json(result);

        }

    }

}
