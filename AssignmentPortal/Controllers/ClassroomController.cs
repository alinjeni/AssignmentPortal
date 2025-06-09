using System.Security.Claims;
using AssignmentPortal.Models;
using AssignmentPortal.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AssignmentPortal.Controllers
{
    [Authorize]
    public class ClassroomController : Controller
    {
        private readonly IClassroomRepository _classroomRepository;
        private readonly IUserRepository _userRepository;

        public ClassroomController(IClassroomRepository classroomRepository, IUserRepository userRepository)
        {
            _classroomRepository = classroomRepository;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index(bool showArchived = false)
        {
            ViewBag.ShowArchived = showArchived;
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            IEnumerable<Classroom> classrooms;

            if (User.IsInRole("Faculty"))
            {
                classrooms = await _classroomRepository.GetAllClassroomsByFacultyAsync(userId, showArchived);
            }
            else
            {
                classrooms = await _classroomRepository.GetClassroomsForStudentAsync(userId, showArchived);
            }

            return View(classrooms);
        }


        [Authorize(Roles = "Faculty")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Faculty")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Classroom classroom)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Error in creating.";
                return View(classroom);
            }

            classroom.CreatedAt = DateTime.UtcNow;
            classroom.CreatedById = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _classroomRepository.CreateClassroomAsync(classroom);
            TempData["success"] = "Classsroom is created successfullly.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> Edit(int id)
        {
            var classroom = await _classroomRepository.GetClassroomByIdAsync(id);
            if (classroom == null)
                return NotFound();

            return View(classroom);
        }

        [Authorize(Roles = "Faculty")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Classroom classroom)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Error in updating.";
                return View(classroom);
            }               

            await _classroomRepository.UpdateClassroomAsync(classroom);
            TempData["success"] = "Classroom is updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> Archive(int id)
        {
            await _classroomRepository.ArchiveClassroomAsync(id);
            TempData["success"] = "Archived.";
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> UnArchive(int id)
        {
            await _classroomRepository.UnArchiveClassroomAsync(id);
            TempData["success"] = "UnArchived.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> ManageStudents(int id)
        {
            var classroom = await _classroomRepository.GetClassroomByIdAsync(id);
            if (classroom == null) return NotFound();

            var studentsInClass = await _classroomRepository.GetStudentsInClassroomAsync(id);
            var allStudents = (await _userRepository.GetUsersByRoleAsync("Student")).ToList();
            var availableStudents = allStudents.Where(s => !studentsInClass.Any(cs => cs.Id == s.Id)).ToList();

            var viewModel = new ManageClassroomStudentsViewModel
            {
                Classroom = classroom,
                AvailableStudents = availableStudents,
                CurrentStudents = studentsInClass.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> AddStudent(int classroomId, int studentId)
        {
            await _classroomRepository.AddStudentToClassroomAsync(classroomId, studentId);
            return RedirectToAction("ManageStudents", new { id = classroomId });
        }

        [HttpPost]
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> RemoveStudent(int classroomId, int studentId)
        {
            await _classroomRepository.RemoveStudentFromClassroomAsync(classroomId, studentId);
            return RedirectToAction("ManageStudents", new { id = classroomId });
        }
    }
}
