namespace BookStore.Api.DTOs.Request
{
    public class UpdateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
       
        [MaxLength(1000)]
        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
