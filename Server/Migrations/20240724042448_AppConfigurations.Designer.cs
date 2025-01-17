﻿// <auto-generated />
using System;
using DarkPatterns.OneTimePassword.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DarkPatterns.OneTimePassword.Migrations
{
    [DbContext(typeof(OtpDbContext))]
    [Migration("20240724042448_AppConfigurations")]
    partial class AppConfigurations
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DarkPatterns.OneTimePassword.Persistence.ConfiguredApplication", b =>
                {
                    b.Property<Guid>("ConfigurationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("uuid");

                    b.HasKey("ConfigurationId", "ApplicationId");

                    b.HasIndex("ApplicationId");

                    b.ToTable("ConfiguredApplications");
                });

            modelBuilder.Entity("DarkPatterns.OneTimePassword.Persistence.DeliveredOneTimePassword", b =>
                {
                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("uuid");

                    b.Property<string>("MediumCode")
                        .HasColumnType("text");

                    b.Property<string>("DeliveryTarget")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("ExpirationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<int>("RemainingCount")
                        .HasColumnType("integer");

                    b.HasKey("ApplicationId", "MediumCode", "DeliveryTarget");

                    b.ToTable("DeliveredPasswords");
                });

            modelBuilder.Entity("DarkPatterns.OneTimePassword.Persistence.OtpApplication", b =>
                {
                    b.Property<Guid>("ApplicationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("ApplicationId");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("DarkPatterns.OneTimePassword.Persistence.OtpConfiguration", b =>
                {
                    b.Property<Guid>("ConfigurationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<TimeSpan>("ExpirationWindow")
                        .HasColumnType("interval");

                    b.Property<bool>("IsSliding")
                        .HasColumnType("boolean");

                    b.Property<int>("MaxAttemptCount")
                        .HasColumnType("integer");

                    b.HasKey("ConfigurationId");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("DarkPatterns.OneTimePassword.Persistence.ConfiguredApplication", b =>
                {
                    b.HasOne("DarkPatterns.OneTimePassword.Persistence.OtpApplication", "Application")
                        .WithMany()
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DarkPatterns.OneTimePassword.Persistence.OtpConfiguration", "Configuration")
                        .WithMany()
                        .HasForeignKey("ConfigurationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Application");

                    b.Navigation("Configuration");
                });

            modelBuilder.Entity("DarkPatterns.OneTimePassword.Persistence.DeliveredOneTimePassword", b =>
                {
                    b.HasOne("DarkPatterns.OneTimePassword.Persistence.OtpApplication", "Application")
                        .WithMany()
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Application");
                });
#pragma warning restore 612, 618
        }
    }
}
