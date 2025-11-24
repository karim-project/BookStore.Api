namespace BookStore.Api.DTOs.Request
{
    public class CreateOrderRequest
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
