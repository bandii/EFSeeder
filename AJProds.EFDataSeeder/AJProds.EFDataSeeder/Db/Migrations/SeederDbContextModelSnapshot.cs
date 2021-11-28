﻿// <auto-generated />
using System;
using AJProds.EFDataSeeder.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AJProds.EFDataSeeder.Db.Migrations
{
    [DbContext(typeof(SeederDbContext))]
    partial class SeederDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("sdr")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AJProds.EFDataSeeder.Db.SeederHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("AlwaysRun")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FirstRunAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastRunAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("SeedName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("SeedName")
                        .IsUnique();

                    b.ToTable("SeederHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
