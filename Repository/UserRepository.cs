using Product.Models.Interfaces;
using Product.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ProductsApi.Models;
using ProductsApi.Data;

namespace Product.Data
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            try
            {
                return await GetByPredicateAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching user with username {username}", ex);
            }
        }
    }
}
