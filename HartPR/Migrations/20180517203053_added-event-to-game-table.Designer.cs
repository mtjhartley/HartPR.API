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
    [Migration("20180517203053_added-event-to-game-table")]
    partial class addedeventtogametable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HartPR.Entities.Character", b =>
                {
                    b.Property<byte>("Id");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("HartPR.Entities.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte>("Enum");

                    b.Property<string>("Event")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("HartPR.Entities.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("FirstName");

                    b.Property<DateTimeOffset?>("LastActive");

                    b.Property<string>("LastName");

                    b.Property<int>("SggPlayerId");

                    b.Property<string>("State")
                        .IsRequired();

                    b.Property<string>("Tag")
                        .IsRequired();

                    b.Property<double>("Trueskill");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.Property<Guid?>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("HartPR.Entities.Set", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<Guid>("LoserId");

                    b.Property<int?>("LoserScore");

                    b.Property<Guid>("TournamentId");

                    b.Property<DateTimeOffset>("UpdatedAt");

                    b.Property<Guid>("WinnerId");

                    b.Property<int?>("WinnerScore");

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

            modelBuilder.Entity("HartPR.Entities.TrueskillHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("PlayerId");

                    b.Property<Guid>("TournamentId");

                    b.Property<double>("Trueskill");

                    b.HasKey("Id");

                    b.ToTable("TrueskillHistories");
                });

            modelBuilder.Entity("HartPR.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte>("Character");

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

                    b.Property<string>("Twitter");

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
