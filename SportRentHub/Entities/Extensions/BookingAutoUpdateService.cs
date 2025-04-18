
using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Entities.Enum;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Entities.Extensions
{
    public class BookingAutoUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BookingAutoUpdateService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var serviceManager = scope.ServiceProvider.GetRequiredService<IServiceManager>();

                try
                {

                    //  Update lịch đã BOOKED nhưng đã quá giờ → UNPAID (do không thanh toán)
                    var bookedExpired = await serviceManager.BookingService.Search(new BookingSearchDto
                    {
                        Status = (int)BookingStatus.BOOKED,
                        EndTime = DateTime.Now.AddHours(12)
                    });

                    foreach (var booking in bookedExpired)
                    {
                        await serviceManager.BookingService.Update(new BookingUpdateDto
                        {
                            Id = booking.Id,
                            Status = (int)BookingStatus.UNPAID
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AutoUpdate Error] {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
            }
        }
    }
}
