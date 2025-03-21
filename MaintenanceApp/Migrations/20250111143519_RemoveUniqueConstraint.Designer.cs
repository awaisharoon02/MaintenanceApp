﻿// <auto-generated />
using System;
using MaintenanceApp.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MaintenanceApp.Migrations
{
    [DbContext(typeof(MaintenanceDbContext))]
    [Migration("20250111143519_RemoveUniqueConstraint")]
    partial class RemoveUniqueConstraint
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MaintenanceApp.Models.PhraseModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsChecked")
                        .HasColumnType("bit");

                    b.Property<string>("Phrase")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RequestMaintenanceID")
                        .HasColumnType("int");

                    b.Property<int?>("RequestMaintenanceModelId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RequestMaintenanceID");

                    b.HasIndex("RequestMaintenanceModelId");

                    b.ToTable("phraseModel");
                });

            modelBuilder.Entity("MaintenanceApp.Models.RequestMaintenanceModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ActionAcknowledgedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("ActionAcknowledgedDate")
                        .HasColumnType("date");

                    b.Property<string>("ActionApprovedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("ActionApprovedDate")
                        .HasColumnType("date");

                    b.Property<string>("ActionTakenBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("ActionTakenDate")
                        .HasColumnType("date");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DescriptionAction")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Machine")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MachineDowntime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeOnly>("MachineRunning")
                        .HasColumnType("time");

                    b.Property<string>("Proposal")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReceivedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeOnly>("ReceivedTime")
                        .HasColumnType("time");

                    b.Property<DateOnly>("RequestDate")
                        .HasColumnType("date");

                    b.Property<int>("RequestID")
                        .HasColumnType("int");

                    b.Property<string>("RequestOpen")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RequestedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("requestMaintenanceModel");
                });

            modelBuilder.Entity("MaintenanceApp.Models.PhraseModel", b =>
                {
                    b.HasOne("MaintenanceApp.Models.RequestMaintenanceModel", "RequestMaintenance")
                        .WithMany()
                        .HasForeignKey("RequestMaintenanceID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("MaintenanceApp.Models.RequestMaintenanceModel", null)
                        .WithMany("phraseModel")
                        .HasForeignKey("RequestMaintenanceModelId");

                    b.Navigation("RequestMaintenance");
                });

            modelBuilder.Entity("MaintenanceApp.Models.RequestMaintenanceModel", b =>
                {
                    b.Navigation("phraseModel");
                });
#pragma warning restore 612, 618
        }
    }
}
