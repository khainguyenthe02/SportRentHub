using Mapster;
using Serilog;
using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Entities.DTOs.ChildCourt;
using SportRentHub.Entities.DTOs.Court;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Entities.Enum;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Services
{
    public class BookingService : IBookingService
    {
        private readonly IRepositoryManager _repositoryManager;
        public BookingService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<bool> Create(BookingCreateDto create)
        {
            try
            {
                var booking = create.Adapt<Booking>();
                booking.CreateDate = DateTime.Now;
                booking.Status = (int)BookingStatus.BOOKED;

                var result = await _repositoryManager.BookingRepository.Create(booking);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[BookingService] Error creating booking: {@CreateDto}", create, ex.Message);
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var result = await _repositoryManager.BookingRepository.Delete(id);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[BookingService] Error deleting booking with ID: {Id}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<BookingDto>> GetAll()
        {
            try
            {
                var bookings = await _repositoryManager.BookingRepository.GetAll();
                return await FilterData(bookings.Adapt<List<BookingDto>>());
            }
            catch (Exception ex)
            {
                Log.Error("[BookingService] Error fetching all bookings.", ex.Message);
                throw;
            }
        }

        public async Task<BookingDto> GetById(int id)
        {
            try
            {
                var booking = await _repositoryManager.BookingRepository.GetById(id);
                var filteredList = await FilterData(new List<BookingDto> { booking.Adapt<BookingDto>() });

                return filteredList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error("[BookingService] Error fetching booking with ID: {Id}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<BookingDto>> Search(BookingSearchDto search)
        {
            try
            {
                var bookings = await _repositoryManager.BookingRepository.Search(search);
                return await FilterData(bookings.Adapt<List<BookingDto>>());
            }
            catch (Exception ex)
            {
                Log.Error("[BookingService] Error searching bookings: {@Search}", search, ex.Message);
                throw;
            }
        }

        public async Task<bool> Update(BookingUpdateDto update)
        {
            try
            {
                var existingBooking = await _repositoryManager.BookingRepository.GetById(update.Id);
                if (existingBooking == null)
                {
                    Log.Warning("[BookingService] Booking not found with ID: {Id}", update.Id);
                    return false;
                }
                update.Adapt(existingBooking);

                var result = await _repositoryManager.BookingRepository.Update(existingBooking);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[BookingService] Error updating booking: {@Update}", update, ex.Message);
                throw;
            }
        }
		public async Task<List<BookingDto>> FilterData(List<BookingDto> lst)
		{
			if (lst.Any())
			{
				// Adapter user
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
								item.UserFullName = userLst[item.UserId].Fullname;
								item.UserPhoneNumber = userLst[item.UserId].PhoneNumber;
							}
						}
					}
				}

				var childCourtIdLst = lst.Where(item => item.ChildCourtId != 0).Select(item => item.ChildCourtId).ToList();
				if (childCourtIdLst.Any())
				{
					var searchChildCourt = new ChildCourtSearchDto
					{
						IdLst = childCourtIdLst,
					};
					var childCourtLst = (await _repositoryManager.ChildCourtRepository.Search(searchChildCourt))
						.ToDictionary(c => c.Id, c => (c.ChildCourtName, c.CourtId));

					if (childCourtLst.Any())
					{
						var courtIdLst = childCourtLst.Values
							.Where(c => c.CourtId != 0)
							.Select(c => c.CourtId)
							.Distinct()
							.ToList();

						Dictionary<int, Court> courtLst = new();
						if (courtIdLst.Any())
						{
							var searchCourt = new CourtSearchDto
							{
								IdLst = courtIdLst,
							};
							var courts = await _repositoryManager.CourtRepository.Search(searchCourt);
							if (courts?.Any() == true)
							{
								courtLst = courts.ToDictionary(
									c => c.Id,
									c => new Court
									{
                                        Id = c.Id,
										CourtName = c.CourtName,
										Ward = c.Ward,
										Street = c.Street,
										District = c.District
									}
								);
							}
						}

						// Update booking items
						foreach (var item in lst)
						{
							if (item.ChildCourtId != 0 && childCourtLst.ContainsKey(item.ChildCourtId))
							{
								item.ChildCourtName = childCourtLst[item.ChildCourtId].ChildCourtName;
								var courtId = childCourtLst[item.ChildCourtId].CourtId;
								if (courtId != 0 && courtLst.ContainsKey(courtId))
								{
									var court = courtLst[courtId];
                                    item.CourtId = court.Id;
                                    item.CourtName = court.CourtName;
                                    item.CourtWard = court.Ward;
                                    item.CourtStreet = court.Street;
                                    item.CourtDistrict = court.District;
								}
							}
						}
					}
				}
			}
			return lst;
		}
	}
}
