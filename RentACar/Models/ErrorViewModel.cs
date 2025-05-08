namespace RentACar.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public string Message { get; set; } = "An unexpected error occurred.";
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
