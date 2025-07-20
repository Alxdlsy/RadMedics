using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadMedics.Models
{
    public class MailMessage
    {
        public int Id { get; set; }

        [Required]
        public string FromUserId { get; set; } = string.Empty;
        [ForeignKey("FromUserId")]
        public ApplicationUser? FromUser { get; set; }

        [Required]
        public string ToUserId { get; set; } = string.Empty;
        [ForeignKey("ToUserId")]
        public ApplicationUser? ToUser { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime? SentDate { get; set; }
        public bool IsDraft { get; set; }
        public bool IsRead { get; set; }
    }
} 