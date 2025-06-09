using AssignmentPortal.Models;
using AssignmentPortal.Repository.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentPortal.Controllers
{
    [Authorize]
    public class SubmissionController : Controller
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IAssessmentRepository _assessmentRepository;
        private readonly IAssignmentRepository _assignmentRepository;

        public SubmissionController(ISubmissionRepository submissionRepository, 
                                    IWebHostEnvironment environment, IAssessmentRepository assessmentRepository, 
                                    IAssignmentRepository assignmentRepository)
        {
            _submissionRepository = submissionRepository;
            _environment = environment;
            _assessmentRepository = assessmentRepository;
            _assignmentRepository = assignmentRepository;
        }

        public async Task<IActionResult> Index(int assignmentId)
        {
            var submissions = await _submissionRepository.GetSubmissionsByAssignmentIdAsync(assignmentId);
            var assignment = await _assignmentRepository.GetAssignmentByIdAsync(assignmentId);
            ViewBag.ClassroomId = assignment!.ClassroomId;
            return View(submissions);
        }

        [Authorize(Roles = "Student")]
        public IActionResult Create(int assignmentId)
        {
            ViewBag.AssignmentId = assignmentId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Create(Submission submission, List<IFormFile>? files)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Error in creating.";
                return View(submission);
            }

            submission.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            submission.SubmittedAt = DateTime.UtcNow;

            var submissionId = await _submissionRepository.CreateSubmissionAsync(submission);

            if (files != null && files.Count > 0)
            {
                await UploadSubmissionFilesAsync(submissionId, files);
            }
            TempData["success"] = "Assignment submitted successsfully.";
            return RedirectToAction("Details", "Assignment", new { id = submission.AssignmentId });
        }
        private async Task UploadSubmissionFilesAsync(int submissionId, List<IFormFile> files)
        {
            var uploadsPath = Path.Combine(_environment.WebRootPath, "submission_uploads");
            Directory.CreateDirectory(uploadsPath);

            foreach (var file in files)
            {
                var filePath = Path.Combine(uploadsPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var submissionFile = new SubmissionFile
                {
                    SubmissionId = submissionId,
                    FileName = file.FileName,
                    FilePath = "/submission_uploads/" + file.FileName,
                    FileSize = file.Length,
                    MimeType = file.ContentType,
                    UploadedAt = DateTime.UtcNow,
                    UploadedById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
                };

                await _submissionRepository.AddSubmissionFileAsync(submissionFile);
                TempData["success"] = "Files uploaded.";
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var submission = await _submissionRepository.GetSubmissionByIdAsync(id);
            if (submission == null) return NotFound();

            var files = await _submissionRepository.GetFilesBySubmissionIdAsync(id);
            var assessment = await _assessmentRepository.GetBySubmissionIdAsync(id);
            var criteria = assessment != null
                ? await _assessmentRepository.GetCriteriaResultsAsync(assessment.Id)
                : new List<AssessmentResultViewModel>();

            var model = new SubmissionDetailsViewModel
            {
                Submission = submission,
                Files = files.ToList(),
                Assessment = assessment,
                CriteriaResults = criteria
            };

            return View(model);
        }

        [Authorize(Roles ="Student")]
        public async Task<IActionResult> Edit(int id)
        {
            var submission = await _submissionRepository.GetSubmissionByIdAsync(id);
            if (submission == null)
                return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (submission.UserId != userId)
                return Forbid();

            return View(submission);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Edit(Submission submission, List<IFormFile>? files)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Error in updating.";
                return View(submission);
            }

            var existing = await _submissionRepository.GetSubmissionByIdAsync(submission.Id);

            if (existing == null || existing.GradedAt != null)
                return Forbid();

            submission.SubmittedAt = DateTime.UtcNow;
            await _submissionRepository.UpdateSubmissionAsync(submission);

            if (files != null)
                await UploadSubmissionFilesAsync(submission.Id, files);
            TempData["success"] = "Submission updated successfully,";
            return RedirectToAction("Details", "Submission", new { id = submission.Id });
        }

        [Authorize(Roles ="Student")]
        [HttpPost]
        public async Task<IActionResult> DeleteFile(int fileId, int submissionId)
        {
            var file = await _submissionRepository.GetFileByIdAsync(fileId);
            if (file == null || file.SubmissionId != submissionId)
                return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var submission = await _submissionRepository.GetSubmissionByIdAsync(submissionId);
            if (submission == null || submission.UserId != userId || submission.GradedAt != null)
                return Forbid();

            var filePath = Path.Combine(_environment.WebRootPath, file.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            await _submissionRepository.DeleteSubmissionFileAsync(fileId);
            TempData["success"] = "Files deleted succcessfully.";

            return RedirectToAction("Details", new { id = submissionId });
        }

        [HttpPost]
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> DeleteSubmission(int submissionId)
        {
            var submission = await _submissionRepository.GetSubmissionByIdAsync(submissionId);
            if (submission == null)
                return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (submission.UserId != userId || submission.GradedAt != null)
                return Forbid();

            var files = await _submissionRepository.GetFilesBySubmissionIdAsync(submissionId);
            foreach (var file in files)
            {
                var filePath = Path.Combine(_environment.WebRootPath, file.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                await _submissionRepository.DeleteSubmissionFileAsync(file.Id);
            }

            await _submissionRepository.DeleteSubmissionAsync(submissionId);
            TempData["success"] = "Submission deleted successfully.";
            return RedirectToAction("Details", "Assignment", new { id = submission.AssignmentId });
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentSubmissions()
        {
            int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var model = await _submissionRepository.GetSubmissionsForStudentAsync(studentId);
            return View("StudentSubmissions", model);
        }
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> FacultySubmissions()
        {
            int facultyId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var model = await _submissionRepository.GetSubmissionsForFacultyAsync(facultyId);
            return View("FacultySubmissions", model);
        }
    }
}
