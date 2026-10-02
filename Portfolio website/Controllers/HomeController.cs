using Microsoft.AspNetCore.Mvc;
using PortfolioWebsite.Data;
using PortfolioWebsite.Models.Entities;
using PortfolioWebsite.Models.ViewModels;

namespace PortfolioWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPortfolioRepository _repository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IPortfolioRepository repository, ILogger<HomeController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var profile = await _repository.GetProfileAsync();
            var projects = await _repository.GetAllProjectsAsync();
            var skills = await _repository.GetAllSkillsAsync();
            var experiences = await _repository.GetAllExperiencesAsync();
            var certifications = await _repository.GetAllCertificationsAsync();

            var viewModel = new HomeViewModel
            {
                Profile = profile,
                Projects = projects,
                Skills = skills,
                Experiences = experiences,
                Certifications = certifications,
                SkillCategories = skills.Select(s => s.Category ?? "Other").Distinct().ToList(),
                ProjectCategories = projects.Select(p => p.Category ?? "Other").Distinct().ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index), new { error = "Please fill all required fields correctly." });
            }

            try
            {
                var message = new ContactMessage
                {
                    Name = model.Name,
                    Email = model.Email,
                    Subject = model.Subject,
                    Message = model.Message
                };

                await _repository.AddMessageAsync(message);
                return RedirectToAction(nameof(Index), new { success = "Message sent successfully! I'll get back to you soon." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending contact message");
                return RedirectToAction(nameof(Index), new { error = "An error occurred while sending your message. Please try again." });
            }
        }
    }
}
