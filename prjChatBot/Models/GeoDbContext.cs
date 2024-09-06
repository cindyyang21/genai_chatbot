﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace prjChatBot.Models;

public partial class GeoDbContext : DbContext
{
    public GeoDbContext(DbContextOptions<GeoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Icon> Icons { get; set; }

    public virtual DbSet<InitialMessage> InitialMessages { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<ProductCard> ProductCards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3214EC077588A279");

            entity.ToTable("Feedback");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Option)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<Icon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC07856C6BB4");

            entity.ToTable("Icon");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Picture).HasMaxLength(255);
        });

        modelBuilder.Entity<InitialMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__InitialM__3214EC07C7E201B6");

            entity.ToTable("InitialMessage");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message).IsRequired();
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC07376B82BB");

            entity.ToTable("Menu");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageFileName).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.TextContent).HasMaxLength(255);
        });

        modelBuilder.Entity<ProductCard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductC__3214EC07E95A8E3F");

            entity.ToTable("ProductCard");

            entity.Property(e => e.ImageFileName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Name1)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Name2)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Url1)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Url2)
                .IsRequired()
                .HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}