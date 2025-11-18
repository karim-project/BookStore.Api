using System.ComponentModel.DataAnnotations;

namespace BookStore.Api.Models
{
    public class Category
    {
      
        public int Id { get; set; }


        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;


        public ICollection<Book>? Books { get; set; }
    }
}
