using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Commands.Books;
using ProgLibrary.Infrastructure.Exceptions;
using ProgLibrary.Infrastructure.Services;
using System;
using System.Net.Http;
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

        [HttpGet("Get/{bookId}")]
        [Authorize("HasUserRole")]
        public async Task<JsonResult> Get(Guid bookId)
        {
            var books = await _bookService.GetAsync(bookId);
            if (books == null)
            {
                return Json(new Exception("Brak książki"));
            }
            return Json(books);
        }


        [HttpGet("Get")]
        [Authorize("HasUserRole")]
        public async Task<JsonResult> Get(string title)
        {
            var books = await _bookService.BrowseAsync(title);
            return Json(books);
        }


        [HttpPost("Create")]
        [Authorize("HasUserRole")]
        public async Task<JsonResult> Create([FromBody] CreateBook command)
        {
            if (ModelState.IsValid)
            {            
                var result = await _bookService.CreateAsync(Guid.NewGuid(), command.Title, command.Author, command.ReleaseDate, command.Description);
                if (result)
                {
                    return Json($"Utworzono pomyślnie książkę ");
                }
                    return Json($"Błąd tworzenia książki");
            }
            return Json("Błąd tworzenia książki", null);
            
           
        }



        [HttpPut("Update/{id}")]
        [Authorize("HasAdminRole")]
        public async Task<JsonResult> Update(Guid id, [FromBody] UpdateBook command)
        {
            var result = await Task.FromResult(_bookService.UpdateAsync(id, command.Title, command.Author, command.ReleaseDate, command.Description));
            return Json(result);
        }




        [HttpDelete("Delete/{bookId}")]
        [Authorize("HasAdminRole")]
        public async Task<HttpResponse> Delete(Guid bookId)
        {
           
            var result = await _bookService.DeleteAsync(bookId);
            if (result)
            {
                 Response.Headers.Add("DeleteMessage","Usunięto pomyślnie książkę o id:" + bookId);
                return Response;
            }

            Response.Headers.Add("DeleteMessage", "Błąd usuwania");
            return Response;





        }
    }
}
