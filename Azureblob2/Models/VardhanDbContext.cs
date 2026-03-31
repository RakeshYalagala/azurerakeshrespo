using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Azureblob2.Models;

public partial class VardhanDbContext : DbContext
{
    public VardhanDbContext()
    {
    }

    public VardhanDbContext(DbContextOptions<VardhanDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UserMaster> UserMasters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=vardhanserver.database.windows.net;Database=vardhanDB;User Id=vardhanadmin;Password=lUCKY@16;TrustServerCertificate=True;MultipleActiveResultSets=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserMaster>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("USER_MASTER_PK");

            entity.ToTable("USER_MASTER");

            entity.HasIndex(e => e.UserEmail, "UQ__USER_MAS__43CA3168FAC93D73").IsUnique();

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USER_ID");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("CREATED_ON");
            entity.Property(e => e.FailedAttempts).HasColumnName("FAILED_ATTEMPTS");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsBlocked).HasColumnName("IS_BLOCKED");
            entity.Property(e => e.LastFailedAttempt)
                .HasColumnType("datetime")
                .HasColumnName("LAST_FAILED_ATTEMPT");
            entity.Property(e => e.LockoutEndTime)
                .HasColumnType("datetime")
                .HasColumnName("LOCKOUT_END_TIME");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MODIFIED_BY");
            entity.Property(e => e.ModifiedOn)
                .HasColumnType("datetime")
                .HasColumnName("MODIFIED_ON");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.RepMgrtoken)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REP_MGRTOKEN");
            entity.Property(e => e.UserContactNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("USER_CONTACT_NO");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USER_EMAIL");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USER_NAME");
            entity.Property(e => e.UserType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("AD")
                .HasColumnName("USER_TYPE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
