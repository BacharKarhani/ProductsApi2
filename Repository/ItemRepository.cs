using Microsoft.EntityFrameworkCore;
using Product.Data;
using Product.Models;
using Product.Models.Interfaces;
using ProductsApi.Data;
using ProductsApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Data
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _db;

        public ItemRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<OperationResult> GetAllItemsAsync()
        {
            try
            {
                var items = await _db.items.ToListAsync();
                return OperationResult.Ok(items);
            }
            catch (Exception ex)
            {
                return OperationResult.Error("Error", ex.Message);
            }
        }

        public async Task<Item> GetItemByIdAsync(int id)
        {
            try
            {
                return await _db.items.FindAsync(id);
            }
            catch
            {
                return null; // or handle exceptions as per your requirements
            }
        }

        public async Task<bool> CreateItemAsync(Item item)
        {
            if (item == null)
            {
                return false;
            }

            try
            {
                _db.items.Add(item); // Only adds the Item to the database, no order handling
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> UpdateItemAsync(Item item)
        {
            var existingItem = await _db.items.FindAsync(item.Id);
            if (existingItem == null)
            {
                return false;
            }

            // Update fields including Price
            _db.Entry(existingItem).CurrentValues.SetValues(item); // This will include the Price field
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await _db.items.FindAsync(id);
            if (item != null)
            {
                _db.items.Remove(item);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ItemExistsAsync(int id)
        {
            return await _db.items.AnyAsync(e => e.Id == id);
        }
    }
}

