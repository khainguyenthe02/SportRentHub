using System.Net;
using Microsoft.AspNetCore.Mvc;
using SportRentHub.Entities;
using SportRentHub.Entities.Const;
using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Entities.DTOs.Payment;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Services.Interfaces;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using SportRentHub.Entities.Enum;

namespace SportRentHub.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IServiceManager _serviceManager;
		private readonly IVnpay _vnpay;
		private readonly IConfiguration _configuration;


        public PaymentController(IServiceManager serviceManager, IConfiguration configuration)
		{
			_serviceManager = serviceManager;
			_configuration = configuration;

			// Kiểm tra giá trị cấu hình
			var tmnCode = _configuration["Vnpay:TmnCode"];
			var hashSecret = _configuration["Vnpay:HashSecret"];
			var baseUrl = _configuration["Vnpay:BaseUrl"];
			var callbackUrl = _configuration["Vnpay:CallbackUrl"];

			if (string.IsNullOrEmpty(tmnCode) || string.IsNullOrEmpty(hashSecret) || string.IsNullOrEmpty(baseUrl) || string.IsNullOrEmpty(callbackUrl))
			{
				throw new ArgumentException("Không tìm thấy BaseUrl, TmnCode, HashSecret, hoặc CallbackUrl");
			}

			// Khởi tạo VNPay với cấu hình
			_vnpay = new Vnpay();
			_vnpay.Initialize(tmnCode, hashSecret, baseUrl, callbackUrl);
		}
		[HttpGet("IpnAction")]
		public IActionResult IpnAction()
		{
			if (Request.QueryString.HasValue)
			{
				try
				{
					var paymentResult = _vnpay.GetPaymentResult(Request.Query);
					if (paymentResult.IsSuccess)
					{
						return Ok("Thành cônggg");
					}
					return BadRequest("Thanh toán thất bại");
				}
				catch (Exception ex)
				{
					return BadRequest(ex.Message);
				}
			}

			return NotFound("Không tìm thấy thông tin thanh toán.");
		}

		[HttpGet("CreatePaymentUrl")]
		[Authorize(Roles = "Admin, User")]
		public async Task<ActionResult<string>> CreatePaymentUrlAsync(int paymentId, double moneyToPay)
		{
			try
			{
				var ipAddress = NetworkHelper.GetIpAddress(HttpContext);

				var request = new PaymentRequest
				{
					PaymentId = DateTime.Now.Ticks,
					Money = moneyToPay,
					Description = paymentId.ToString(),
					IpAddress = ipAddress,
					BankCode = BankCode.ANY,
					CreatedDate = DateTime.Now.AddHours(12),
					Currency = Currency.VND,
					Language = DisplayLanguage.Vietnamese
				};
				var payment = await _serviceManager.PaymentService.GetById(paymentId);
				if (payment == null)
				{
					return NotFound("Không tìm thấy thông tin thanh toán.");
				}
				if (payment.Status == (int)PaymentStatus.PAID)
				{
					return BadRequest("Thanh toán đã được thực hiện trước đó.");
				}

				var paymentUrl = _vnpay.GetPaymentUrl(request);

				return Created(paymentUrl, paymentUrl);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

        [HttpGet("Callback")]
        public async Task<IActionResult> Callback()
        {
            var successUrl = _configuration["URL_PaymentResult:URL_Success"];
            var failUrl = _configuration["URL_PaymentResult:URL_Failed"];

            if (!Request.QueryString.HasValue)
            {
                return GenerateHtmlRedirect(failUrl, "Không tìm thấy thông tin thanh toán.");
            }

            try
            {
                var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                if (!paymentResult.IsSuccess)
                {
                    return GenerateHtmlRedirect(failUrl, "Thanh toán thất bại.");
                }

                string orderInfo = Request.Query["vnp_OrderInfo"];
                if (!int.TryParse(orderInfo, out int paymentId))
                {
                    return GenerateHtmlRedirect(failUrl, "Không đọc được mã thanh toán.");
                }

                var payment = await _serviceManager.PaymentService.GetById(paymentId);
                if (payment == null)
                {
                    return GenerateHtmlRedirect(failUrl, "Không tìm thấy thanh toán tương ứng.");
                }

                if (payment.Status == (int)PaymentStatus.PAID)
                {
                    return GenerateHtmlRedirect(successUrl, "Thanh toán đã được xử lý trước đó.");
                }

                var updatePayment = new PaymentUpdateDto
                {
                    Id = paymentId,
                    Status = (int)PaymentStatus.PAID
                };
                var updatePaymentResult = await _serviceManager.PaymentService.Update(updatePayment);
                if (!updatePaymentResult)
                {
                    return GenerateHtmlRedirect(failUrl, "Cập nhật trạng thái thanh toán thất bại.");
                }

                var booking = await _serviceManager.BookingService.GetById(payment.BookingId);
                if (booking != null && booking.Status == (int)BookingStatus.BOOKED)
                {
                    var updateBooking = new BookingUpdateDto
                    {
                        Id = booking.Id,
                        Status = (int)BookingStatus.PAID
                    };
                    await _serviceManager.BookingService.Update(updateBooking);
                }

                return GenerateHtmlRedirect(successUrl, "Thanh toán thành công. Đang chuyển hướng...");
            }
            catch (Exception)
            {
                return GenerateHtmlRedirect(failUrl, "Đã xảy ra lỗi khi xử lý thanh toán.");
            }
        }

        /// <summary>
        /// Trả về HTML có <meta> redirect để hỗ trợ redirect trên host như Somee.
        /// </summary>
        private IActionResult GenerateHtmlRedirect(string url, string message)
        {
            string html = $@"
				<html>
				  <head>
					<meta charset='utf-8' />
					<meta http-equiv='refresh' content='1;url={url}' />
					<title>Đang chuyển hướng...</title>
					<style>
					  body {{ font-family: Arial, sans-serif; text-align: center; padding: 50px; }}
					  h1 {{ color: green; }}
					</style>
				  </head>
				  <body>
					<h1>{message}</h1>
					<p>Nếu bạn không được chuyển hướng, <a href='{url}'>nhấn vào đây</a>.</p>
				  </body>
				</html>";
            return Content(html, "text/html");
        }



        [HttpGet("id/{id}")]
		[Authorize(Roles = "Admin, User")]
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
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> GetPayments()
		{
			var payments = await _serviceManager.PaymentService.GetAll();
			return Ok(payments ?? new List<PaymentDto>());
		}

		[HttpPost("create")]
		[Authorize(Roles = "Admin, User")]
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
		[Authorize(Roles = "Admin, User")]
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
		[Authorize(Roles = "Admin, User")]
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
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> Search(PaymentSearchDto search)
		{
			var searchList = await _serviceManager.PaymentService.Search(search);
			if (!searchList.Any()) return Ok(searchList);
			return Ok(searchList);
		}
	}
}