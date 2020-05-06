using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OmniCRM_Web.Models
{
    public partial class OmniCRMContext : DbContext
    {
        public OmniCRMContext()
        {
        }

        public OmniCRMContext(DbContextOptions<OmniCRMContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppoinmentStatusMaster> AppoinmentStatusMaster { get; set; }
        public virtual DbSet<AppointmentDetail> AppointmentDetail { get; set; }
        public virtual DbSet<CallDetail> CallDetail { get; set; }
        public virtual DbSet<CallOutcomeMaster> CallOutcomeMaster { get; set; }
        public virtual DbSet<CallTransactionDetail> CallTransactionDetail { get; set; }
        public virtual DbSet<FollowupHistory> FollowupHistory { get; set; }
        public virtual DbSet<RoleMaster> RoleMaster { get; set; }
        public virtual DbSet<UserMaster> UserMaster { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=OmniCRM;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppoinmentStatusMaster>(entity =>
            {
                entity.HasKey(e => e.AppoinStatusId);

                entity.Property(e => e.AppoinStatusId).HasColumnName("AppoinStatusID");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<AppointmentDetail>(entity =>
            {
                entity.HasKey(e => e.AppintmentId);

                entity.Property(e => e.AppintmentId).HasColumnName("AppintmentID");

                entity.Property(e => e.AppoinStatusId).HasColumnName("AppoinStatusID");

                entity.Property(e => e.AppointmentDateTime).HasColumnType("datetime");

                entity.Property(e => e.CallId).HasColumnName("CallID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RelationshipManagerId).HasColumnName("RelationshipManagerID");

                entity.HasOne(d => d.AppoinStatus)
                    .WithMany(p => p.AppointmentDetail)
                    .HasForeignKey(d => d.AppoinStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppointmentDetail_AppoinmentStatusMaster");

                entity.HasOne(d => d.Call)
                    .WithMany(p => p.AppointmentDetail)
                    .HasForeignKey(d => d.CallId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppointmentDetail_CallDetail");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.AppointmentDetail)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppointmentDetail_UserMaster");
            });

            modelBuilder.Entity<CallDetail>(entity =>
            {
                entity.HasKey(e => e.CallId);

                entity.Property(e => e.CallId).HasColumnName("CallID");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FirmName).HasMaxLength(128);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastChangedDate).HasColumnType("datetime");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(12);

                entity.Property(e => e.OutComeId).HasColumnName("OutComeID");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.CallDetail)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CallDetail_UserMaster");

                entity.HasOne(d => d.OutCome)
                    .WithMany(p => p.CallDetail)
                    .HasForeignKey(d => d.OutComeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CallDetail_CallOutcomeMaster");
            });

            modelBuilder.Entity<CallOutcomeMaster>(entity =>
            {
                entity.HasKey(e => e.OutComeId);

                entity.Property(e => e.OutComeId).HasColumnName("OutComeID");

                entity.Property(e => e.OutCome)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<CallTransactionDetail>(entity =>
            {
                entity.HasKey(e => e.CallTransactionId);

                entity.Property(e => e.CallTransactionId).HasColumnName("CallTransactionID");

                entity.Property(e => e.CallId).HasColumnName("CallID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OutComeId).HasColumnName("OutComeID");

                entity.HasOne(d => d.Call)
                    .WithMany(p => p.CallTransactionDetail)
                    .HasForeignKey(d => d.CallId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CallTransactionDetail_CallDetail");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.CallTransactionDetail)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CallTransactionDetail_UserMaster");

                entity.HasOne(d => d.OutCome)
                    .WithMany(p => p.CallTransactionDetail)
                    .HasForeignKey(d => d.OutComeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CallTransactionDetail_CallOutcomeMaster");
            });

            modelBuilder.Entity<FollowupHistory>(entity =>
            {
                entity.HasKey(e => e.FollowupId);

                entity.Property(e => e.FollowupId).HasColumnName("FollowupID");

                entity.Property(e => e.AppoinDate).HasColumnType("datetime");

                entity.Property(e => e.AppoinStatusId).HasColumnName("AppoinStatusID");

                entity.Property(e => e.CallId).HasColumnName("CallID");

                entity.Property(e => e.CreatedByRmanagerId).HasColumnName("CreatedByRManagerID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FollowupType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.AppoinStatus)
                    .WithMany(p => p.FollowupHistory)
                    .HasForeignKey(d => d.AppoinStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FollowupHistory_AppoinmentStatusMaster");

                entity.HasOne(d => d.Call)
                    .WithMany(p => p.FollowupHistory)
                    .HasForeignKey(d => d.CallId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FollowupHistory_CallDetail");

                entity.HasOne(d => d.CreatedByRmanager)
                    .WithMany(p => p.FollowupHistory)
                    .HasForeignKey(d => d.CreatedByRmanagerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FollowupHistory_UserMaster");
            });

            modelBuilder.Entity<RoleMaster>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserMaster>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.HasIndex(e => e.Email)
                    .HasName("UK_UserMaster")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.EmployeeCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LinkExpiryDate).HasColumnType("datetime");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserMaster)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserMaster_RoleMaster");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
