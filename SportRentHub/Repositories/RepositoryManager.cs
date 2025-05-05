using SportRentHub.Repositories.Interfaces;

namespace SportRentHub.Repositories
{
	public class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<IUserRepository> _lazyUserRepository;
        private readonly Lazy<ICourtRepository> _lazyCourtRepository;
        private readonly Lazy<IReviewRepository> _lazyReviewRepository;
        private readonly Lazy<IBookingRepository> _lazyBookingRepository;
        private readonly Lazy<IPaymentRepository> _lazyPaymentRepository;
        private readonly Lazy<IChildCourtRepository> _lazyChildCourtRepository;
		private readonly Lazy<IReportRepository> _lazyReportRepository;
		public RepositoryManager (IConfiguration configuration)
        {
            _lazyUserRepository = new Lazy<IUserRepository>(() => new UserRepository(configuration));
            _lazyCourtRepository = new Lazy<ICourtRepository>(() => new CourtRepository(configuration));
            _lazyReviewRepository = new Lazy<IReviewRepository>(() => new ReviewRepository(configuration));
            _lazyBookingRepository = new Lazy<IBookingRepository>(() => new BookingRepository(configuration));
            _lazyPaymentRepository = new Lazy<IPaymentRepository>(() => new PaymentRepository(configuration));
            _lazyChildCourtRepository = new Lazy<IChildCourtRepository>(() => new ChildCourtRepository(configuration));
			_lazyReportRepository = new Lazy<IReportRepository>(() => new ReportRepository(configuration));
		}
        public IUserRepository UserRepository => _lazyUserRepository.Value;

        public ICourtRepository CourtRepository => _lazyCourtRepository.Value;

        public IReviewRepository ReviewRepository => _lazyReviewRepository.Value;

        public IBookingRepository BookingRepository => _lazyBookingRepository.Value;

        public IPaymentRepository PaymentRepository => _lazyPaymentRepository.Value;

		public IChildCourtRepository ChildCourtRepository => _lazyChildCourtRepository.Value;

		public IReportRepository ReportRepository => _lazyReportRepository.Value;
	}
}
