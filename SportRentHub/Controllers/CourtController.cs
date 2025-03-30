using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportRentHub.Entities.Const;
using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Entities.DTOs.ChildCourt;
using SportRentHub.Entities.DTOs.Court;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public CourtController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetCourtById(int id)
        {
            var courtDto = await _serviceManager.CourtService.GetById(id);
            if (courtDto == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            var childLst = await _serviceManager.ChildCourtService.Search(new ChildCourtSearchDto { CourtId = courtDto.Id });
            if (childLst.Any())
            {
                courtDto.ChildLst = childLst;
            }
            return Ok(courtDto);
        }

        [HttpGet("get-list")]
        public async Task<IActionResult> GetCourts()
        {
            var courts = await _serviceManager.CourtService.GetAll();
            if(!courts.Any()) return Ok(courts);
            foreach (var item in courts)
            {
				var childLst = await _serviceManager.ChildCourtService.Search(new ChildCourtSearchDto { CourtId = item.Id });
				if (childLst.Any())
				{
					item.ChildLst = childLst;
				}
			}
            return Ok(courts);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCourt([FromBody] CourtCreateDto createCourtDto)
        {
            if (createCourtDto == null)
            {
                return BadRequest("Thông tin sân không hợp lệ.");
            }
            var result = await _serviceManager.CourtService.Create(createCourtDto);
            if (!result) return BadRequest(MessageError.ErrorCreate);

            var lst = new List<CourtDto>();
            lst = await _serviceManager.CourtService.GetAll();
            if (!lst.Any()) return BadRequest(MessageError.ErrorCreate);
            return Ok(lst[0]);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCourt([FromBody] CourtUpdateDto updateCourtDto)
        {
            if (updateCourtDto == null)
            {
                return BadRequest("Thông tin cập nhật không hợp lệ.");
            }
            var court = await _serviceManager.CourtService.GetById(updateCourtDto.Id);
            if (court == null) return BadRequest("Sân không tồn tại.");

            var result = await _serviceManager.CourtService.Update(updateCourtDto);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Cập nhật sân thất bại.");
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCourt(int id)
        {
            var court = await _serviceManager.CourtService.GetById(id);
            if (court == null)
            {
                return BadRequest("Sân không tồn tại.");
            }
            await _serviceManager.CourtService.Delete(id);
            return Ok();
        }
		[HttpPost("search")]
		public async Task<IActionResult> Search(CourtSearchDto search)
		{
			var searchList = await _serviceManager.CourtService.Search(search);
			if (!searchList.Any()) return Ok(searchList);
			return Ok(searchList);
		}
	}
}