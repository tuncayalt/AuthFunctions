using AuthFunctions.Data.Contexts;
using AuthFunctions.Data.Repositories;
using System.Threading.Tasks;

namespace AuthFunctions.Data.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _dbContext;

        public UnitOfWork(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
            UserRepository = new UserRepository(dbContext);
        }

        public IUserRepository UserRepository { get; set; }

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
