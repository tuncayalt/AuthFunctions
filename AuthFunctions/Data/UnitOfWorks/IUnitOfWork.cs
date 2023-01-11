using AuthFunctions.Data.Repositories;
using System.Threading.Tasks;

namespace AuthFunctions.Data.UnitOfWorks
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; set; }
        Task<int> CompleteAsync();
    }
}
