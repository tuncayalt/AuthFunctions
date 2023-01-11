using AuthFunctions.Data.Contexts;
using AuthFunctions.Domain.Models.Entities;

namespace AuthFunctions.Data.Repositories
{
    public class UserRepository : EfRepository<User>, IUserRepository
    {
        public UserRepository(AuthDbContext dbContext) : base(dbContext)
        {
        }
    }
}
