using ProductsApi.Models;
using System.Threading.Tasks;

namespace Product.Models.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task AddAsync(User user);
    }
}
