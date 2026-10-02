using System.ComponentModel.DataAnnotations;

namespace PortfolioWebsite.Models.Entities
{
    public class Skill
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Category { get; set; }

        public int ProficiencyLevel { get; set; } // 1-100

        public string? IconUrl { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
