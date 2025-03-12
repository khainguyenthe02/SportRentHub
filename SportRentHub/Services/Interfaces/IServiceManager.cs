namespace SportRentHub.Services.Interfaces
{
    public interface IServiceManager
    {
        public IUserService UserService { get; }
        IEmailService EmailService { get; }
    }
}
