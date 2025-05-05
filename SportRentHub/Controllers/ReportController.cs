using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using SportRentHub.Entities.Const;
using SportRentHub.Entities.DTOs.Report;
using SportRentHub.Entities.Models;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReportController : ControllerBase
	{
		private readonly IServiceManager _serviceManager;

		public ReportController(IServiceManager serviceManager)
		{
			_serviceManager = serviceManager;
		}

		[HttpGet("id/{id}")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> GetReportById(int id)
		{
			var reportDto = await _serviceManager.ReportService.GetById(id);
			if (reportDto == null)
			{
				return StatusCode((int)HttpStatusCode.NoContent);
			}
			return Ok(reportDto);
		}

		[HttpGet("get-list")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> GetReports()
		{
			var reports = await _serviceManager.ReportService.GetAll();
			if (!reports.Any()) return Ok(new List<ReportDto>());
			return Ok(reports);
		}

		[HttpPost("create")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> CreateReport([FromBody] ReportCreateDto createReportDto)
		{
			if (createReportDto == null)
			{
				return BadRequest("Thông tin báo cáo không hợp lệ.");
			}
			var courtId = createReportDto.CourtId;
			var post = await _serviceManager.CourtService.GetById(courtId);
			if (post == null)
			{
				return BadRequest(MessageError.UserOrPostNotExist);
			}
			var reportSearch = new ReportSearchDto
			{
				CourtId = courtId,
				UserId = createReportDto.UserId,
			};
			var reportLast = (await _serviceManager.ReportService.Search(reportSearch)).FirstOrDefault();
			if (reportLast != null)
			{
				var now = DateTime.Now;
				if (now < reportLast.CreateDate.AddHours(24))
				{
					return BadRequest(MessageError.UserHasReportedYet + reportLast.CreateDate + ". " + MessageError.ReportLessThan24h);
				}
			}
			var result = await _serviceManager.ReportService.Create(createReportDto);
			if (!result) return BadRequest(MessageError.ErrorCreate);
			return Ok(result);
		}

		[HttpPut("update")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> UpdateReport([FromBody] ReportUpdateDto updateReportDto)
		{
			if (updateReportDto == null)
			{
				return BadRequest("Thông tin cập nhật không hợp lệ.");
			}
			var report = await _serviceManager.ReportService.GetById(updateReportDto.Id);
			if (report == null) return BadRequest("Báo cáo không tồn tại.");

			var result = await _serviceManager.ReportService.Update(updateReportDto);
			if (result)
			{
				return Ok();
			}
			return BadRequest("Cập nhật báo cáo thất bại.");
		}

		[HttpDelete("delete")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> DeleteReport(int id)
		{
			var report = await _serviceManager.ReportService.GetById(id);
			if (report == null)
			{
				return BadRequest("Báo cáo không tồn tại.");
			}
			await _serviceManager.ReportService.Delete(id);
			return Ok();
		}

		[HttpPost("search")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> Search(ReportSearchDto search)
		{
			var searchList = await _serviceManager.ReportService.Search(search);
			if (!searchList.Any()) return Ok(new List<ReportDto>());
			return Ok(searchList);
		}
	}
}