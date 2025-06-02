using AssignmentPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace AssignmentPortal.Data
{
    public class ApplicationContext: DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
        // DbSets for your models
        public DbSet<Assignment> Assignments { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Submission> Submissions { get; set; } = null!;
        public DbSet<SubmissionFile> SubmissionFiles { get; set; } = null!;
        public DbSet<AssignmentFile> AssignmentFiles { get; set; } = null!;
        public DbSet<AssignmentComment> AssignmentComments { get; set; } = null!;
        public DbSet<AssessmentCriterion> AssessmentCriteria { get; set; } = null!;
        public DbSet<Assessment> Assessments { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssignmentComment>()
                        .HasOne(ac => ac.User)
                        .WithMany()
                        .HasForeignKey(ac => ac.UserId)
                        .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssignmentFile>()
                        .HasOne(af => af.UploadedBy)
                        .WithMany()
                        .HasForeignKey(af => af.UploadedById)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignmentFile>()
                        .HasOne(af => af.Assignment)
                        .WithMany()
                        .HasForeignKey(af => af.AssignmentId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Submission>()
                        .HasOne(s => s.User)
                        .WithMany()
                        .HasForeignKey(s => s.UserId)
                        .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Submission>()
                        .HasOne(s => s.Assignment)
                        .WithMany()
                        .HasForeignKey(s => s.AssignmentId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Assessment>()
                        .HasOne(a => a.Faculty)
                        .WithMany()
                        .HasForeignKey(a => a.FacultyId)
                        .OnDelete(DeleteBehavior.Restrict); // Prevent cascade from Users

            modelBuilder.Entity<Assessment>()
                        .HasOne(a => a.Submission)
                        .WithMany()
                        .HasForeignKey(a => a.SubmissionId)
                        .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SubmissionFile>()
                        .HasOne(sf => sf.Submission)
                        .WithMany()
                        .HasForeignKey(sf => sf.SubmissionId)
                        .OnDelete(DeleteBehavior.Cascade); // allow cascading here

            modelBuilder.Entity<SubmissionFile>()
                        .HasOne(sf => sf.UploadedBy)
                        .WithMany()
                        .HasForeignKey(sf => sf.UploadedById)
                        .OnDelete(DeleteBehavior.Restrict);
            base.OnModelCreating(modelBuilder);
        }
    }
}
