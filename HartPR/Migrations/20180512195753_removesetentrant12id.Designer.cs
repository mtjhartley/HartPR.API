﻿// <auto-generated />
using HartPR.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace HartPR.Migrations
{
    [DbContext(typeof(HartPRContext))]
    [Migration("20180512195753_removesetentrant12id")]
    partial class removesetentrant12id
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HartPR.Entities.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<int>("SggPlayerId");

                    b.Property<string>("State")
                        .IsRequired();

                    b.Property<string>("Tag")
                        .IsRequired();

                    b.Property<double>("Trueskill");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("HartPR.Entities.Set", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<Guid>("LoserId");

                    b.Property<Guid>("TournamentId");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.Property<Guid>("WinnerId");

                    b.HasKey("Id");

                    b.HasIndex("TournamentId");

                    b.ToTable("Sets");
                });

            modelBuilder.Entity("HartPR.Entities.Tournament", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<DateTimeOffset>("Date");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("SggTournamentId");

                    b.Property<string>("Subdomain");

                    b.Property<string>("URL")
                        .IsRequired();

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.Property<string>("Website")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Tournaments");
                });

            modelBuilder.Entity("HartPR.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<bool>("IsAdmin");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HartPR.Entities.Set", b =>
                {
                    b.HasOne("HartPR.Entities.Tournament", "Tournament")
                        .WithMany()
                        .HasForeignKey("TournamentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
