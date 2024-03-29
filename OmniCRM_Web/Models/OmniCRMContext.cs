﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

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

        public virtual DbSet<AdminSetting> AdminSetting { get; set; }
        public virtual DbSet<AppoinmentStatusMaster> AppoinmentStatusMaster { get; set; }
        public virtual DbSet<AppointmentDetail> AppointmentDetail { get; set; }
        public virtual DbSet<CallDetail> CallDetail { get; set; }
        public virtual DbSet<CallDetailArchive> CallDetailArchive { get; set; }
        public virtual DbSet<CallOutcomeMaster> CallOutcomeMaster { get; set; }
        public virtual DbSet<CallTransactionDetail> CallTransactionDetail { get; set; }
        public virtual DbSet<CityMaster> CityMaster { get; set; }
        public virtual DbSet<CompanyMaster> CompanyMaster { get; set; }
        public virtual DbSet<FollowupHistory> FollowupHistory { get; set; }
        public virtual DbSet<ProductMaster> ProductMaster { get; set; }
        public virtual DbSet<RoleMaster> RoleMaster { get; set; }
        public virtual DbSet<StateMaster> StateMaster { get; set; }
        public virtual DbSet<TargetMaster> TargetMaster { get; set; }
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
            modelBuilder.Entity<AdminSetting>(entity =>
            {
                entity.HasKey(e => e.SettingId);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DailyEmailTime).HasColumnType("datetime");

                entity.Property(e => e.OverDueDaysRm).HasColumnName("OverDueDaysRM");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.AdminSetting)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AdminSetting_UserMaster");
            });

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

                entity.HasIndex(e => e.AppoinStatusId)
                    .HasName("IX_AppointmentDetail_AppointStatusID");

                entity.HasIndex(e => e.AppointmentDateTime)
                    .HasName("IX_AppointmentDetail_AppointDateTime");

                entity.HasIndex(e => e.RelationshipManagerId)
                    .HasName("IX_AppointmentDetail_RelationShipMngrID");

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

                entity.HasIndex(e => e.CompanyId);

                entity.HasIndex(e => e.CreatedBy);

                entity.HasIndex(e => e.LastChangedDate);

                entity.HasIndex(e => e.OutComeId);

                entity.Property(e => e.CallId).HasColumnName("CallID");

                entity.Property(e => e.CityId).HasColumnName("CityID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmailId)
                    .HasColumnName("EmailID")
                    .HasMaxLength(256);

                entity.Property(e => e.FirmName).HasMaxLength(128);

                entity.Property(e => e.FirstName).IsRequired();

                entity.Property(e => e.LastChangedDate).HasColumnType("datetime");

                entity.Property(e => e.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(12);

                entity.Property(e => e.NextCallDate).HasColumnType("datetime");

                entity.Property(e => e.OutComeId).HasColumnName("OutComeID");

                entity.Property(e => e.StateId).HasColumnName("StateID");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.CallDetail)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_CallDetail_CityMaster");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CallDetail)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CallDetail_CompanyMaster");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.CallDetail)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_CallDetail_UserMaster");

                entity.HasOne(d => d.OutCome)
                    .WithMany(p => p.CallDetail)
                    .HasForeignKey(d => d.OutComeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CallDetail_CallOutcomeMaster");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CallDetail)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_CallDetail_ProductMaster");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.CallDetail)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("FK_CallDetail_StateMaster");
            });

            modelBuilder.Entity<CallDetailArchive>(entity =>
            {
                entity.HasKey(e => e.CallId);

                entity.Property(e => e.CallId)
                    .HasColumnName("CallID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CityId).HasColumnName("CityID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailId)
                    .HasColumnName("EmailID")
                    .HasMaxLength(256);

                entity.Property(e => e.FirmName).HasMaxLength(128);

                entity.Property(e => e.FirstName).IsRequired();

                entity.Property(e => e.LastChangedDate).HasColumnType("datetime");

                entity.Property(e => e.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(12);

                entity.Property(e => e.NextCallDate).HasColumnType("datetime");

                entity.Property(e => e.OutComeId).HasColumnName("OutComeID");

                entity.Property(e => e.StateId).HasColumnName("StateID");
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

            modelBuilder.Entity<CityMaster>(entity =>
            {
                entity.HasKey(e => e.CityId);

                entity.Property(e => e.CityId).HasColumnName("CityID");

                entity.Property(e => e.CityName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StateId).HasColumnName("StateID");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.CityMaster)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CityMaster_StateMaster");
            });

            modelBuilder.Entity<CompanyMaster>(entity =>
            {
                entity.HasKey(e => e.CompanyId);

                entity.Property(e => e.CompanyId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(256);

                entity.Property(e => e.CompanyLogo).HasColumnType("image");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PhoneNo).HasMaxLength(50);
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

            modelBuilder.Entity<ProductMaster>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.ProductName).HasMaxLength(50);
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

            modelBuilder.Entity<StateMaster>(entity =>
            {
                entity.HasKey(e => e.StateId);

                entity.Property(e => e.StateId).HasColumnName("StateID");

                entity.Property(e => e.StateName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TargetMaster>(entity =>
            {
                entity.HasKey(e => e.TagetId)
                    .HasName("PK_TargetMaster_1");

                entity.HasIndex(e => new { e.TelecallerId, e.MonthYear })
                    .HasName("UK_TargetMaster")
                    .IsUnique();

                entity.Property(e => e.TagetId).HasColumnName("TagetID");

                entity.Property(e => e.MonthYear).HasColumnType("date");

                entity.Property(e => e.TelecallerId).HasColumnName("TelecallerID");

                entity.HasOne(d => d.Telecaller)
                    .WithMany(p => p.TargetMaster)
                    .HasForeignKey(d => d.TelecallerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TargetMaster_UserMaster");
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

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.UserMaster)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserMaster_CompanyMaster");

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
