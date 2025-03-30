using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportRentHub.Entities.Const;
using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Entities.DTOs.Payment;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public PaymentController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var paymentDto = await _serviceManager.PaymentService.GetById(id);
            if (paymentDto == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            return Ok(paymentDto);
        }

        [HttpGet("get-list")]
        public async Task<IActionResult> GetPayments()
        {
            var payments = await _serviceManager.PaymentService.GetAll();
            return Ok(payments ?? new List<PaymentDto>());
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto createPaymentDto)
        {
            if (createPaymentDto == null)
            {
                return BadRequest("Thông tin thanh toán không hợp lệ.");
            }
            var result = await _serviceManager.PaymentService.Create(createPaymentDto);
            if (!result) return BadRequest(MessageError.ErrorCreate);

            var lst = new List<PaymentDto>();
            lst = await _serviceManager.PaymentService.GetAll();
            if (!lst.Any()) return BadRequest(MessageError.ErrorCreate);
            return Ok(lst[0]);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePayment([FromBody] PaymentUpdateDto updatePaymentDto)
        {
            if (updatePaymentDto == null)
            {
                return BadRequest("Thông tin cập nhật không hợp lệ.");
            }
            var payment = await _serviceManager.PaymentService.GetById(updatePaymentDto.Id);
            if (payment == null) return BadRequest("Thanh toán không tồn tại.");

            var result = await _serviceManager.PaymentService.Update(updatePaymentDto);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Cập nhật thanh toán thất bại.");
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _serviceManager.PaymentService.GetById(id);
            if (payment == null)
            {
                return BadRequest("Thanh toán không tồn tại.");
            }
            await _serviceManager.PaymentService.Delete(id);
            return Ok();
        }
		[HttpPost("search")]
		public async Task<IActionResult> Search(PaymentSearchDto search)
		{
			var searchList = await _serviceManager.PaymentService.Search(search);
			if (!searchList.Any()) return Ok(searchList);
			return Ok(searchList);
		}
	}
}