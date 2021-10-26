using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Commands.Books;
using ProgLibrary.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace ProgLibrary.API.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        public BooksController(IBookService eventService)
        {
            _bookService = eventService;
        }
        [AllowAnonymous]
        [HttpGet("Get/{bookId}")]
        public async Task<IActionResult> Get(Guid bookId)
        {
            var books = await _bookService.GetAsync(bookId);
            if (books == null)
            {
                return NotFound();
            }
            return Json(books);
        }
        [AllowAnonymous]
        [HttpGet("Get")]
        public async Task<IActionResult> Get(string title)
        {
            var books = await _bookService.BrowseAsync(title);
            return Json(books);
        }

        
        [HttpPost("Create")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<IActionResult> Create([FromBody]CreateBook command)
        {
            var Id = Guid.NewGuid();
            var result = await Task.FromResult( _bookService.CreateAsync(Id, command.Title, command.Author, command.ReleaseDate, command.Description));
            return Json(result);
        }

        [Authorize(Policy = "HasAdminRole")]
        [HttpPut("Update/{bookId}")]
        public async Task<IActionResult> Update (Guid bookId,[FromBody]UpdateBook command)
        {
            await _bookService.UpdateAsync(bookId, command.Title, command.Author, command.ReleaseDate, command.Description);

            //204
            return Json(command);
        }

        [HttpDelete("Delete/{bookId}")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<IActionResult> Delete(Guid bookId)
        {
            await _bookService.DeleteAsync(bookId);

            //204
            return NoContent();
            
                 
            
        }
    }
}
