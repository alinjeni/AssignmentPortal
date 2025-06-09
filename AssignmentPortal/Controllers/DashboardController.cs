using System.Security.Claims;
using AssignmentPortal.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentPortal.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _dashboardRepo;
        public DashboardController(IDashboardRepository dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Student"))
            {
                return RedirectToAction("Student");
            }
            else if (User.IsInRole("Faculty"))
            {
                return RedirectToAction("Faculty");
            }
            return Forbid();
        }

        [Authorize(Roles ="Student")]
        public async Task<IActionResult> Student()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var model = await _dashboardRepo.GetStudentDashboardDataAsync(userId);
            return View("Student", model);
        }

        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> Faculty()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var model = await _dashboardRepo.GetFacultyDashboardDataAsync(userId);
            return View("Faculty", model);
        }
    }
}
