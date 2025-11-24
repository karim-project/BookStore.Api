namespace BookStore.Api.DTOs.Request
{
    public class UpdateBookRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public decimal Discont { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public IFormFile? img { get; set; }
    }
}
