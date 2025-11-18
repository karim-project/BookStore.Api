using Stripe.Climate;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Api.Models
{
    public class OrdersItem
    {
       
        public int Id { get; set; } 


        public int OrderId { get; set; }
        public Orders? Orders { get; set; }


        public int BookId { get; set; }
        public Book? Book { get; set; }


        public int Quantity { get; set; }


        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}
