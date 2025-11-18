

namespace BookStore.Api.DTOs.Request
{
    public class ResendComfirmEmailRequset
    {
        [Required]
        public string UserNameOrEmail { get; set; } = string.Empty;
    }
}
