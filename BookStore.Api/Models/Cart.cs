using System.ComponentModel.DataAnnotations;

namespace BookStore.Api.Models
{
    public class Cart
    {
        public int Id { get; set; } 


        public int BookId { get; set; }
        public Book? Book { get; set; }


        public int Quantity { get; set; } 


        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
