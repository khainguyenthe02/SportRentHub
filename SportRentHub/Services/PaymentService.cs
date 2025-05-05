using Mapster;
using Serilog;
using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Entities.DTOs.ChildCourt;
using SportRentHub.Entities.DTOs.Court;
using SportRentHub.Entities.DTOs.Payment;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Entities.Enum;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepositoryManager _repositoryManager;

        public PaymentService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<bool> Create(PaymentCreateDto create)
        {
            try
            {
                var payment = create.Adapt<Payment>();
                payment.CreateDate = DateTime.Now;
                payment.Status = (int)PaymentStatus.UN_PAID;

                var result = await _repositoryManager.PaymentRepository.Create(payment);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[PaymentService] Error creating payment: {@createDto}", create, ex.Message);
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var result = await _repositoryManager.PaymentRepository.Delete(id);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[PaymentService] Error deleting payment with ID: {Id}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<PaymentDto>> GetAll()
        {
            try
            {
                var payments = await _repositoryManager.PaymentRepository.GetAll();
                var result = payments.Adapt<List<PaymentDto>>();
				return await FilterData(result);
            }
            catch (Exception ex)
            {
                Log.Error("[PaymentService] Error fetching all payments.", ex.Message);
                throw;
            }
        }

        public async Task<PaymentDto> GetById(int id)
        {
            try
            {
                var payment = await _repositoryManager.PaymentRepository.GetById(id);
                return (await FilterData(new List<PaymentDto> { payment?.Adapt<PaymentDto>() })).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error("[PaymentService] Error fetching payment with ID: {Id}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<PaymentDto>> Search(PaymentSearchDto search)
        {
            try
            {
                var payments = await _repositoryManager.PaymentRepository.Search(search);
                return await FilterData( payments.Adapt<List<PaymentDto>>());
            }
            catch (Exception ex)
            {
                Log.Error("[PaymentService] Error searching payments: {@Search}", search, ex.Message);
                throw;
            }
        }

        public async Task<bool> Update(PaymentUpdateDto update)
        {
            try
            {
                var existingPayment = await _repositoryManager.PaymentRepository.GetById(update.Id);
                if (existingPayment == null)
                {
                    Log.Warning("[PaymentService] Payment not found with ID: {Id}", update.Id);
                    return false;
                }
                update.Adapt(existingPayment);

                var result = await _repositoryManager.PaymentRepository.Update(existingPayment);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[PaymentService] Error updating payment: {@Update}", update, ex.Message);
                throw;
            }
        }
        public async Task<List<PaymentDto>> FilterData(List<PaymentDto> lst)
        {
			if (!lst.Any())
				return lst;

			// Enrich user data
			var userIdLst = lst.Where(item => item.UserId != 0).Select(item => item.UserId).Distinct().ToList();
			if (userIdLst.Any())
			{
				var searchUser = new UserSearchDto { IdLst = userIdLst };
				var userLst = await _repositoryManager.UserRepository.Search(searchUser);
				if (userLst?.Any() == true)
				{
					var userDict = userLst.ToDictionary(u => u.Id, u => (u.Fullname, u.PhoneNumber));
					foreach (var item in lst.Where(i => i.UserId != 0 && userDict.ContainsKey(i.UserId)))
					{
						item.UserFullname = userDict[item.UserId].Fullname;
						item.UserPhoneNumber = userDict[item.UserId].PhoneNumber;
					}
				}
			}

			// Enrich court data through Booking -> ChildCourt -> Court
			var bookingIdLst = lst.Where(item => item.BookingId != 0).Select(item => item.BookingId).Distinct().ToList();
			if (bookingIdLst.Any())
			{
				var searchBooking = new BookingSearchDto { IdLst = bookingIdLst };
				var bookingLst = await _repositoryManager.BookingRepository.Search(searchBooking);
				if (bookingLst?.Any() == true)
				{
					var bookingDict = bookingLst.ToDictionary(b => b.Id, b => b.ChildCourtId);
					var childCourtIdLst = bookingDict.Values.Distinct().ToList();
					if (childCourtIdLst.Any())
					{
						var searchChildCourt = new ChildCourtSearchDto { IdLst = childCourtIdLst };
						var childCourtLst = await _repositoryManager.ChildCourtRepository.Search(searchChildCourt);
						if (childCourtLst?.Any() == true)
						{
							var childCourtDict = childCourtLst.ToDictionary(cc => cc.Id, cc => cc.CourtId);
							var courtIdLst = childCourtDict.Values.Distinct().ToList();
							if (courtIdLst.Any())
							{
								var searchCourt = new CourtSearchDto { IdLst = courtIdLst };
								var courtLst = await _repositoryManager.CourtRepository.Search(searchCourt);
								if (courtLst?.Any() == true)
								{
									var courtDict = courtLst.ToDictionary(c => c.Id, c => (c.Id, c.CourtName));
									foreach (var item in lst.Where(i => i.BookingId != 0 && bookingDict.ContainsKey(i.BookingId)))
									{
										var childCourtId = bookingDict[item.BookingId];
										if (childCourtDict.ContainsKey(childCourtId))
										{
											var courtId = childCourtDict[childCourtId];
											if (courtDict.ContainsKey(courtId))
											{
												item.CourtId = courtDict[courtId].Id;
												item.CourtName = courtDict[courtId].CourtName;
											}
										}
									}
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
