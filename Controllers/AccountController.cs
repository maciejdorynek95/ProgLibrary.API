using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgLibrary.API.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("[controller]")]
        public Task<IActionResult> Get()
        {
            throw new NotImplementedException();
        }

        [HttpGet("UserBooks")]
        public async Task<IActionResult> GetBooks()
        {
            throw new NotImplementedException();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register()
        {
            throw new NotImplementedException();
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            throw new NotImplementedException();
        }
    }
   
}
