using System.ComponentModel.DataAnnotations;

namespace BookStore.Api.Models
{
    public class Orders
    {
        public int Id { get; set; } 


        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;


        public decimal TotalPrice { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public ICollection<OrdersItem> Items { get; set; } = new List<OrdersItem>();
    }
}
