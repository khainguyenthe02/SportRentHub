using SportRentHub.Repositories.Interfaces;

namespace SportRentHub.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<IUserRepository> _lazyUserRepository;
        public RepositoryManager (IConfiguration configuration)
        {
            _lazyUserRepository = new Lazy<IUserRepository>(() => new UserRepository(configuration));
        }
        public IUserRepository UserRepository => _lazyUserRepository.Value;
    }
}
