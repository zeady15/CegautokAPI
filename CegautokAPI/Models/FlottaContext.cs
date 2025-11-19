using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CegautokAPI.Models;

public partial class FlottaContext : DbContext
{
    public FlottaContext()
    {
    }

    public FlottaContext(DbContextOptions<FlottaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Privilege> Privileges { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json")
        .Build();
        optionsBuilder.UseMySQL(configuration.GetConnectionString("FlottaConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Privilege>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("privilege");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Level).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(64);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.HasIndex(e => e.Permission, "Permission");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Address).HasMaxLength(256);
            entity.Property(e => e.Email).HasMaxLength(64);
            entity.Property(e => e.Hash).HasMaxLength(256);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(64);
            entity.Property(e => e.Permission).HasColumnType("int(11)");
            entity.Property(e => e.Phone)
                .HasMaxLength(32)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Salt).HasMaxLength(256);

            entity.HasOne(d => d.PermissionNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Permission)
                .HasConstraintName("user_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
