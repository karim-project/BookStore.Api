namespace BookStore.Api.DTOs.Request
{
    public class ForgetPasswordRequest
    {
        [Required]
        public string UserNameOrEmail { get; set; } = string.Empty;
    }
}
