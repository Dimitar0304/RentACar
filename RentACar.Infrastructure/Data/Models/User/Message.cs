using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentACar.Infrastructure.Data.Models.User
{
    /// <summary>
    /// Represents a message in the support chat system
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Message identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Content of the message
        /// </summary>
        [Required]
        public string Content { get; set; } = null!;

        /// <summary>
        /// Identifier of the message sender
        /// </summary>
        [Required]
        [ForeignKey(nameof(Sender))]
        public string SenderId { get; set; } = null!;

        /// <summary>
        /// Name of the message sender
        /// </summary>
        [Required]
        public string SenderName { get; set; } = null!;

        /// <summary>
        /// Timestamp when the message was sent
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Indicates whether the message has been read
        /// </summary>
        [Required]
        public bool IsRead { get; set; }

        /// <summary>
        /// Current status of the message (e.g., "Open", "In Progress", "Resolved")
        /// </summary>
        [Required]
        public string Status { get; set; } = "Open";

        /// <summary>
        /// Navigation property for the sender
        /// </summary>
        public ApplicationUser Sender { get; set; } = null!;
    }
}