using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgLibrary.API.Controllers
{
    [Route("[controller]")]
    public class BooksController : Controller
    {
        private readonly IBookService _bookservice;
        public BooksController(IBookService eventService)
        {
            _bookservice = eventService;
        }
        [HttpGet]
        public async Task<IActionResult> Get(string title)
        {
            var books = await _bookservice.BrowseAsync(title);
            return Json(books);
        }
    }
}
