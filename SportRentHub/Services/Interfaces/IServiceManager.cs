namespace SportRentHub.Services.Interfaces
{
    public interface IServiceManager
    {
        IUserService UserService { get; }
        IEmailService EmailService { get; }
        ICourtService CourtService { get; }
        IReviewService ReviewService { get; }
        IBookingService BookingService { get; }
        IPaymentService PaymentService { get; }
        IChildCourtService ChildCourtService { get; }
		IReportService ReportService { get; }
	}
}
