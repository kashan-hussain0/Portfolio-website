using System.ComponentModel.DataAnnotations;

namespace PortfolioWebsite.Models.Entities
{
    public class Profile
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Tagline { get; set; }

        public string? Bio { get; set; }

        public string? ProfileImageUrl { get; set; }

        public string? ResumeUrl { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Location { get; set; }

        public string? GitHubUrl { get; set; }

        public string? LinkedInUrl { get; set; }

        public string? TwitterUrl { get; set; }

        public string? InstagramUrl { get; set; }

        public string? FacebookUrl { get; set; }

        public string? YouTubeUrl { get; set; }

        public string? DribbbleUrl { get; set; }

        public string? BehanceUrl { get; set; }

        public string? PersonalWebsite { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
