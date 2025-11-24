namespace BookStore.Api.DTOs.Request
{
    public record FilterBookReaquest(string name, decimal? MainPrice, decimal? MaxPrice, int? categoryId, bool LessQuantity, bool isHot);
}
