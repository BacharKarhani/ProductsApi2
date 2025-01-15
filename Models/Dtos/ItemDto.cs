namespace ProductsApi.Models.Dtos
{
    public class ItemDto
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public decimal Price { get; set; }
        // Removed Orders, as they aren't needed for Item creation
    }
}
