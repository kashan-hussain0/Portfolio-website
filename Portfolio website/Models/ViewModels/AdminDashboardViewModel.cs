using PortfolioWebsite.Models.Entities;

namespace PortfolioWebsite.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalProjects { get; set; }
        public int TotalSkills { get; set; }
        public int TotalMessages { get; set; }
        public int UnreadMessages { get; set; }
        public int TotalExperiences { get; set; }
        public List<ContactMessage> RecentMessages { get; set; } = new();
    }
}
