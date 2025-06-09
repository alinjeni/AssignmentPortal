using System.Data;
using AssignmentPortal.Models;
using AssignmentPortal.Repository.Interfaces;
using Dapper;

namespace AssignmentPortal.Repository.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IDbConnection _db;

        public DashboardRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<FacultyDashboardViewModel> GetFacultyDashboardDataAsync(int facultyId)
        {
            var classrooms = await _db.QueryAsync<ClassroomWithAssignmentCount>(
                @"SELECT c.Id, c.Name, COUNT(a.Id) AS AssignmentCount
                  FROM Classrooms c
                  LEFT JOIN Assignments a ON c.Id = a.ClassroomId
                  WHERE c.CreatedById = @FacultyId
                  GROUP BY c.Id, c.Name",
                new { FacultyId = facultyId });

            var assignments = await _db.QueryAsync<AssignmentWithAvgGrade>(
                @"SELECT a.Id, a.Title, AVG(CAST(s.TotalGrade AS FLOAT)) AS AverageGrade
                  FROM Assignments a
                  LEFT JOIN Submissions s ON a.Id = s.AssignmentId
                  WHERE a.ClassroomId IN (SELECT Id FROM Classrooms WHERE CreatedById = @FacultyId)
                  GROUP BY a.Id, a.Title",
                new { FacultyId = facultyId });

            var submissionsPerStudent = await _db.QueryAsync<StudentSubmissionCount>(
                @"SELECT u.Name AS StudentName, COUNT(s.Id) AS SubmissionCount
                  FROM Users u
                  JOIN ClassroomMembers cm ON u.Id = cm.StudentId
                  JOIN Classrooms c ON c.Id = cm.ClassroomId
                  LEFT JOIN Submissions s ON s.UserId = u.Id AND s.AssignmentId IN (
                      SELECT Id FROM Assignments WHERE ClassroomId = c.Id
                  )
                  WHERE c.CreatedById = @FacultyId
                  GROUP BY u.Name",
                new { FacultyId = facultyId });

            var viewModel = new FacultyDashboardViewModel
            {
                ClassroomNames = classrooms.Select(c => c.Name).ToList(),
                AssignmentsPerClassroom = classrooms.Select(c => c.AssignmentCount).ToList(),

                AssignmentTitles = assignments.Select(a => a.Title).ToList(),
                AverageGrades = assignments.Select(a => Math.Round(a.AverageGrade ?? 0, 2)).ToList(),

                StudentNames = submissionsPerStudent.Select(s => s.StudentName).ToList(),
                SubmissionsPerStudent = submissionsPerStudent.Select(s => s.SubmissionCount).ToList()
            };

            return viewModel;
        }

        public async Task<StudentDashboardViewModel> GetStudentDashboardDataAsync(int studentId)
        {
            var submissions = await _db.QueryAsync<SubmissionWithAssignment>(
                @"SELECT s.*, a.Title 
                  FROM Submissions s 
                  JOIN Assignments a ON s.AssignmentId = a.Id 
                  WHERE s.UserId = @StudentId",
                new { StudentId = studentId });

            var allAssignments = await _db.QueryAsync<int>(
                @"SELECT a.Id 
                  FROM Assignments a
                  JOIN ClassroomMembers cs ON a.ClassroomId = cs.ClassroomId
                  WHERE cs.StudentId = @StudentId",
                new { StudentId = studentId });

            var submittedIds = submissions.Select(s => s.AssignmentId).ToHashSet();

            var progress = submissions
                .OrderBy(s => s.SubmittedAt)
                .Select(s => new
                {
                    Date = s.SubmittedAt!.ToString("yyyy-MM-dd"),
                    Grade = Math.Round(s.TotalGrade ?? 0, 2)
                }).ToList();

           
            var performanceBuckets = new int[4]; 
            foreach (var grade in submissions.Select(s => s.TotalGrade ?? 0))
            {
                if (grade >= 90) performanceBuckets[0]++;
                else if (grade >= 75) performanceBuckets[1]++;
                else if (grade >= 60) performanceBuckets[2]++;
                else performanceBuckets[3]++;
            }

            var viewModel = new StudentDashboardViewModel
            {
                AssignmentTitles = submissions.Select(s => s.Title).ToList(),
                AssignmentGrades = submissions.Select(s => Math.Round(s.TotalGrade ?? 0, 2)).ToList(),
                SubmittedCount = submissions.Count(),
                PendingCount = allAssignments.Count(aid => !submittedIds.Contains(aid)),

                ProgressDates = progress.Select(p => p.Date).ToList(),
                ProgressScores = progress.Select(p => p.Grade).ToList(),
                PerformanceBuckets = performanceBuckets.ToList()
            };

            return viewModel;
        }
    }
}
