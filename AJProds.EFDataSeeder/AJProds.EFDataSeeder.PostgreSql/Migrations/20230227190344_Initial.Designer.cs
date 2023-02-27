﻿// <auto-generated />
using System;
using AJProds.EFDataSeeder.Core.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AJProds.EFDataSeeder.PostgreSql.Migrations
{
    [DbContext(typeof(SeederDbContext))]
    [Migration("20230227190344_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("sdr")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("AJProds.EFDataSeeder.Core.Db.SeederHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("AlwaysRun")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("FirstRunAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("LastRunAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("SeedName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("SeedName")
                        .IsUnique();

                    b.ToTable("SeederHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
