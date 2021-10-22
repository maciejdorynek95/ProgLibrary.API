using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Commands.Reservations;
using ProgLibrary.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace ProgLibrary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : Controller
    {
        private  IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        // GET: api/Reservations
        [HttpGet]
        public async Task<IActionResult> GetReservations(string userEmail)
        {
            return Json(await _reservationService.BrowseAsync(userEmail));
        }

        [HttpPost]
        public async Task<IActionResult> Create ([FromBody]CreateReservation command)
        {
             await _reservationService.CreateAsync(Guid.NewGuid(), command.UserId, command.BookId, command.ReservationTimeFrom, command.ReservationTimeTo);
            return Json("Rezerwacja dokonana", null);
        }

    }
}
