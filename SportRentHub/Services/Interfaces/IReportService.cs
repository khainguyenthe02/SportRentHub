using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Entities.DTOs.Report;

namespace SportRentHub.Services.Interfaces
{
	public interface IReportService
	{
		Task<bool> Create(ReportCreateDto create);
		Task<List<ReportDto>> GetAll();
		Task<bool> Delete(int id);
		Task<ReportDto> GetById(int id);
		Task<bool> Update(ReportUpdateDto update);
		Task<List<ReportDto>> Search(ReportSearchDto search);
	}
}
