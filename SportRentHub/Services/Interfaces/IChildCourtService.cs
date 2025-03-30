using SportRentHub.Entities.DTOs.ChildCourt;

namespace SportRentHub.Services.Interfaces
{
	public interface IChildCourtService
	{
		Task<bool> Create(ChildCourtCreateDto create);
		Task<List<ChildCourtDto>> GetAll();
		Task<bool> Delete(int id);
		Task<ChildCourtDto> GetById(int id);
		Task<bool> Update(ChildCourtUpdateDto update);
		Task<List<ChildCourtDto>> Search(ChildCourtSearchDto search);
	}
}
