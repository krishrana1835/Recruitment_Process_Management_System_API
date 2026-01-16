using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RecruitmentApi.Models;

namespace RecruitmentApi.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Candidate> Candidates { get; set; }

    public virtual DbSet<Candidate_Document> Candidate_Documents { get; set; }

    public virtual DbSet<Candidate_Review> Candidate_Reviews { get; set; }

    public virtual DbSet<Candidate_Skill> Candidate_Skills { get; set; }

    public virtual DbSet<Candidate_Status_History> Candidate_Status_Histories { get; set; }

    public virtual DbSet<Employee_Record> Employee_Records { get; set; }

    public virtual DbSet<Interview> Interviews { get; set; }

    public virtual DbSet<Interview_Feedback> Interview_Feedbacks { get; set; }

    public virtual DbSet<Interview_Type> Interview_Types { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<HR_Review> HR_Reviews { get; set; }


    public virtual DbSet<Jobs_Skill> Jobs_Skills { get; set; }

    public virtual DbSet<Jobs_Status> Jobs_Statuses { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<EmailMessage> EmailMessages { get; set; }
    public virtual DbSet<EmailRecipient> EmailRecipients { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=RecruitmentProcessManage;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.HasKey(e => e.CandidateId);

            entity.HasIndex(e => e.Email, "UQ_Candidates_Email").IsUnique();

            entity.Property(e => e.CandidateId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.ResumePath).HasMaxLength(500);
            entity.Property(e => e.Password).HasMaxLength(255);
        });

        modelBuilder.Entity<Candidate_Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId);

            entity.Property(e => e.CandidateId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DocumentType).HasMaxLength(100);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.VerificationStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateDocuments)
                .HasForeignKey(d => d.CandidateId)
                .HasConstraintName("FK_Candidate_Documents_Candidates");
        });

        modelBuilder.Entity<Candidate_Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId);

            entity.Property(e => e.CandidateId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ReviewedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateReviews)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidate_Reviews_Candidates");

            entity.HasOne(d => d.Job).WithMany(p => p.CandidateReviews)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidate_Reviews_Jobs");

            entity.HasOne(d => d.User).WithMany(p => p.CandidateReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidate_Reviews_Users");
        });

        modelBuilder.Entity<Candidate_Skill>(entity =>
        {
            entity.HasKey(e => e.CandidateSkillId);

            entity.Property(e => e.CandidateId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateSkills)
                .HasForeignKey(d => d.CandidateId)
                .HasConstraintName("FK_Candidate_Skills_Candidates");

            entity.HasOne(d => d.Skill).WithMany(p => p.CandidateSkills)
                .HasForeignKey(d => d.SkillId)
                .HasConstraintName("FK_Candidate_Skills_Skills");
        });

        modelBuilder.Entity<Candidate_Status_History>(entity =>
        {
            entity.HasKey(e => e.CandidateStatusId);

            entity.ToTable("Candidate_Status_History");

            entity.Property(e => e.CandidateId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ChangedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ChangedBy)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateStatusHistories)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidate_Status_History_Candidates");

            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.CandidateStatusHistories)
                .HasForeignKey(d => d.ChangedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidate_Status_History_Users");

            entity.HasOne(d => d.Job).WithMany(p => p.CandidateStatusHistories)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidate_Status_History_Jobs");
        });

        modelBuilder.Entity<HR_Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId);

            entity.ToTable("HR_Review");

            entity.Property(e => e.UserId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .IsRequired();

            entity.Property(e => e.Strengths).HasMaxLength(2000);
            entity.Property(e => e.AreasForImprovement).HasMaxLength(2000);
            entity.Property(e => e.TrainingRecommendations).HasMaxLength(2000);
            entity.Property(e => e.CareerPathNotes).HasMaxLength(2000);

            entity.HasOne(d => d.Interview)
                .WithMany(p => p.HrReviews)
                .HasForeignKey(d => d.InterviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HR_Review_Interviews");

            entity.HasOne(d => d.User)
                .WithMany(p => p.HrReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HR_Review_Users");
        });


        modelBuilder.Entity<Employee_Record>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);

            entity.HasIndex(e => e.CandidateId, "UQ_Employee_Records_Candidate").IsUnique();

            entity.Property(e => e.EmployeeId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CandidateId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OfferLetterPath).HasMaxLength(500);

            entity.HasOne(d => d.Candidate).WithOne(p => p.EmployeeRecord)
                .HasForeignKey<Employee_Record>(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Records_Candidates");

            entity.HasOne(d => d.Job).WithMany(p => p.EmployeeRecords)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Records_Jobs");
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.InterviewId)
                  .HasName("PK_Interviews");

            entity.Property(e => e.CandidateId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .IsRequired();

            entity.Property(e => e.LocationOrLink)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(e => e.Mode)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.ScheduledBy)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .IsRequired();

            entity.Property(e => e.StartTime)
                .IsRequired();

            entity.Property(e => e.EndTime)
            .IsRequired();

            entity.Property(e => e.AllowFeedback)
            .HasMaxLength(20)
            .IsRequired();

            entity.Property(e => e.Status)
            .HasMaxLength(100)
            .IsRequired();

            // Foreign key relationships
            entity.HasOne(d => d.Candidate)
                .WithMany(p => p.Interviews)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Interviews_Candidates");

            entity.HasOne(d => d.InterviewType)
                .WithMany(p => p.Interviews)
                .HasForeignKey(d => d.InterviewTypeId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Interviews_Interview_Type");

            entity.HasOne(d => d.Job)
                .WithMany(p => p.Interviews)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Interviews_Jobs");

            entity.HasOne(d => d.ScheduledByUser)
                .WithMany(p => p.ScheduledInterviews)
                .HasForeignKey(d => d.ScheduledBy)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Interviews_Users");
        });


        modelBuilder.Entity<Interview_Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId);

            entity.ToTable("Interview_Feedback");

            entity.Property(e => e.FeedbackAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Interview).WithMany(p => p.InterviewFeedbacks)
                .HasForeignKey(d => d.InterviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Interview_Feedback_Interviews");

            entity.HasOne(d => d.CandidateSkill).WithMany(p => p.InterviewFeedbacks)
                .HasForeignKey(d => d.CandidateSkillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Interview_Feedback_CandidateSkills");

            entity.HasOne(d => d.User).WithMany(p => p.InterviewFeedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Interview_Feedback_Users");
        });

        modelBuilder.Entity<Interview_Type>(entity =>
        {
            entity.HasKey(e => e.InterviewTypeId);

            entity.ToTable("Interview_Type");

            entity.Property(e => e.InterviewRoundName).HasMaxLength(100);
            entity.Property(e => e.ProcessDescreption).HasMaxLength(1000);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.JobId);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.JobTitle).HasMaxLength(255);
            entity.Property(e => e.Scheduled).HasMaxLength(50).HasDefaultValue("Pending");


            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Jobs_Users");

            entity.HasOne(d => d.Status).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Jobs_Jobs_Status");
        });

        modelBuilder.Entity<Jobs_Skill>(entity =>
        {
            entity.HasKey(e => new { e.SkillId, e.JobId });

            entity.Property(e => e.SkillType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Job).WithMany(p => p.JobsSkills)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_Jobs_Skills_Jobs");

            entity.HasOne(d => d.Skill).WithMany(p => p.JobsSkills)
                .HasForeignKey(d => d.SkillId)
                .HasConstraintName("FK_Jobs_Skills_Skills");
        });

        modelBuilder.Entity<Jobs_Status>(entity =>
        {
            entity.HasKey(e => e.StatusId);

            entity.ToTable("Jobs_Status");

            entity.Property(e => e.ChangedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ChangedBy)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.JobsStatuses)
                .HasForeignKey(d => d.ChangedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Jobs_Status_Users");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Unread");
            entity.Property(e => e.UserId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.Property(e => e.RoleName).HasMaxLength(100);

            entity.HasMany(d => d.Users).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "Users_Role",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("user_id")
                        .HasConstraintName("FK_Users_Roles_Users"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("role_id")
                        .HasConstraintName("FK_Users_Roles_Roles"),
                    j =>
                    {
                        j.HasKey("role_id", "user_id");
                        j.ToTable("Users_Roles");
                        j.IndexerProperty<string>("user_id")
                            .HasMaxLength(8)
                            .IsUnicode(false)
                            .IsFixedLength();
                    });
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId);

            entity.HasIndex(e => e.SkillName, "UQ_Skills_Name").IsUnique();

            entity.Property(e => e.SkillName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.UserId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);

            entity.HasMany(d => d.AttendedInterviews).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "Interview_Panel",
                    r => r.HasOne<Interview>().WithMany()
                        .HasForeignKey("interview_id")
                        .HasConstraintName("FK_Interview_Panel_Interviews"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Interview_Panel_Users"),
                    j =>
                    {
                        j.HasKey("user_id", "interview_id");
                        j.ToTable("Interview_Panel");
                        j.IndexerProperty<string>("user_id")
                            .HasMaxLength(8)
                            .IsUnicode(false)
                            .IsFixedLength();
                    });
        });

        modelBuilder.Entity<EmailRecipient>()
            .HasOne(r => r.EmailMessage)
            .WithMany(e => e.Recipients)
            .HasForeignKey(r => r.EmailMessageId);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
