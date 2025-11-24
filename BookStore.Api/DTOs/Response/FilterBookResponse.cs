namespace BookStore.Api.DTOs.Response
{
    public class FilterBookResponse
    {
        public string? Name { get; set; }
        public decimal? MainPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? CategoryId { get; set; }
        public bool LessQuantity { get; set; }
        public bool IsHot { get; set; }
    }
}
