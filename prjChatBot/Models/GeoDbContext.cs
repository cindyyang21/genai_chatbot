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

    public virtual DbSet<BotName> BotNames { get; set; }

    public virtual DbSet<ChatbotIcon> ChatbotIcons { get; set; }

    public virtual DbSet<CloseIcon> CloseIcons { get; set; }

    public virtual DbSet<ColorSelection> ColorSelections { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<InitialMessage> InitialMessages { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<PieChartDatum> PieChartData { get; set; }

    public virtual DbSet<ProductCard> ProductCards { get; set; }

    public virtual DbSet<RefreshIcon> RefreshIcons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BotName>(entity =>
        {
            entity.ToTable("BotName");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<ChatbotIcon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatbotI__3214EC071E377D75");

            entity.ToTable("ChatbotIcon");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Picture)
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<CloseIcon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CloseIco__3214EC0737C0BFB1");

            entity.ToTable("CloseIcon");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Picture)
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<ColorSelection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ColorSel__3214EC07B9A2205B");

            entity.ToTable("ColorSelection");

            entity.Property(e => e.ColorCode)
                .HasMaxLength(7)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC07AF79CC4F");

            entity.ToTable("Feedback");

            entity.Property(e => e.ChatbotMessage).UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.FeedbackType)
                .HasMaxLength(10)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.OtherReason).UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Reasons).UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.SubmittedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<InitialMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__InitialM__3214EC07C7E201B6");

            entity.ToTable("InitialMessage");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message)
                .IsRequired()
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC07376B82BB");

            entity.ToTable("Menu");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageFileName)
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.TextContent)
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<PieChartDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PieChart__3214EC07577C12E5");

            entity.Property(e => e.Reason).UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<ProductCard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductC__3214EC07E95A8E3F");

            entity.ToTable("ProductCard");

            entity.Property(e => e.ImageFileName)
                .IsRequired()
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Name1)
                .IsRequired()
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Name2)
                .IsRequired()
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Url1)
                .IsRequired()
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Url2)
                .IsRequired()
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<RefreshIcon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshI__3214EC07E57F4BCC");

            entity.ToTable("RefreshIcon");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Picture)
                .HasMaxLength(255)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}