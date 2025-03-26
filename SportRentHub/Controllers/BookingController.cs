using System.Net;
using Microsoft.AspNetCore.Mvc;
using SportRentHub.Entities.Const;
using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public BookingController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var bookingDto = await _serviceManager.BookingService.GetById(id);
            if (bookingDto == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            return Ok(bookingDto);
        }

        [HttpGet("get-list")]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _serviceManager.BookingService.GetAll();
            return Ok(bookings ?? new List<BookingDto>());
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto createBookingDto)
        {
            if (createBookingDto == null)
            {
                return BadRequest("Thông tin đặt sân không hợp lệ.");
            }
            var result = await _serviceManager.BookingService.Create(createBookingDto);
            if (!result) return BadRequest(MessageError.ErrorCreate);

            var lst = new List<BookingDto>();
            lst = await _serviceManager.BookingService.GetAll();
            if (!lst.Any()) return BadRequest(MessageError.ErrorCreate);
            return Ok(lst[0]);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateBooking([FromBody] BookingUpdateDto updateBookingDto)
        {
            if (updateBookingDto == null)
            {
                return BadRequest("Thông tin cập nhật không hợp lệ.");
            }
            var booking = await _serviceManager.BookingService.GetById(updateBookingDto.Id);
            if (booking == null) return BadRequest("Đặt sân không tồn tại.");

            var result = await _serviceManager.BookingService.Update(updateBookingDto);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Cập nhật đặt sân thất bại.");
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _serviceManager.BookingService.GetById(id);
            if (booking == null)
            {
                return BadRequest("Đặt sân không tồn tại.");
            }
            await _serviceManager.BookingService.Delete(id);
            return Ok();
        }
    }
}