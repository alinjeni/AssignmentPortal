using AssignmentPortal.Models;
using AssignmentPortal.Repository.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AssignmentPortal.Controllers
{
    [Authorize]
    public class AssessmentController : Controller
    {
        private readonly IAssessmentRepository _assessmentRepo;
        private readonly ISubmissionRepository _submissionRepo;

        public AssessmentController(
            IAssessmentRepository assessmentRepo,
            ISubmissionRepository submissionRepo)
        {
            _assessmentRepo = assessmentRepo;
            _submissionRepo = submissionRepo;
        }

        [Authorize(Roles ="Faculty")]
        [HttpGet]
        public async Task<IActionResult> Assess(int submissionId)
        {
            var submission = await _submissionRepo.GetSubmissionByIdAsync(submissionId);
            if (submission == null) return NotFound();

            var criteria = await _assessmentRepo.GetAllCriteriaAsync();
            var viewModel = new AssessSubmissionViewModel
            {
                AssignmentId = submission.AssignmentId,
                SubmissionId = submissionId,
                Criteria = criteria.Select(c => new AssessedCriterion
                {
                    CriterionId = c.Id,
                    CriterionName = c.CriterionName,
                    Description = c.Description,
                    MaxScore = c.MaxScore
                }).ToList()
            };

            return View(viewModel);
        }
        [Authorize(Roles = "Faculty")]
        [HttpPost]
        public async Task<IActionResult> Assess(AssessSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Error in Assessment.";
                return View(model);
            }

            var facultyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var assessment = new Assessment
            {
                SubmissionId = model.SubmissionId,
                FacultyId = facultyId,
                AssessedAt = DateTime.UtcNow
            };

            var assessmentId = await _assessmentRepo.CreateAssessmentAsync(assessment);

            foreach (var c in model.Criteria)
            {
                c.AssessmentId = assessmentId;
            }

            await _assessmentRepo.AddEvaluatedCriteriaAsync(model.Criteria);

            var totalScore = model.Criteria.Sum(c => c.Score);
            var maxTotal = model.Criteria.Sum(c => c.MaxScore);

            var gradePercent = Math.Round((totalScore / maxTotal) * 100, 2);

            var submission = await _submissionRepo.GetSubmissionByIdAsync(model.SubmissionId);
            submission.GradedAt = DateTime.UtcNow;
            submission.TotalGrade = gradePercent;
            submission.Feedback = model.Feedback;

            await _submissionRepo.UpdateSubmissionAsync(submission);
            TempData["success"] = "Submission assessed successfully.";
            return RedirectToAction("Details", "Submission", new { id = model.SubmissionId });
        }

    }
}
