using SportRentHub.Entities.DTOs.ChildCourt;
using SportRentHub.Entities.Models;

namespace SportRentHub.Repositories.Interfaces
{
	public interface IChildCourtRepository
	{
		Task<bool> Create(ChildCourt create);
		Task<List<ChildCourt>> GetAll();
		Task<bool> Update(ChildCourt update);
		Task<bool> Delete(int id);
		Task<ChildCourt> GetById(int id);
		Task<List<ChildCourt>> Search(ChildCourtSearchDto search);
	}
}
