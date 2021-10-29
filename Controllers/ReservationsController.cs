using Microsoft.AspNetCore.Mvc;
using ProgLibrary.Infrastructure.Commands.Reservations;
using ProgLibrary.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace ProgLibrary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : Controller
    {
        private  IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }


        [HttpGet("{userEmail}")]
        public async Task<JsonResult> GetReservations([FromBody] string userEmail)
        {
            return Json(await _reservationService.BrowseAsync(userEmail));
        }

        [HttpGet("{command}")]
        public async Task<JsonResult> Create ([FromBody]CreateReservation command)
        {
             await _reservationService.CreateAsync(Guid.NewGuid(), command.UserId, command.BookId, command.ReservationTimeFrom, command.ReservationTimeTo);
            return Json("Rezerwacja dokonana", null);
        }

    }
}
