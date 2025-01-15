namespace ProductsApi.Models.Dtos
{
    public class OrderDto
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Username { get; set; }
    }
}
