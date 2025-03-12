using SportRentHub.Repositories.Interfaces;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _lazyUserService;
        private readonly Lazy<IEmailService> _lazyEmailService;
        public ServiceManager(IRepositoryManager repositoryManager, IConfiguration configuration)
        {
            _lazyUserService = new Lazy<IUserService>(() => new UserService(repositoryManager));
            _lazyEmailService = new Lazy<IEmailService>(() => new EmailService(configuration));
        }

        public IUserService UserService => _lazyUserService.Value;

        public IEmailService EmailService => _lazyEmailService.Value;
    }
}
