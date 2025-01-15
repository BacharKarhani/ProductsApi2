using System.Threading.Tasks;
using ProductsApi.Models;

namespace ProductsApi.Models.Interfaces
{
    public interface IOrderRepository
    {
        Task<bool> CreateOrderAsync(Order order);
        Task<IEnumerable<Order>> GetAllOrdersAsync();

    }
}
