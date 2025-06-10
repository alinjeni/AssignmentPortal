using AssignmentPortal.Models;
using AssignmentPortal.Services;
using Dapper;
using System.Data;

namespace AssignmentPortal.Data
{
    public class DatabaseSeeder
    {
        private readonly IDbConnection _db;

        public DatabaseSeeder(IDbConnection db)
        {
            _db = db;
        }
        public async Task CreateStoredProcedureAsync()
        {
            string createProcSql = @"CREATE OR ALTER PROCEDURE sp_Createtables
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Users Table
                    
                    IF OBJECT_ID('Users', 'U') IS NULL
                    BEGIN
                        CREATE TABLE Users (
                            Id INT IDENTITY PRIMARY KEY,
                            Name NVARCHAR(100) NOT NULL,
                            Email NVARCHAR(100) NOT NULL UNIQUE,
                            PasswordHash NVARCHAR(255) NOT NULL,
                            Role NVARCHAR(20) NOT NULL,
                            CreatedAt DATETIME NOT NULL
                        );
                    END

                     -- Classrooms Table
                    IF OBJECT_ID('Classrooms', 'U') IS NULL
                    BEGIN
                        CREATE TABLE Classrooms (
                            Id INT IDENTITY PRIMARY KEY,
                            Name NVARCHAR(100) NOT NULL,
                            Description NVARCHAR(255),
                            IsArchived BIT DEFAULT 0,
                            CreatedAt DATETIME NOT NULL,
                            CreatedById INT NOT NULL,
                            FOREIGN KEY (CreatedById) REFERENCES Users(Id)
                        );
                    END

                    --ClassroomMembers Table
                    IF OBJECT_ID('ClassroomMembers', 'U') IS NULL
                    BEGIN
                        CREATE TABLE ClassroomMembers (
                            Id INT IDENTITY PRIMARY KEY,
                            ClassroomId INT NOT NULL,
                            StudentId INT NOT NULL,
                            FOREIGN KEY (ClassroomId) REFERENCES Classrooms(Id),
                            FOREIGN KEY (StudentId) REFERENCES Users(Id)
                        );
                    END

                    -- Assignments Table
                    IF OBJECT_ID('Assignments', 'U') IS NULL
                    BEGIN
                        CREATE TABLE Assignments (
                            Id INT IDENTITY PRIMARY KEY,
                            Title NVARCHAR(100) NOT NULL,
                            Description NVARCHAR(255),
                            DueDate DATETIME NOT NULL,
                            CreatedById INT NOT NULL,
                            CreatedAt DATETIME NOT NULL,
                            UpdatedAt DATETIME,
                            IsArchived BIT DEFAULT 0,
                            ClassroomId INT NOT NULL,
                            FOREIGN KEY (ClassroomId) REFERENCES Classrooms(Id),
                            FOREIGN KEY (CreatedById) REFERENCES Users(Id)
                        );
                    END

                    --AssignmentFiles Table
                    IF OBJECT_ID('AssignmentFiles', 'U') IS NULL
                    BEGIN
                        CREATE TABLE AssignmentFiles (
                            Id INT IDENTITY PRIMARY KEY,
                            AssignmentId INT NOT NULL,
                            FileName NVARCHAR(200) NOT NULL,
                            FilePath NVARCHAR(500) NOT NULL,
                            FileSize BIGINT,
                            MimeType NVARCHAR(100),
                            UploadedAt DATETIME NOT NULL,
                            UploadedById INT NOT NULL,
                            FOREIGN KEY (AssignmentId) REFERENCES Assignments(Id),
                            FOREIGN KEY (UploadedById) REFERENCES Users(Id)
                        );
                    END

                    --AssignmentComments Table
                    IF OBJECT_ID('AssignmentComments', 'U') IS NULL
                    BEGIN
                        CREATE TABLE AssignmentComments (
                            Id INT IDENTITY PRIMARY KEY,
                            AssignmentId INT NOT NULL,
                            UserId INT NOT NULL,
                            Content NVARCHAR(1000) NOT NULL,
                            CreatedAt DATETIME NOT NULL,
                            UpdatedAt DATETIME,
                            UserName NVARCHAR(100),
                            FOREIGN KEY (AssignmentId) REFERENCES Assignments(Id),
                            FOREIGN KEY (UserId) REFERENCES Users(Id)
                        );
                    END

                    -- Submissions Table
                    IF OBJECT_ID('Submissions', 'U') IS NULL
                    BEGIN
                        CREATE TABLE Submissions (
                            Id INT IDENTITY PRIMARY KEY,
                            AssignmentId INT NOT NULL,
                            UserId INT NOT NULL,
                            Content NVARCHAR(MAX) NOT NULL,
                            SubmittedAt DATETIME NOT NULL,
                            GradedAt DATETIME,
                            Feedback NVARCHAR(500),
                            TotalGrade FLOAT,
                            FOREIGN KEY (AssignmentId) REFERENCES Assignments(Id),
                            FOREIGN KEY (UserId) REFERENCES Users(Id)
                        );
                    END

                    --SubmissionFiles
                    IF OBJECT_ID('SubmissionFiles', 'U') IS NULL
                    BEGIN
                        CREATE TABLE SubmissionFiles (
                            Id INT IDENTITY PRIMARY KEY,
                            SubmissionId INT NOT NULL,
                            FileName NVARCHAR(200) NOT NULL,
                            FilePath NVARCHAR(500) NOT NULL,
                            FileSize BIGINT,
                            MimeType NVARCHAR(100),
                            UploadedAt DATETIME NOT NULL,
                            UploadedById INT NOT NULL,
                            FOREIGN KEY (SubmissionId) REFERENCES Submissions(Id),
                            FOREIGN KEY (UploadedById) REFERENCES Users(Id)
                        );
                    END

                    --Assessments
                    IF OBJECT_ID('Assessments', 'U') IS NULL
                    BEGIN
                        CREATE TABLE Assessments (
                            Id INT IDENTITY PRIMARY KEY,
                            SubmissionId INT NOT NULL,
                            FacultyId INT NOT NULL,
                            AssessedAt DATETIME NOT NULL,
                            FOREIGN KEY (SubmissionId) REFERENCES Submissions(Id),
                            FOREIGN KEY (FacultyId) REFERENCES Users(Id)
                        );
                    END

                    --AssessmentCriteria
                    IF OBJECT_ID('AssessmentCriteria', 'U') IS NULL
                    BEGIN
                        CREATE TABLE AssessmentCriteria (
                            Id INT IDENTITY PRIMARY KEY,
                            CriterionName NVARCHAR(200) NOT NULL,
                            Description NVARCHAR(1000),
                            MaxScore FLOAT NOT NULL
                        );
                    END

                    --AssessedCriteria
                    IF OBJECT_ID('AssessedCriteria', 'U') IS NULL
                    BEGIN
                        CREATE TABLE AssessedCriteria (
                            Id INT IDENTITY PRIMARY KEY,
                            AssessmentId INT NOT NULL,
                            CriterionId INT NOT NULL,
                            Score FLOAT NOT NULL,            
                            Remarks NVARCHAR(1000),
                            FOREIGN KEY (AssessmentId) REFERENCES Assessments(Id),
                            FOREIGN KEY (CriterionId) REFERENCES AssessmentCriteria(Id)
                        );
                    END

                    IF OBJECT_ID('dbo.AssessmentCriteria', 'U') IS NOT NULL
                    BEGIN
                        -- Insert default criteria if not already present
                        IF NOT EXISTS (SELECT 1 FROM AssessmentCriteria WHERE CriterionName = 'Clarity')
                        BEGIN
                            INSERT INTO AssessmentCriteria (CriterionName, Description, MaxScore)
                            VALUES 
                                ('Clarity', 'Clear explanation and understanding of the topic.', 10),
                                ('Completeness', 'All required parts are included and addressed.', 10),
                                ('Originality', 'Demonstrates original thought or unique approach.', 10),
                                ('Presentation', 'Well-organized, structured, and formatted.', 5);
                        END
                    END
                END;
            ";

            await _db.ExecuteAsync(createProcSql);
        }

