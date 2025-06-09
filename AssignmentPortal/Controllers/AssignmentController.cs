using AssignmentPortal.Models;
using AssignmentPortal.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using AssignmentPortal.Repository.Implementations;

namespace AssignmentPortal.Controllers
{
    [Authorize]
    public class AssignmentController : Controller
    {
        private readonly IAssignmentRepository _assignmentRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly ISubmissionRepository _submissionRepo;
        private readonly IUserRepository _userRepo;

        public AssignmentController(IAssignmentRepository assignmentRepo, IWebHostEnvironment environment, ISubmissionRepository submissionRepo, IUserRepository userRepo)
        {
            _assignmentRepo = assignmentRepo;
            _environment = environment;
            _submissionRepo = submissionRepo;
            _userRepo = userRepo;
        }

        [Authorize(Roles = "Faculty")]
        public IActionResult Create(int classroomId)
        {
            ViewBag.ClassroomId = classroomId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> Create(Assignment assignment, List<IFormFile>? files)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Error in Creating.";
                return View(assignment);
            }
            assignment.CreatedById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            assignment.CreatedAt = DateTime.UtcNow;

            var assignmentId = await _assignmentRepo.CreateAssignmentAsync(assignment);
            if (files != null)
                await UploadAssignmentFileAsync(assignmentId, files);
            TempData["success"] = "Assignment is created successfully.";
            return RedirectToAction("List", new { classroomId = assignment.ClassroomId });
        }

        [Authorize(Roles = "Faculty")]
        public IActionResult Edit(int id)
        {
            var assignment = _assignmentRepo.GetAssignmentByIdAsync(id).Result;
            if (assignment == null)
            {
                return NotFound();
            }
            if (assignment.CreatedById != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
            {
                return Forbid();
            }
            return View(assignment);
        }

        [Authorize(Roles = "Faculty")]
        [HttpPost]
        public async Task<IActionResult> Edit(Assignment assignment, List<IFormFile>? files)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Error in Updating.";
                return View(assignment);
            }

            assignment.UpdatedAt = DateTime.UtcNow;
            await _assignmentRepo.UpdateAssignmentAsync(assignment);
            if (files != null)
                await UploadAssignmentFileAsync(assignment.Id, files);
            TempData["success"] = "Assignment is updated successfully.";
            return RedirectToAction("List", new { classroomId = assignment.ClassroomId });
        }

        [Authorize]
        public async Task<IActionResult> List(int classroomId, string filter = "active")
        {
            bool includeArchived = filter.ToLower() == "archived";
            var assignments = await _assignmentRepo.GetAssignmentsByClassroomAsync(classroomId, includeArchived);
            ViewBag.ClassroomId = classroomId;
            ViewBag.Filter = filter;
            return View(assignments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;
            var assignment = await _assignmentRepo.GetAssignmentByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            var submission = await _submissionRepo.GetSubmissionByUserAndAssignmentAsync(userId, id);

            var files = await _assignmentRepo.GetFilesByAssignmentIdAsync(id);
            var comments = await _assignmentRepo.GetCommentsByAssignmentIdAsync(id);

            var viewModel = new AssignmentDetailsViewModel
            {
                Assignment = assignment,
                Files = files,
                Comments = comments,
                HasSubmitted = submission != null,
                SubmissionId = submission?.Id,
                UserRole = userRole
            };

            return View(viewModel);
        }

        private async Task UploadAssignmentFileAsync(int assignmentId, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return;

            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var filePath = Path.Combine(uploads, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var assignmentFile = new AssignmentFile
                    {
                        AssignmentId = assignmentId,
                        FileName = file.FileName,
                        FilePath = "/uploads/" + file.FileName,
                        FileSize = file.Length,
                        MimeType = file.ContentType,
                        UploadedById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
                    };

                    await _assignmentRepo.AddFileAsync(assignmentFile);
                    TempData["success"] ="Files uploaded successfully.";
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int assignmentId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction("Details", new { id = assignmentId });
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _userRepo.GetByIdAsync(userId);
            var comment = new AssignmentComment
            {
                AssignmentId = assignmentId,
                Content = content,
                UserId = userId,
                UserName = user!.Name
            };

            await _assignmentRepo.AddCommentAsync(comment);
            TempData["success"] = "Comment Added successfully.";
            return RedirectToAction("Details", new { id = assignmentId });
        }
        
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> Archive(int id)
        {
            var assignment = await _assignmentRepo.GetAssignmentByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            if (assignment.CreatedById != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
            {
                return Forbid();
            }
            var result = await _assignmentRepo.ArchiveAssignmentAsync(id);
            if (result)
            {
                TempData["success"] = "Assignment Archived.";
                return RedirectToAction("List", new { classroomId = assignment.ClassroomId });
            }
            else
            {
                return BadRequest("Failed to Archive the assignment.");
            }
        }

        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> UnArchive(int id)
        {
            var assignment = await _assignmentRepo.GetAssignmentByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            if (assignment.CreatedById != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
            {
                return Forbid();
            }
            var result = await _assignmentRepo.UnArchiveAssignmentAsync(id);
            if (result)
            {
                TempData["success"] = "Assignment Un-Archived.";
                return RedirectToAction("List", new { classroomId = assignment.ClassroomId });
            }
            else
            {
                return BadRequest("Failed to Activate the assignment.");
            }
        }
        [HttpPost]
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> DeleteFile(int fileId, int assignmentId)
        {
            var file = await _assignmentRepo.GetFileByIdAsync(fileId);
            if (file == null || file.AssignmentId != assignmentId)
                return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var assignment = await _assignmentRepo.GetAssignmentByIdAsync(assignmentId);
            if (assignment == null || assignment.CreatedById != userId)
                return Forbid();

            var filePath = Path.Combine(_environment.WebRootPath, file.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            await _assignmentRepo.DeleteAssignmentFileAsync(fileId);
            TempData["success"] = "File Deleted.";

            return RedirectToAction("Details", new { id = assignmentId });
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentAssignments()
        {
            int studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var model = await _assignmentRepo.GetAssignmentsForStudentAsync(studentId);
            return View("StudentAssignments", model);
        }
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> FacultyAssignments()
        {
            int facultyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var model = await _assignmentRepo.GetAssignmentsForFacultyAsync(facultyId);
            return View("FacultyAssignments", model);
        }
    }
}
