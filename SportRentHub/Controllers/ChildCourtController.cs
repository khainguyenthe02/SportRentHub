using System.Net;
using Microsoft.AspNetCore.Mvc;
using SportRentHub.Entities.Const;
using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Entities.DTOs.ChildCourt;
using SportRentHub.Entities.Models;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChildCourtController : ControllerBase
	{
		private readonly IServiceManager _serviceManager;

		public ChildCourtController(IServiceManager serviceManager)
		{
			_serviceManager = serviceManager;
		}

		[HttpGet("id/{id}")]
		public async Task<IActionResult> GetChildCourtById(int id)
		{
			var childCourtDto = await _serviceManager.ChildCourtService.GetById(id);
			if (childCourtDto == null)
			{
				return StatusCode((int)HttpStatusCode.NoContent);
			}
			return Ok(childCourtDto);
		}

		[HttpGet("get-list")]
		public async Task<IActionResult> GetChildCourts()
		{
			var childCourts = await _serviceManager.ChildCourtService.GetAll();
			if (!childCourts.Any()) return Ok(new List<ChildCourtDto>());
			return Ok(childCourts);
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateChildCourt([FromBody] ChildCourtCreateDto createChildCourtDto)
		{
			if (createChildCourtDto == null)
			{
				return BadRequest("Thông tin sân con không hợp lệ.");
			}
			var court = await _serviceManager.CourtService.GetById(createChildCourtDto.CourtId);
			if(court != null && (createChildCourtDto.RentCost >= court.MaxPrice || createChildCourtDto.RentCost <= court.MinPrice))
			{
				return BadRequest(" Giá thuê cho sân con không nằm trong khoảng giá được thiết lập của sân chính.");
			}
			var result = await _serviceManager.ChildCourtService.Create(createChildCourtDto);
			if (!result) return BadRequest(MessageError.ErrorCreate);

			var lst = await _serviceManager.ChildCourtService.GetAll();
			if (!lst.Any()) return BadRequest(MessageError.ErrorCreate);
			return Ok(lst[0]);
		}

		[HttpPut("update")]
		public async Task<IActionResult> UpdateChildCourt([FromBody] ChildCourtUpdateDto updateChildCourtDto)
		{
			if (updateChildCourtDto == null)
			{
				return BadRequest("Thông tin cập nhật không hợp lệ.");
			}
			var childCourt = await _serviceManager.ChildCourtService.GetById(updateChildCourtDto.Id);
			if (childCourt == null) return BadRequest("Sân con không tồn tại.");
			if (updateChildCourtDto.CourtId.HasValue)
			{
				var court = await _serviceManager.CourtService.GetById((int)updateChildCourtDto.CourtId);
				if (court != null && (updateChildCourtDto.RentCost >= court.MaxPrice || updateChildCourtDto.RentCost <= court.MinPrice))
				{
					return BadRequest(" Giá thuê cho sân con không nằm trong khoảng giá được thiết lập của sân chính.");
				}
			}			
			var result = await _serviceManager.ChildCourtService.Update(updateChildCourtDto);
			if (result)
			{
				return Ok();
			}
			return BadRequest("Cập nhật sân con thất bại.");
		}

		[HttpDelete("delete")]
		public async Task<IActionResult> DeleteChildCourt(int id)
		{
			var childCourt = await _serviceManager.ChildCourtService.GetById(id);
			if (childCourt == null)
			{
				return BadRequest("Sân con không tồn tại.");
			}
			await _serviceManager.ChildCourtService.Delete(id);
			return Ok();
		}
		[HttpPost("search")]
		public async Task<IActionResult> Search(ChildCourtSearchDto search)
		{
			var searchList = await _serviceManager.ChildCourtService.Search(search);
			if (!searchList.Any()) return Ok(new List<ChildCourtDto>());
			return Ok(searchList);
		}
	}
}