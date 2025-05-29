using System.ComponentModel.DataAnnotations;

namespace RentACar.Core.Models.Chat
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public string SenderId { get; set; } = null!;
        public string SenderName { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public string Status { get; set; } = null!;
    }

    public class MessageInputModel
    {
        [Required]
        public string Content { get; set; } = null!;
        [Required]
        public string SenderId { get; set; } = null!;
        [Required]
        public string SenderName { get; set; } = null!;
    }

    public class MessageStatusUpdateModel
    {
        [Required]
        public int MessageId { get; set; }
        [Required]
        public string Status { get; set; } = null!;
    }

    public class MessageReadUpdateModel
    {
        [Required]
        public int MessageId { get; set; }
        [Required]
        public bool IsRead { get; set; }
    }
}
