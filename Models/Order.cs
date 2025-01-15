namespace ProductsApi.Models
{
    public class Order
    {
        public int OrderId { get; set; } // Primary key, auto-generated
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; } // Calculated: Item Price * Quantity
        public string Username { get; set; } // User who placed the order

        // Foreign Key to Item
        public int ItemId { get; set; } // ItemId now represents the foreign key to Item

        // Navigation property (if you want to link to the Item model)
        public Item Item { get; set; }
    }
}
