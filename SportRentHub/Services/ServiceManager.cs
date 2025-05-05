using SportRentHub.Repositories.Interfaces;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Services
{
	public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _lazyUserService;
        private readonly Lazy<IEmailService> _lazyEmailService;
        private readonly Lazy<ICourtService> _lazyCourtService;
        private readonly Lazy<IReviewService> _lazyReviewService;
        private readonly Lazy<IBookingService> _lazyBookingService;
        private readonly Lazy<IPaymentService> _lazyPaymentService;
        private readonly Lazy<IChildCourtService> _lazyChildCourtService;
		private readonly Lazy<IReportService> _lazyReportService;

		public ServiceManager(IRepositoryManager repositoryManager, IConfiguration configuration)
        {
            _lazyUserService = new Lazy<IUserService>(() => new UserService(repositoryManager));
            _lazyEmailService = new Lazy<IEmailService>(() => new EmailService(configuration));
            _lazyCourtService = new Lazy<ICourtService>(() => new CourtService(repositoryManager));
            _lazyReviewService = new Lazy<IReviewService>(() => new ReviewService(repositoryManager));
            _lazyBookingService = new Lazy<IBookingService>(() => new BookingService(repositoryManager));
            _lazyPaymentService = new Lazy<IPaymentService>(() => new PaymentService(repositoryManager));
            _lazyChildCourtService = new Lazy<IChildCourtService> (() => new ChildCourtService(repositoryManager));
			_lazyReportService = new Lazy<IReportService>(() => new ReportService(repositoryManager));
		}

        public IUserService UserService => _lazyUserService.Value;

        public IEmailService EmailService => _lazyEmailService.Value;

        public ICourtService CourtService => _lazyCourtService.Value;

        public IReviewService ReviewService => _lazyReviewService.Value;

        public IBookingService BookingService => _lazyBookingService.Value;

        public IPaymentService PaymentService => _lazyPaymentService.Value;

		public IChildCourtService ChildCourtService => _lazyChildCourtService.Value;

		public IReportService ReportService => _lazyReportService.Value;
	}
}
