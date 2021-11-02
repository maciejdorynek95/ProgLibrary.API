using Microsoft.AspNetCore.Authorization;
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
        private  readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        [Authorize("HasUserRole")]
        [Route("[Action]")]
        public async Task<JsonResult> GetReservations()
        => Json(await _reservationService.BrowseAsync()); // dla usera

        [HttpGet("{userEmail}")]
        [Authorize("HasAdminRole")]
        public async Task<JsonResult> GetReservations(string userEmail)
        => Json(await _reservationService.BrowseAsync(userEmail));
        
     
        [HttpPost]
        [Authorize("HasUserRole")]
        [Route("[Action]")]
        public async Task<JsonResult> Create ([FromBody]CreateReservation command)
        => Json(await Task.FromResult(_reservationService.CreateAsync(Guid.NewGuid(), command.UserId, command.BookId, command.ReservationTimeFrom, command.ReservationTimeTo)));


    }
}
