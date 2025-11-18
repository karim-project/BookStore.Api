using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Api.Models
{
    public class Book
    {
        public int Id { get; set; } 


        [Required]
        [MaxLength(250)]
        public string Title { get; set; } = string.Empty;


        [MaxLength(200)]
        public string Author { get; set; } = string.Empty;


        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }


        public string? Description { get; set; }


        public string? Image { get; set; }


        // Foreign key
        public int CategoryId { get; set; }
        public Category? Category { get; set; }


        public bool IsAvailable { get; set; } = true;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
