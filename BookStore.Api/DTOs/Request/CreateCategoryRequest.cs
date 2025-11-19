namespace BookStore.Api.DTOs.Request
{
    public class CreateCategoryRequest
    {

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
