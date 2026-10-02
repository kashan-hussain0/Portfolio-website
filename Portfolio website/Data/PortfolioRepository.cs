using Microsoft.EntityFrameworkCore;
using PortfolioWebsite.Models.Entities;

namespace PortfolioWebsite.Data
{
    public interface IPortfolioRepository
    {
        // Profile
        Task<Profile?> GetProfileAsync();
        Task<Profile> UpdateProfileAsync(Profile profile);

        // Projects
        Task<List<Project>> GetAllProjectsAsync();
        Task<List<Project>> GetFeaturedProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int id);
        Task<Project> AddProjectAsync(Project project);
        Task<Project> UpdateProjectAsync(Project project);
        Task<bool> DeleteProjectAsync(int id);

        // Skills
        Task<List<Skill>> GetAllSkillsAsync();
        Task<Skill?> GetSkillByIdAsync(int id);
        Task<Skill> AddSkillAsync(Skill skill);
        Task<Skill> UpdateSkillAsync(Skill skill);
        Task<bool> DeleteSkillAsync(int id);

        // Contact Messages
        Task<List<ContactMessage>> GetAllMessagesAsync();
        Task<List<ContactMessage>> GetUnreadMessagesAsync();
        Task<ContactMessage?> GetMessageByIdAsync(int id);
        Task<ContactMessage> AddMessageAsync(ContactMessage message);
        Task<bool> MarkMessageAsReadAsync(int id);
        Task<bool> DeleteMessageAsync(int id);

        // Experience
        Task<List<Experience>> GetAllExperiencesAsync();
        Task<Experience?> GetExperienceByIdAsync(int id);
        Task<Experience> AddExperienceAsync(Experience experience);
        Task<Experience> UpdateExperienceAsync(Experience experience);
        Task<bool> DeleteExperienceAsync(int id);

        // Certifications
        Task<List<Certification>> GetAllCertificationsAsync();
        Task<Certification?> GetCertificationByIdAsync(int id);
        Task<Certification> AddCertificationAsync(Certification certification);
        Task<Certification> UpdateCertificationAsync(Certification certification);
        Task<bool> DeleteCertificationAsync(int id);

        // Admin
        Task<AdminUser?> GetAdminByUsernameAsync(string username);
        Task UpdateAdminLastLoginAsync(int adminId);
    }

    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDbContext _context;

        public PortfolioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Profile
        public async Task<Profile?> GetProfileAsync()
        {
            return await _context.Profiles.FirstOrDefaultAsync();
        }

        public async Task<Profile> UpdateProfileAsync(Profile profile)
        {
            var existing = await _context.Profiles.FirstOrDefaultAsync();
            if (existing == null)
            {
                profile.CreatedAt = DateTime.UtcNow;
                _context.Profiles.Add(profile);
            }
            else
            {
                _context.Entry(existing).CurrentValues.SetValues(profile);
                existing.UpdatedAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
            return profile;
        }

        // Projects
        public async Task<List<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects.OrderBy(p => p.DisplayOrder).ToListAsync();
        }

        public async Task<List<Project>> GetFeaturedProjectsAsync()
        {
            return await _context.Projects.Where(p => p.IsFeatured).OrderBy(p => p.DisplayOrder).ToListAsync();
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task<Project> AddProjectAsync(Project project)
        {
            project.CreatedAt = DateTime.UtcNow;
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<Project> UpdateProjectAsync(Project project)
        {
            var existing = await _context.Projects.FindAsync(project.Id);
            if (existing == null) throw new KeyNotFoundException("Project not found");

            _context.Entry(existing).CurrentValues.SetValues(project);
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        // Skills
        public async Task<List<Skill>> GetAllSkillsAsync()
        {
            return await _context.Skills.OrderBy(s => s.DisplayOrder).ToListAsync();
        }

        public async Task<Skill?> GetSkillByIdAsync(int id)
        {
            return await _context.Skills.FindAsync(id);
        }

        public async Task<Skill> AddSkillAsync(Skill skill)
        {
            skill.CreatedAt = DateTime.UtcNow;
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return skill;
        }

        public async Task<Skill> UpdateSkillAsync(Skill skill)
        {
            var existing = await _context.Skills.FindAsync(skill.Id);
            if (existing == null) throw new KeyNotFoundException("Skill not found");

            _context.Entry(existing).CurrentValues.SetValues(skill);
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteSkillAsync(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return false;

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            return true;
        }

        // Contact Messages
        public async Task<List<ContactMessage>> GetAllMessagesAsync()
        {
            return await _context.ContactMessages.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<List<ContactMessage>> GetUnreadMessagesAsync()
        {
            return await _context.ContactMessages.Where(c => !c.IsRead).OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<ContactMessage?> GetMessageByIdAsync(int id)
        {
            return await _context.ContactMessages.FindAsync(id);
        }

        public async Task<ContactMessage> AddMessageAsync(ContactMessage message)
        {
            message.CreatedAt = DateTime.UtcNow;
            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<bool> MarkMessageAsReadAsync(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null) return false;

            message.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null) return false;

            _context.ContactMessages.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }

        // Experience
        public async Task<List<Experience>> GetAllExperiencesAsync()
        {
            return await _context.Experiences.OrderByDescending(e => e.StartDate).ToListAsync();
        }

        public async Task<Experience?> GetExperienceByIdAsync(int id)
        {
            return await _context.Experiences.FindAsync(id);
        }

        public async Task<Experience> AddExperienceAsync(Experience experience)
        {
            experience.CreatedAt = DateTime.UtcNow;
            _context.Experiences.Add(experience);
            await _context.SaveChangesAsync();
            return experience;
        }

        public async Task<Experience> UpdateExperienceAsync(Experience experience)
        {
            var existing = await _context.Experiences.FindAsync(experience.Id);
            if (existing == null) throw new KeyNotFoundException("Experience not found");

            _context.Entry(existing).CurrentValues.SetValues(experience);
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteExperienceAsync(int id)
        {
            var experience = await _context.Experiences.FindAsync(id);
            if (experience == null) return false;

            _context.Experiences.Remove(experience);
            await _context.SaveChangesAsync();
            return true;
        }

        // Certifications
        public async Task<List<Certification>> GetAllCertificationsAsync()
        {
            return await _context.Certifications.OrderBy(c => c.DisplayOrder).ToListAsync();
        }

        public async Task<Certification?> GetCertificationByIdAsync(int id)
        {
            return await _context.Certifications.FindAsync(id);
        }

        public async Task<Certification> AddCertificationAsync(Certification certification)
        {
            certification.CreatedAt = DateTime.UtcNow;
            _context.Certifications.Add(certification);
            await _context.SaveChangesAsync();
            return certification;
        }

        public async Task<Certification> UpdateCertificationAsync(Certification certification)
        {
            var existing = await _context.Certifications.FindAsync(certification.Id);
            if (existing == null) throw new KeyNotFoundException("Certification not found");

            _context.Entry(existing).CurrentValues.SetValues(certification);
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteCertificationAsync(int id)
        {
            var certification = await _context.Certifications.FindAsync(id);
            if (certification == null) return false;

            _context.Certifications.Remove(certification);
            await _context.SaveChangesAsync();
            return true;
        }

        // Admin
        public async Task<AdminUser?> GetAdminByUsernameAsync(string username)
        {
            return await _context.AdminUsers.FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task UpdateAdminLastLoginAsync(int adminId)
        {
            var admin = await _context.AdminUsers.FindAsync(adminId);
            if (admin != null)
            {
                admin.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
