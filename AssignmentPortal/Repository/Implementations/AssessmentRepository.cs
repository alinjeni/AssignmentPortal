using System.Data;
using AssignmentPortal.Models;
using AssignmentPortal.Repository.Interfaces;
using Dapper;

namespace AssignmentPortal.Repository.Implementations
{
    public class AssessmentRepository : IAssessmentRepository
    {
        private readonly IDbConnection _db;

        public AssessmentRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<AssessmentCriterion>> GetAllCriteriaAsync()
        {
            var sql = "SELECT * FROM AssessmentCriteria";
            return await _db.QueryAsync<AssessmentCriterion>(sql);
        }

        public async Task<int> CreateAssessmentAsync(Assessment assessment)
        {
            var sql = @"
            INSERT INTO Assessments (SubmissionId, FacultyId, AssessedAt)
            VALUES (@SubmissionId, @FacultyId, @AssessedAt);
            SELECT CAST(SCOPE_IDENTITY() as int)";
            return await _db.ExecuteScalarAsync<int>(sql, assessment);
        }

        public async Task<bool> AddEvaluatedCriteriaAsync(List<AssessedCriterion> assessedCriteria)
        {
            var sql = @"
            INSERT INTO AssessedCriteria (AssessmentId, CriterionId, Score, Remarks)
            VALUES (@AssessmentId, @CriterionId, @Score, @Remarks)";
            var result = await _db.ExecuteAsync(sql, assessedCriteria);
            return result > 0;
        }

        public async Task<Assessment?> GetAssessmentBySubmissionIdAsync(int submissionId)
        {
            var sql = "SELECT * FROM Assessments WHERE SubmissionId = @SubmissionId";
            return await _db.QueryFirstOrDefaultAsync<Assessment>(sql, new { SubmissionId = submissionId });
        }

        public async Task<IEnumerable<AssessedCriterion>> GetEvaluatedCriteriaBySubmissionIdAsync(int submissionId)
        {
            var sql = @"
            SELECT ac.Id, ac.AssessmentId, ac.CriterionId, ac.Score, ac.Remarks,
                   c.CriterionName, c.Description, c.MaxScore
            FROM AssessedCriteria ac
            INNER JOIN Assessments a ON ac.AssessmentId = a.Id
            INNER JOIN AssessmentCriteria c ON ac.CriterionId = c.Id
            WHERE a.SubmissionId = @SubmissionId";
            return await _db.QueryAsync<AssessedCriterion>(sql, new { SubmissionId = submissionId });
        }

        public async Task<Assessment?> GetBySubmissionIdAsync(int submissionId)
        {
            var sql = "SELECT * FROM Assessments WHERE SubmissionId = @SubmissionId";
            return await _db.QueryFirstOrDefaultAsync<Assessment>(sql, new { SubmissionId = submissionId });
        }

        public async Task<List<AssessmentResultViewModel>> GetCriteriaResultsAsync(int assessmentId)
        {
            var sql = @"
        SELECT 
            fc.CriterionName,
            fc.Description,
            fc.MaxScore,
            ac.Score,
            ac.Remarks
        FROM AssessedCriteria ac
        INNER JOIN AssessmentCriteria fc ON ac.CriterionId = fc.Id
        WHERE ac.AssessmentId = @AssessmentId";

            return (await _db.QueryAsync<AssessmentResultViewModel>(sql, new { AssessmentId = assessmentId })).ToList();
        }
    }
}