        public async Task RunStoredProcedureAsync()
        {
            await _db.ExecuteAsync("sp_Createtables", commandType: CommandType.StoredProcedure);
        }
        public async Task SeedUsersAsync()
        {
            var usersToSeed = new[]
            {
                new { Name = "Student One", Email = "student1@example.com", Password = "student123", Role = "Student" },
                new { Name = "Faculty One", Email = "faculty1@example.com", Password = "faculty123", Role = "Faculty" },
                new { Name = "Student Two", Email = "student2@example.com", Password = "student234", Role = "Student" },
                new { Name = "Faculty Two", Email = "faculty2@example.com", Password = "faculty234", Role = "Faculty" }
            };

            var emails = usersToSeed.Select(u => u.Email).ToArray();

            var existingUsers = (await _db.QueryAsync<string>(
                "SELECT Email FROM Users WHERE Email IN @emails", new { emails }))
                .ToHashSet();

            var newUsers = usersToSeed
                .Where(u => !existingUsers.Contains(u.Email))
                .Select(u => new
                {
                    u.Name,
                    u.Email,
                    PasswordHash = PasswordHelper.HashPassword(u.Password),
                    u.Role
                });

            if (newUsers.Any())
            {
                var sql = @"
                    INSERT INTO Users (Name, Email, PasswordHash, Role, CreatedAt)
                    VALUES (@Name, @Email, @PasswordHash, @Role, GETUTCDATE())";

                await _db.ExecuteAsync(sql, newUsers);
            }
        }
    }
}
