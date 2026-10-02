using System.ComponentModel.DataAnnotations;

namespace PortfolioWebsite.Models.Entities
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public string? LiveDemoUrl { get; set; }

        public string? GitHubUrl { get; set; }

        public string? Category { get; set; }

        public string? Technologies { get; set; }

        public bool IsFeatured { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
