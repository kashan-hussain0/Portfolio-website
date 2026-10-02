using PortfolioWebsite.Models.Entities;

namespace PortfolioWebsite.Models.ViewModels
{
    public class HomeViewModel
    {
        public Profile? Profile { get; set; }
        public List<Project> Projects { get; set; } = new();
        public List<Skill> Skills { get; set; } = new();
        public List<Experience> Experiences { get; set; } = new();
        public List<Certification> Certifications { get; set; } = new();
        public List<string> SkillCategories { get; set; } = new();
        public List<string> ProjectCategories { get; set; } = new();
    }
}
