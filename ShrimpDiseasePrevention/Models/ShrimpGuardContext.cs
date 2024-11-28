using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShrimpDiseasePrevention.Models;

public partial class ShrimpGuardContext : DbContext
{
    public ShrimpGuardContext()
    {
    }

    public ShrimpGuardContext(DbContextOptions<ShrimpGuardContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Disease> Diseases { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Prevention> Preventions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__C3B4DFAAD8BA07CD");

            entity.ToTable("Comment");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.CommentCreateAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ParentCommentId).HasColumnName("ParentCommentID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK__Comment__ParentC__43D61337");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__PostID__42E1EEFE");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__UserID__41EDCAC5");
        });

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.DiseaseId).HasName("PK__Disease__69B533A9305DF6E2");

            entity.ToTable("Disease");

            entity.Property(e => e.DiseaseId).HasColumnName("DiseaseID");
            entity.Property(e => e.DiseaseCreateAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DiseaseShortDescription).HasMaxLength(255);
            entity.Property(e => e.DiseaseTitle).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Diseases)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Disease__UserID__3493CFA7");

            entity.HasMany(d => d.Preventions).WithMany(p => p.Diseases)
                .UsingEntity<Dictionary<string, object>>(
                    "DiseasePrevention",
                    r => r.HasOne<Prevention>().WithMany()
                        .HasForeignKey("PreventionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__DiseasePr__Preve__3A4CA8FD"),
                    l => l.HasOne<Disease>().WithMany()
                        .HasForeignKey("DiseaseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__DiseasePr__Disea__395884C4"),
                    j =>
                    {
                        j.HasKey("DiseaseId", "PreventionId").HasName("PK__DiseaseP__896E5ABA2E6969C0");
                        j.ToTable("DiseasePrevention");
                        j.IndexerProperty<int>("DiseaseId").HasColumnName("DiseaseID");
                        j.IndexerProperty<int>("PreventionId").HasColumnName("PreventionID");
                    });
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Image__7516F4ECEAB24B6D");

            entity.ToTable("Image");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.DiseaseId).HasColumnName("DiseaseID");
            entity.Property(e => e.ImageCreateAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ImagePath).HasMaxLength(255);
            entity.Property(e => e.NewsId).HasColumnName("NewsID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.PreventionId).HasColumnName("PreventionID");

            entity.HasOne(d => d.Disease).WithMany(p => p.Images)
                .HasForeignKey(d => d.DiseaseId)
                .HasConstraintName("FK__Image__DiseaseID__489AC854");

            entity.HasOne(d => d.News).WithMany(p => p.Images)
                .HasForeignKey(d => d.NewsId)
                .HasConstraintName("FK__Image__NewsID__47A6A41B");

            entity.HasOne(d => d.Post).WithMany(p => p.Images)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__Image__PostID__4A8310C6");

            entity.HasOne(d => d.Prevention).WithMany(p => p.Images)
                .HasForeignKey(d => d.PreventionId)
                .HasConstraintName("FK__Image__Preventio__498EEC8D");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.NewsId).HasName("PK__News__954EBDD3998D0E79");

            entity.Property(e => e.NewsId).HasColumnName("NewsID");
            entity.Property(e => e.NewsCreateAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NewsShortDescription).HasMaxLength(300);
            entity.Property(e => e.NewsTitle).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.News)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__News__UserID__30C33EC3");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Post__AA12603839DC8ACE");

            entity.ToTable("Post");

            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.PostCreateAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PostTitle).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Post__UserID__3E1D39E1");
        });

        modelBuilder.Entity<Prevention>(entity =>
        {
            entity.HasKey(e => e.PreventionId).HasName("PK__Preventi__0DB6913FF54DC0EA");

            entity.ToTable("Prevention");

            entity.Property(e => e.PreventionId).HasColumnName("PreventionID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACBA794345");

            entity.ToTable("User");

            entity.HasIndex(e => e.UserName, "UQ__User__C9F28456AA844A31").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserCreateAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UserFullName).HasMaxLength(50);
            entity.Property(e => e.UserName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.UserPassword)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__RoleID__2CF2ADDF");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__UserRole__8AFACE3A64F14B31");

            entity.ToTable("UserRole");

            entity.HasIndex(e => e.RoleName, "UQ__UserRole__8A2B616028D89BD7").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
