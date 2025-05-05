using SportRentHub.Entities.DTOs.Report;
using SportRentHub.Entities.Models;

namespace SportRentHub.Repositories.Interfaces
{
	public interface IReportRepository
	{
		Task<bool> Create(Report create);
		Task<List<Report>> GetAll();
		Task<bool> Update(Report update);
		Task<bool> Delete(int id);
		Task<Report> GetById(int id);
		Task<List<Report>> Search(ReportSearchDto search);
	}
}
