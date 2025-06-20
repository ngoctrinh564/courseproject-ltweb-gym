using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace gym.Data;

public partial class GymContext : DbContext
{
    public GymContext()
    {
    }

    public GymContext(DbContextOptions<GymContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberPakage> MemberPakages { get; set; }

    public virtual DbSet<MemberPayment> MemberPayments { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    public virtual DbSet<TrainingSchedule> TrainingSchedules { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<UserNotification> UserNotifications { get; set; }
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=haiit;Initial Catalog=GYM;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId)
                .HasName("PK_MEMBER")
                .IsClustered(false);

            entity.ToTable("Member");

            entity.Property(e => e.MemberId).HasColumnName("memberId");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("createDate");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("datetime")
                .HasColumnName("dateOfBirth");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("fullName");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.Sex).HasColumnName("sex");
        });

        modelBuilder.Entity<MemberPakage>(entity =>
        {
            entity.HasKey(e => new { e.MemberId, e.PackageId }).HasName("PK_MEMBERPAKAGE");

            entity.ToTable("MemberPakage");

            entity.HasIndex(e => e.PackageId, "MemberPakage2_FK");

            entity.HasIndex(e => e.MemberId, "MemberPakage_FK");

            entity.Property(e => e.MemberId).HasColumnName("memberId");
            entity.Property(e => e.PackageId).HasColumnName("packageId");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("endDate");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.IsPaid).HasColumnName("isPaid");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("startDate");

            entity.HasOne(d => d.Member).WithMany(p => p.MemberPakages)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEMBERPA_MEMBERPAK_MEMBER");

            entity.HasOne(d => d.Package).WithMany(p => p.MemberPakages)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEMBERPA_MEMBERPAK_PACKAGE");
            // ✅ Thêm quan hệ với bảng Payment
            entity.HasOne(d => d.Payment)
                .WithMany() // Nếu 1 payment gắn 1 MemberPakage → dùng WithOne()
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK_MEMBERPA_MEMBERPAK_PAYMENT");
        });

        modelBuilder.Entity<MemberPayment>(entity =>
        {
            entity.HasKey(e => new { e.MemberId, e.PaymentId }).HasName("PK_MEMBERPAYMENT");

            entity.ToTable("MemberPayment");

            entity.HasIndex(e => e.PaymentId, "MemberPayment2_FK");

            entity.HasIndex(e => e.StaffId, "MemberPayment3_FK");

            entity.HasIndex(e => e.MemberId, "MemberPayment_FK");

            entity.Property(e => e.MemberId).HasColumnName("memberId");
            entity.Property(e => e.PaymentId).HasColumnName("paymentId");
            //entity.Property(e => e.StaffId).HasColumnName("staffId");
            entity.Property(e => e.StaffId)
                .HasColumnName("staffId"); // KHÔNG cần thêm gì khác

            entity.HasOne(e => e.Staff)
                .WithMany()
                .HasForeignKey(e => e.StaffId)
                .IsRequired(false); // <-- Đây là phần QUAN TRỌNG để cho phép null

            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("paymentDate");

            entity.HasOne(d => d.Member).WithMany(p => p.MemberPayments)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEMBERPA_MEMBERPAY_MEMBER");

            entity.HasOne(d => d.Payment).WithMany(p => p.MemberPayments)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEMBERPA_MEMBERPAY_PAYMENT");

            entity.HasOne(d => d.Staff).WithMany(p => p.MemberPayments)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEMBERPA_MEMBERPAY_STAFF");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId)
                .HasName("PK_NOTIFICATION")
                .IsClustered(false);

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("notificationId");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.SendRole).HasColumnName("sendRole");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackageId)
                .HasName("PK_PACKAGE")
                .IsClustered(false);

            entity.ToTable("Package");

            entity.Property(e => e.PackageId).HasColumnName("packageId");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DurationInDays).HasColumnName("durationInDays");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type");
            entity.Property(e => e.IsActive)
                .HasColumnName("isActive")
                .HasDefaultValue(true);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId)
                .HasName("PK_PAYMENT")
                .IsClustered(false);

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("paymentId");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");
            entity.Property(e => e.Method)
                .HasMaxLength(100)
                .HasColumnName("method");

            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsPaid)
                .HasColumnName("isPaid")
                .HasDefaultValue(false);

            entity.Property(e => e.DueDate)
                .HasColumnName("dueDate");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId)
                .HasName("PK_ROLE")
                .IsClustered(false);

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId)
                .HasName("PK_STAFF")
                .IsClustered(false);

            entity.Property(e => e.StaffId).HasColumnName("staffId");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("fullName");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.WorkingSince)
                .HasColumnType("datetime")
                .HasColumnName("workingSince");
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(e => e.TrainerId)
                .HasName("PK_TRAINER")
                .IsClustered(false);

            entity.ToTable("Trainer");

            entity.Property(e => e.TrainerId).HasColumnName("trainerId");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("fullName");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.ScheduleNote)
                .HasMaxLength(255)
                .HasColumnName("scheduleNote");
            entity.Property(e => e.Specialty)
                .HasMaxLength(255)
                .HasColumnName("specialty");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.Gender)
              .HasColumnName("Gender")
              .HasColumnType("bit");
        });

        modelBuilder.Entity<TrainingSchedule>(entity =>
        {
            entity.HasKey(e => new { e.TrainerId, e.MemberId }).HasName("PK_TRAININGSCHEDULE");

            entity.ToTable("TrainingSchedule");

            entity.HasIndex(e => e.MemberId, "TrainingSchedule2_FK");

            entity.HasIndex(e => e.TrainerId, "TrainingSchedule_FK");

            entity.Property(e => e.TrainerId).HasColumnName("trainerId");
            entity.Property(e => e.MemberId).HasColumnName("memberId");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("endTime");
            entity.Property(e => e.Node)
                .HasMaxLength(255)
                .HasColumnName("node");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("startTime");
            entity.Property(e => e.TrainingDate)
                .HasColumnType("datetime")
                .HasColumnName("trainingDate");

            entity.HasOne(d => d.Member).WithMany(p => p.TrainingSchedules)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TRAINING_TRAININGS_MEMBER");

            entity.HasOne(d => d.Trainer).WithMany(p => p.TrainingSchedules)
                .HasForeignKey(d => d.TrainerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TRAINING_TRAININGS_TRAINER");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId)
                .HasName("PK_USER")
                .IsClustered(false);

            entity.ToTable("User");

            entity.HasIndex(e => e.RoleId, "include_FK");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.IsAtive).HasColumnName("isAtive");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.ReferenceId).HasColumnName("referenceId");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("userName");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USER_INCLUDE_ROLE");
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.HasKey(e => new { e.NotificationId, e.UserId }).HasName("PK_USERNOTIFICATION");

            entity.ToTable("UserNotification");

            entity.HasIndex(e => e.UserId, "UserNotification2_FK");

            entity.HasIndex(e => e.NotificationId, "UserNotification_FK");

            entity.Property(e => e.NotificationId).HasColumnName("notificationId");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Seen).HasColumnName("seen");
            entity.Property(e => e.TimeSend)
                .HasColumnType("datetime")
                .HasColumnName("timeSend");

            entity.HasOne(d => d.Notification).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USERNOTI_USERNOTIF_NOTIFICA");

            entity.HasOne(d => d.User).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USERNOTI_USERNOTIF_USER");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId)
                .HasName("PK_Feedback");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedbackId");

            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");

            entity.Property(e => e.ThoiGian)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("thoiGian");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_Feedback_User");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
