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

    public virtual DbSet<Gepjarmu> Gepjarmus { get; set; }

    public virtual DbSet<Kikuldottjarmu> Kikuldottjarmus { get; set; }

    public virtual DbSet<Kikuldte> Kikuldtes { get; set; }

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
        modelBuilder.Entity<Gepjarmu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("gepjarmu");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Marka).HasMaxLength(16);
            entity.Property(e => e.Rendszam).HasMaxLength(8);
            entity.Property(e => e.Tipus).HasMaxLength(16);
            entity.Property(e => e.Ulesek).HasColumnType("int(11)");
        });

        modelBuilder.Entity<Kikuldottjarmu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("kikuldottjarmu");

            entity.HasIndex(e => e.GepjarmuId, "GepjarmuId");

            entity.HasIndex(e => e.KikuldetesId, "KikuldetesId");

            entity.HasIndex(e => e.Sofor, "Sofor");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.GepjarmuId).HasColumnType("int(11)");
            entity.Property(e => e.KikuldetesId).HasColumnType("int(11)");
            entity.Property(e => e.Sofor).HasColumnType("int(11)");

            entity.HasOne(d => d.Gepjarmu).WithMany(p => p.Kikuldottjarmus)
                .HasForeignKey(d => d.GepjarmuId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("kikuldottjarmu_ibfk_2");

            entity.HasOne(d => d.Kikuldetes).WithMany(p => p.Kikuldottjarmus)
                .HasForeignKey(d => d.KikuldetesId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("kikuldottjarmu_ibfk_3");

            entity.HasOne(d => d.SoforNavigation).WithMany(p => p.Kikuldottjarmus)
                .HasForeignKey(d => d.Sofor)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("kikuldottjarmu_ibfk_1");
        });

        modelBuilder.Entity<Kikuldte>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("kikuldtes");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Befejezes).HasColumnType("datetime");
            entity.Property(e => e.Celja).HasColumnType("text");
            entity.Property(e => e.Cim).HasMaxLength(128);
            entity.Property(e => e.Kezdes).HasColumnType("datetime");
        });

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
