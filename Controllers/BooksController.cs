using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Commands.Books;
using ProgLibrary.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace ProgLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        public BooksController(IBookService eventService)
        {
            _bookService = eventService;
        }
        
        [HttpGet("{bookId}")]
        [Authorize(Policy = "HasUserRole")]
        public async Task<JsonResult> Get([FromBody]Guid bookId)
        {
            var books = await _bookService.GetAsync(bookId);
            if (books == null)
            {
                return Json(new Exception("Brak książki"));
            }
            return Json(books);
        }
        
        [HttpGet("{title}")]
        [Authorize(Policy = "HasUserRole")]
        public async Task<JsonResult> Get([FromBody]string title)
        {
            var books = await _bookService.BrowseAsync(title);
            return Json(books);
        }

        
        [HttpPost("Create")]
        [Authorize(Policy = "HasUserRole")]
        public async Task<JsonResult> Create([FromBody]CreateBook command)
        {
            var Id = Guid.NewGuid();
            var result = await Task.FromResult( _bookService.CreateAsync(Id, command.Title, command.Author, command.ReleaseDate, command.Description));
            return Json(result);
        }

       
        [HttpPut("{command}")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<JsonResult> Update (Guid bookId,[FromBody]UpdateBook command)
        {
            var result = await Task.FromResult( _bookService.UpdateAsync(bookId, command.Title, command.Author, command.ReleaseDate, command.Description));
            return Json(result);
        }

        [HttpDelete("{bookId}")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<JsonResult> Delete(Guid bookId)
        {
            var result = await Task.FromResult( _bookService.DeleteAsync(bookId));

            //204
            return Json(result);
            
                 
            
        }
    }
}
