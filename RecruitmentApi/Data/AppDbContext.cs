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
            entity.HasKey(e => e.candidate_id);

            entity.HasIndex(e => e.email, "UQ_Candidates_Email").IsUnique();

            entity.Property(e => e.candidate_id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.full_name).HasMaxLength(255);
            entity.Property(e => e.phone).HasMaxLength(20);
            entity.Property(e => e.resume_path).HasMaxLength(500);
            entity.Property(e => e.password).HasMaxLength(255);
        });

        modelBuilder.Entity<Candidate_Document>(entity =>
        {
            entity.HasKey(e => e.document_id);

            entity.Property(e => e.candidate_id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.document_type).HasMaxLength(100);
            entity.Property(e => e.file_path).HasMaxLength(500);
            entity.Property(e => e.uploaded_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.verification_status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.candidate).WithMany(p => p.Candidate_Documents)
                .HasForeignKey(d => d.candidate_id)
                .HasConstraintName("FK_Candidate_Documents_Candidates");
        });

        modelBuilder.Entity<Candidate_Review>(entity =>
        {
            entity.HasKey(e => e.review_id);

            entity.Property(e => e.candidate_id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.reviewed_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.user_id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.candidate).WithMany(p => p.Candidate_Reviews)
                .HasForeignKey(d => d.candidate_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidate_Reviews_Candidates");

            entity.HasOne(d => d.job).WithMany(p => p.Candidate_Reviews)
                .HasForeignKey(d => d.job_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidate_Reviews_Jobs");

            entity.HasOne(d => d.user).WithMany(p => p.Candidate_Reviews)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidate_Reviews_Users");
        });

        modelBuilder.Entity<Candidate_Skill>(entity =>
        {
            entity.HasKey(e => e.candidate_skill_id);

            entity.Property(e => e.candidate_id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.candidate).WithMany(p => p.Candidate_Skills)
                .HasForeignKey(d => d.candidate_id)
                .HasConstraintName("FK_Candidate_Skills_Candidates");

            entity.HasOne(d => d.skill).WithMany(p => p.CandidateSkills)
                .HasForeignKey(d => d.skill_id)
                .HasConstraintName("FK_Candidate_Skills_Skills");
        });

        modelBuilder.Entity<Candidate_Status_History>(entity =>
        {
            entity.HasKey(e => e.candidate_status_id);

            entity.ToTable("Candidate_Status_History");

            entity.Property(e => e.candidate_id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.changed_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.changed_by)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.reason).HasMaxLength(500);
            entity.Property(e => e.status).HasMaxLength(50);

            entity.HasOne(d => d.candidate).WithMany(p => p.Candidate_Status_Histories)
                .HasForeignKey(d => d.candidate_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidate_Status_History_Candidates");

            entity.HasOne(d => d.changed_by_user).WithMany(p => p.Candidate_Status_Histories)
                .HasForeignKey(d => d.changed_by)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidate_Status_History_Users");

            entity.HasOne(d => d.job).WithMany(p => p.Candidate_Status_Histories)
                .HasForeignKey(d => d.job_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Candidate_Status_History_Jobs");
        });

        modelBuilder.Entity<HR_Review>(entity =>
        {
            entity.HasKey(e => e.review_id);

            entity.ToTable("HR_Review");

            entity.Property(e => e.user_id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .IsRequired();

            entity.Property(e => e.strengths).HasMaxLength(2000);
            entity.Property(e => e.areas_for_improvement).HasMaxLength(2000);
            entity.Property(e => e.training_recommendations).HasMaxLength(2000);
            entity.Property(e => e.career_path_notes).HasMaxLength(2000);

            entity.HasOne(d => d.interview)
                .WithMany(p => p.HR_Reviews)
                .HasForeignKey(d => d.interview_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HR_Review_Interviews");

            entity.HasOne(d => d.user)
                .WithMany(p => p.HR_Reviews)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HR_Review_Users");
        });


        modelBuilder.Entity<Employee_Record>(entity =>
        {
            entity.HasKey(e => e.employee_id);

            entity.HasIndex(e => e.candidate_id, "UQ_Employee_Records_Candidate").IsUnique();

            entity.Property(e => e.employee_id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.candidate_id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.offer_letter_path).HasMaxLength(500);

            entity.HasOne(d => d.candidate).WithOne(p => p.Employee_Record)
                .HasForeignKey<Employee_Record>(d => d.candidate_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Records_Candidates");

            entity.HasOne(d => d.job).WithMany(p => p.Employee_Records)
                .HasForeignKey(d => d.job_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Records_Jobs");
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.interview_id)
                  .HasName("PK_Interviews");

            entity.Property(e => e.candidate_id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .IsRequired();

            entity.Property(e => e.location_or_link)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(e => e.mode)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.scheduled_by)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .IsRequired();

            entity.Property(e => e.start_time)
                .IsRequired();

            entity.Property(e => e.end_time)
            .IsRequired();

            entity.Property(e => e.status)
            .HasMaxLength(100)
            .IsRequired();

            // Foreign key relationships
            entity.HasOne(d => d.candidate)
                .WithMany(p => p.Interviews)
                .HasForeignKey(d => d.candidate_id)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Interviews_Candidates");

            entity.HasOne(d => d.interview_type)
                .WithMany(p => p.Interviews)
                .HasForeignKey(d => d.interview_type_id)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Interviews_Interview_Type");

            entity.HasOne(d => d.job)
                .WithMany(p => p.Interviews)
                .HasForeignKey(d => d.job_id)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Interviews_Jobs");

            entity.HasOne(d => d.scheduled_by_user)
                .WithMany(p => p.ScheduledInterviews)
                .HasForeignKey(d => d.scheduled_by)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Interviews_Users");
        });


        modelBuilder.Entity<Interview_Feedback>(entity =>
        {
            entity.HasKey(e => e.feedback_id);

            entity.ToTable("Interview_Feedback");

            entity.Property(e => e.feedback_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.user_id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.interview).WithMany(p => p.Interview_Feedbacks)
                .HasForeignKey(d => d.interview_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Interview_Feedback_Interviews");

            entity.HasOne(d => d.candidate_skill).WithMany(p => p.Interview_Feedbacks)
                .HasForeignKey(d => d.candidate_skill_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Interview_Feedback_CandidateSkills");

            entity.HasOne(d => d.user).WithMany(p => p.Interview_Feedbacks)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Interview_Feedback_Users");
        });

        modelBuilder.Entity<Interview_Type>(entity =>
        {
            entity.HasKey(e => e.interview_type_id);

            entity.ToTable("Interview_Type");

            entity.Property(e => e.interview_round_name).HasMaxLength(100);
            entity.Property(e => e.process_descreption).HasMaxLength(1000);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.job_id);

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.created_by)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.job_title).HasMaxLength(255);
            entity.Property(e => e.scheduled).HasMaxLength(50).HasDefaultValue("Pending");


            entity.HasOne(d => d.created_by_user).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.created_by)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Jobs_Users");

            entity.HasOne(d => d.status).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.status_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Jobs_Jobs_Status");
        });

        modelBuilder.Entity<Jobs_Skill>(entity =>
        {
            entity.HasKey(e => new { e.skill_id, e.job_id });

            entity.Property(e => e.skill_type)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.job).WithMany(p => p.Jobs_Skills)
                .HasForeignKey(d => d.job_id)
                .HasConstraintName("FK_Jobs_Skills_Jobs");

            entity.HasOne(d => d.skill).WithMany(p => p.JobsSkills)
                .HasForeignKey(d => d.skill_id)
                .HasConstraintName("FK_Jobs_Skills_Skills");
        });

        modelBuilder.Entity<Jobs_Status>(entity =>
        {
            entity.HasKey(e => e.status_id);

            entity.ToTable("Jobs_Status");

            entity.Property(e => e.changed_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.changed_by)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.reason).HasMaxLength(500);
            entity.Property(e => e.status).HasMaxLength(50);

            entity.HasOne(d => d.changed_by_user).WithMany(p => p.Jobs_Statuses)
                .HasForeignKey(d => d.changed_by)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Jobs_Status_Users");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.notification_id);

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.message).HasMaxLength(1000);
            entity.Property(e => e.status)
                .HasMaxLength(20)
                .HasDefaultValue("Unread");
            entity.Property(e => e.user_id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.user).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.role_id);

            entity.Property(e => e.role_name).HasMaxLength(100);

            entity.HasMany(d => d.users).WithMany(p => p.roles)
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
            entity.HasKey(e => e.user_id);

            entity.HasIndex(e => e.email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.user_id)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.name).HasMaxLength(255);
            entity.Property(e => e.password_hash).HasMaxLength(255);

            entity.HasMany(d => d.AttendedInterviews).WithMany(p => p.users)
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
