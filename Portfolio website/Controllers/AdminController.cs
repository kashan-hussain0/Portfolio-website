using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioWebsite.Data;
using PortfolioWebsite.Models.Entities;
using PortfolioWebsite.Models.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PortfolioWebsite.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly IPortfolioRepository _repository;
        private readonly ILogger<AdminController> _logger;
        private readonly IWebHostEnvironment _environment;

        public AdminController(IPortfolioRepository repository, ILogger<AdminController> logger, IWebHostEnvironment environment)
        {
            _repository = repository;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet("Login")]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction(nameof(Dashboard));
            return View(new AdminLoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var admin = await _repository.GetAdminByUsernameAsync(model.Username);
            if (admin == null || !VerifyPassword(model.Password, admin.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            await _repository.UpdateAdminLastLoginAsync(admin.Id);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(8)
                });

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var projects = await _repository.GetAllProjectsAsync();
            var skills = await _repository.GetAllSkillsAsync();
            var messages = await _repository.GetAllMessagesAsync();
            var experiences = await _repository.GetAllExperiencesAsync();
            var certifications = await _repository.GetAllCertificationsAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalProjects = projects.Count,
                TotalSkills = skills.Count,
                TotalMessages = messages.Count,
                UnreadMessages = messages.Count(m => !m.IsRead),
                TotalExperiences = experiences.Count,
                RecentMessages = messages.Take(5).ToList()
            };

            return View(viewModel);
        }

        [HttpGet("Profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var profile = await _repository.GetProfileAsync() ?? new Profile();
            return View(profile);
        }

        [HttpPost("Profile")]
        [Authorize]
        public async Task<IActionResult> Profile(Profile model, IFormFile? profileImage)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (profileImage != null && profileImage.Length > 0)
                {
                    var imageUrl = await SaveImageAsync(profileImage, "profile");
                    if (!string.IsNullOrEmpty(imageUrl))
                        model.ProfileImageUrl = imageUrl;
                }

                await _repository.UpdateProfileAsync(model);
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the profile.");
                return View(model);
            }
        }

        [HttpGet("Projects")]
        [Authorize]
        public async Task<IActionResult> Projects()
        {
            var projects = await _repository.GetAllProjectsAsync();
            return View(projects);
        }

        [HttpGet("Projects/Create")]
        [Authorize]
        public IActionResult CreateProject()
        {
            return View(new Project());
        }

        [HttpPost("Projects/Create")]
        [Authorize]
        public async Task<IActionResult> CreateProject(Project model, IFormFile? projectImage)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (projectImage != null && projectImage.Length > 0)
                {
                    var imageUrl = await SaveImageAsync(projectImage, "projects");
                    if (!string.IsNullOrEmpty(imageUrl))
                        model.ImageUrl = imageUrl;
                }

                await _repository.AddProjectAsync(model);
                TempData["Success"] = "Project created successfully!";
                return RedirectToAction(nameof(Projects));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the project.");
                return View(model);
            }
        }

        [HttpGet("Projects/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditProject(int id)
        {
            var project = await _repository.GetProjectByIdAsync(id);
            if (project == null)
                return NotFound();
            return View(project);
        }

        [HttpPost("Projects/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditProject(int id, Project model, IFormFile? projectImage)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (projectImage != null && projectImage.Length > 0)
                {
                    var imageUrl = await SaveImageAsync(projectImage, "projects");
                    if (!string.IsNullOrEmpty(imageUrl))
                        model.ImageUrl = imageUrl;
                }

                await _repository.UpdateProjectAsync(model);
                TempData["Success"] = "Project updated successfully!";
                return RedirectToAction(nameof(Projects));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the project.");
                return View(model);
            }
        }

        [HttpPost("Projects/Delete/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                var result = await _repository.DeleteProjectAsync(id);
                if (!result)
                    return NotFound();

                TempData["Success"] = "Project deleted successfully!";
                return RedirectToAction(nameof(Projects));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project");
                TempData["Error"] = "An error occurred while deleting the project.";
                return RedirectToAction(nameof(Projects));
            }
        }

        [HttpGet("Skills")]
        [Authorize]
        public async Task<IActionResult> Skills()
        {
            var skills = await _repository.GetAllSkillsAsync();
            return View(skills);
        }

        [HttpGet("Skills/Create")]
        [Authorize]
        public IActionResult CreateSkill()
        {
            return View(new Skill());
        }

        [HttpPost("Skills/Create")]
        [Authorize]
        public async Task<IActionResult> CreateSkill(Skill model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _repository.AddSkillAsync(model);
                TempData["Success"] = "Skill created successfully!";
                return RedirectToAction(nameof(Skills));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating skill");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the skill.");
                return View(model);
            }
        }

        [HttpGet("Skills/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditSkill(int id)
        {
            var skill = await _repository.GetSkillByIdAsync(id);
            if (skill == null)
                return NotFound();
            return View(skill);
        }

        [HttpPost("Skills/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditSkill(int id, Skill model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _repository.UpdateSkillAsync(model);
                TempData["Success"] = "Skill updated successfully!";
                return RedirectToAction(nameof(Skills));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating skill");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the skill.");
                return View(model);
            }
        }

        [HttpPost("Skills/Delete/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            try
            {
                var result = await _repository.DeleteSkillAsync(id);
                if (!result)
                    return NotFound();

                TempData["Success"] = "Skill deleted successfully!";
                return RedirectToAction(nameof(Skills));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting skill");
                TempData["Error"] = "An error occurred while deleting the skill.";
                return RedirectToAction(nameof(Skills));
            }
        }

        [HttpGet("Experience")]
        [Authorize]
        public async Task<IActionResult> Experience()
        {
            var experiences = await _repository.GetAllExperiencesAsync();
            return View(experiences);
        }

        [HttpGet("Experience/Create")]
        [Authorize]
        public IActionResult CreateExperience()
        {
            return View(new Experience());
        }

        [HttpPost("Experience/Create")]
        [Authorize]
        public async Task<IActionResult> CreateExperience(Experience model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _repository.AddExperienceAsync(model);
                TempData["Success"] = "Experience created successfully!";
                return RedirectToAction(nameof(Experience));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating experience");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the experience.");
                return View(model);
            }
        }

        [HttpGet("Experience/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditExperience(int id)
        {
            var experience = await _repository.GetExperienceByIdAsync(id);
            if (experience == null)
                return NotFound();
            return View(experience);
        }

        [HttpPost("Experience/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditExperience(int id, Experience model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _repository.UpdateExperienceAsync(model);
                TempData["Success"] = "Experience updated successfully!";
                return RedirectToAction(nameof(Experience));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating experience");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the experience.");
                return View(model);
            }
        }

        [HttpPost("Experience/Delete/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            try
            {
                var result = await _repository.DeleteExperienceAsync(id);
                if (!result)
                    return NotFound();

                TempData["Success"] = "Experience deleted successfully!";
                return RedirectToAction(nameof(Experience));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting experience");
                TempData["Error"] = "An error occurred while deleting the experience.";
                return RedirectToAction(nameof(Experience));
            }
        }

        [HttpGet("Certifications")]
        [Authorize]
        public async Task<IActionResult> Certifications()
        {
            var certifications = await _repository.GetAllCertificationsAsync();
            return View(certifications);
        }

        [HttpGet("Certifications/Create")]
        [Authorize]
        public IActionResult CreateCertification()
        {
            return View(new Certification());
        }

        [HttpPost("Certifications/Create")]
        [Authorize]
        public async Task<IActionResult> CreateCertification(Certification model, IFormFile? certImage)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (certImage != null && certImage.Length > 0)
                {
                    var imageUrl = await SaveImageAsync(certImage, "certifications");
                    if (!string.IsNullOrEmpty(imageUrl))
                        model.ImageUrl = imageUrl;
                }

                await _repository.AddCertificationAsync(model);
                TempData["Success"] = "Certification created successfully!";
                return RedirectToAction(nameof(Certifications));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating certification");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the certification.");
                return View(model);
            }
        }

        [HttpGet("Certifications/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditCertification(int id)
        {
            var certification = await _repository.GetCertificationByIdAsync(id);
            if (certification == null)
                return NotFound();
            return View(certification);
        }

        [HttpPost("Certifications/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> EditCertification(int id, Certification model, IFormFile? certImage)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (certImage != null && certImage.Length > 0)
                {
                    var imageUrl = await SaveImageAsync(certImage, "certifications");
                    if (!string.IsNullOrEmpty(imageUrl))
                        model.ImageUrl = imageUrl;
                }

                await _repository.UpdateCertificationAsync(model);
                TempData["Success"] = "Certification updated successfully!";
                return RedirectToAction(nameof(Certifications));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating certification");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the certification.");
                return View(model);
            }
        }

        [HttpPost("Certifications/Delete/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCertification(int id)
        {
            try
            {
                var result = await _repository.DeleteCertificationAsync(id);
                if (!result)
                    return NotFound();

                TempData["Success"] = "Certification deleted successfully!";
                return RedirectToAction(nameof(Certifications));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting certification");
                TempData["Error"] = "An error occurred while deleting the certification.";
                return RedirectToAction(nameof(Certifications));
            }
        }

        [HttpGet("Messages")]
        [Authorize]
        public async Task<IActionResult> Messages()
        {
            var messages = await _repository.GetAllMessagesAsync();
            return View(messages);
        }

        [HttpGet("Messages/Details/{id}")]
        [Authorize]
        public async Task<IActionResult> MessageDetails(int id)
        {
            var message = await _repository.GetMessageByIdAsync(id);
            if (message == null)
                return NotFound();

            if (!message.IsRead)
                await _repository.MarkMessageAsReadAsync(id);

            return View(message);
        }

        [HttpPost("Messages/Delete/{id}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            try
            {
                var result = await _repository.DeleteMessageAsync(id);
                if (!result)
                    return NotFound();

                TempData["Success"] = "Message deleted successfully!";
                return RedirectToAction(nameof(Messages));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message");
                TempData["Error"] = "An error occurred while deleting the message.";
                return RedirectToAction(nameof(Messages));
            }
        }

        private async Task<string?> SaveImageAsync(IFormFile image, string folder)
        {
            try
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                return $"/uploads/{folder}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving image");
                return null;
            }
        }

        private static bool VerifyPassword(string password, string hash)
        {
            var computedHash = HashPassword(password);
            return computedHash == hash;
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
