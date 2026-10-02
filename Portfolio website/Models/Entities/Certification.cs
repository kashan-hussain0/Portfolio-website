using System.ComponentModel.DataAnnotations;

namespace PortfolioWebsite.Models.Entities
{
    public class Certification
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Issuer { get; set; }

        public string? CredentialUrl { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime? IssueDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsExpired { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
