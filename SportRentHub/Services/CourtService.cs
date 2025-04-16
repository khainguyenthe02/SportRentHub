using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Serilog;
using SportRentHub.Entities.DTOs.Court;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Services
{
    public class CourtService : ICourtService
    {
        private readonly IRepositoryManager _repositoryManager;
        public CourtService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<bool> Create(CourtCreateDto create)
        {
            try
            {
                var court = create.Adapt<Court>();
                court.CreateDate = DateTime.Now;
                court.UpdateDate = DateTime.Now;
                court.Images =  string.Join(",", create.Images);

                var result = await _repositoryManager.CourtRepository.Create(court);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[CourtService] Error creating court: {@createDto}", create, ex.Message);
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var result = await _repositoryManager.CourtRepository.Delete(id);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[CourtService] Error deleting court with ID: {Id}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<CourtDto>> GetAll()
        {
            try
            {
                var courts = await _repositoryManager.CourtRepository.GetAll();
                return await FilterData(courts.Adapt<List<CourtDto>>());
            }
            catch (Exception ex)
            {
                Log.Error("[CourtService] Error fetching all courts.", ex.Message);
                throw;
            }
        }

        public async Task<CourtDto> GetById(int id)
        {
            try
            {
                var court = await _repositoryManager.CourtRepository.GetById(id);
                if(court == null) return new CourtDto();
                var courtDto = court.Adapt<CourtDto>();
                if (!string.IsNullOrEmpty(court.Images))
                {
                    courtDto.Images = court.Images.Split(',').ToList();
                }
                return  (await FilterData(new List<CourtDto> { courtDto })).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error("[CourtService] Error fetching court with ID: {Id}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<CourtDto>> Search(CourtSearchDto search)
        {
            try
            {
                var courts = await _repositoryManager.CourtRepository.Search(search);
                return await FilterData(courts.Adapt<List<CourtDto>>());
            }
            catch (Exception ex)
            {
                Log.Error("[CourtService] Error searching courts: {@Search}", search, ex.Message);
                throw;
            }
        }

        public async Task<bool> Update(CourtUpdateDto update)
        {
            try
            {
                var existingCourt = await _repositoryManager.CourtRepository.GetById(update.Id);
                if (existingCourt == null)
                {
                    Log.Warning("[CourtService] Court not found with ID: {Id}", update.Id);
                    return false;
                }
                update.Adapt(existingCourt);
                existingCourt.UpdateDate = DateTime.Now;
                existingCourt.Images = string.Join(",", update.Images);

                var result = await _repositoryManager.CourtRepository.Update(existingCourt);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[CourtService] Error updating court: {@Update}", update, ex.Message);
                throw;
            }
        }
        public async Task<List<CourtDto>> FilterData(List<CourtDto> lst)
        {
            if (lst.Any())
            {
                //adapter user
                var userIdLst = lst.Where(item => item.UserId != 0).Select(item => item.UserId).ToList();
                if (userIdLst.Any())
                {
                    var searchUser = new UserSearchDto
                    {
                        IdLst = userIdLst,
                    };
                    var userLst = (await _repositoryManager.UserRepository.Search(searchUser))
                        .ToDictionary(u => u.Id, u => (u.Fullname, u.PhoneNumber));
                    if (userLst.Any())
                    {
                        foreach (var item in lst)
                        {
                            if (item.UserId != 0 && userLst.ContainsKey(item.UserId))
                            {
                                item.UserFullname = userLst[item.UserId].Fullname;
                                item.UserPhoneNumber = userLst[item.UserId].PhoneNumber;
                            }
                        }
                    }
                }
            }
            return lst;
        }
    }
}