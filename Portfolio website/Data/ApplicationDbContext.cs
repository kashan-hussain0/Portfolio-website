using Microsoft.EntityFrameworkCore;
using PortfolioWebsite.Models.Entities;

namespace PortfolioWebsite.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Certification> Certifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Profile>().HasIndex(p => p.FullName);
            modelBuilder.Entity<Project>().HasIndex(p => p.Category);
            modelBuilder.Entity<Project>().HasIndex(p => p.IsFeatured);
            modelBuilder.Entity<Skill>().HasIndex(s => s.Category);
            modelBuilder.Entity<ContactMessage>().HasIndex(c => c.IsRead);
            modelBuilder.Entity<ContactMessage>().HasIndex(c => c.CreatedAt);
            modelBuilder.Entity<AdminUser>().HasIndex(a => a.Username).IsUnique();
            modelBuilder.Entity<Certification>().HasIndex(c => c.Title);
        }
    }
}
