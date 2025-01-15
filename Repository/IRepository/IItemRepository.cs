using Product.Models;
using ProductsApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Product.Models.Interfaces
{
    public interface IItemRepository
    {
        Task<OperationResult> GetAllItemsAsync();
        Task<Item> GetItemByIdAsync(int id);
        Task<bool> CreateItemAsync(Item item); 
        Task<bool> UpdateItemAsync(Item item); 
        Task<bool> DeleteItemAsync(int id); 
        Task<bool> ItemExistsAsync(int id);
    }
}
