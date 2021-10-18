using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Commands.Reservations;
using ProgLibrary.Infrastructure.Services;
using System.Threading.Tasks;

namespace ProgLibrary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        // GET: api/Reservations
        [HttpGet]
        public async Task<IActionResult> GetReservations(string bookTitle)
        {
            var reservations = await _reservationService.BrowseAsync(bookTitle);
            return Json(await _reservationService.BrowseAsync(bookTitle));
        }

        [HttpPost]
        public async Task<IActionResult> Create ([FromBody]CreateReservation command)
        {
            await _reservationService.CreateAsync(command.Id, command.UserId, command.BookId, command.ReservationTimeFrom, command.ReservationTimeTo, command.ReservationTime);
            return Created($"/reservations/{command.Id}", null);
        }
 

  



    }
}
