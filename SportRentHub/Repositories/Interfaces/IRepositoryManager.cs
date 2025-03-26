namespace SportRentHub.Repositories.Interfaces
{
    public interface IRepositoryManager
    {
        IUserRepository UserRepository { get; }
        ICourtRepository CourtRepository { get; }
        IReviewRepository ReviewRepository { get; }
        IBookingRepository BookingRepository { get; }
        IPaymentRepository PaymentRepository { get; }
    }
}
