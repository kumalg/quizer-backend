﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using quizer_backend.Data;

namespace quizer_backend.Migrations
{
    [DbContext(typeof(QuizerContext))]
    [Migration("20181028162756_solvingquizanothertables-nowy2")]
    partial class solvingquizanothertablesnowy2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("quizer_backend.Data.Entities.Quiz", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreationTime");

                    b.Property<long>("LastModifiedTime");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("OwnerId");

                    b.HasKey("Id");

                    b.ToTable("QuizItems");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizAccess", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Access");

                    b.Property<long>("QuizId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("QuizId");

                    b.ToTable("QuizAccessItems");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizLink", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Access");

                    b.Property<string>("Link");

                    b.Property<long>("QuizId");

                    b.HasKey("Id");

                    b.HasIndex("QuizId");

                    b.ToTable("QuizLinks");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizQuestion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreationTime");

                    b.Property<long>("QuizId");

                    b.HasKey("Id");

                    b.HasIndex("QuizId");

                    b.ToTable("QuizQuestionItems");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizQuestionAnswer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreationTime");

                    b.Property<long>("QuizQuestionId");

                    b.HasKey("Id");

                    b.HasIndex("QuizQuestionId");

                    b.ToTable("QuizQuestionAnswerItems");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizQuestionAnswerVersion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreationTime");

                    b.Property<bool>("IsCorrect");

                    b.Property<long>("QuizQuestionAnswerId");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("QuizQuestionAnswerId");

                    b.ToTable("QuizQuestionAnswerVersionItems");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizQuestionVersion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreationTime");

                    b.Property<long>("QuizQuestionId");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("QuizQuestionId");

                    b.ToTable("QuizQuestionVersionItems");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.SolvingQuiz", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreationTime");

                    b.Property<long?>("QuizId");

                    b.Property<long?>("QuizId1");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("QuizId");

                    b.HasIndex("QuizId1");

                    b.ToTable("SolvingQuizItems");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.SolvingQuizFinishedQuestion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("CorrectlyAnswered");

                    b.Property<long?>("QuizQuestionId");

                    b.Property<long>("SolvingQuizId");

                    b.HasKey("Id");

                    b.HasIndex("QuizQuestionId");

                    b.HasIndex("SolvingQuizId");

                    b.ToTable("SolvingQuizFinishedQuestionItems");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.SolvingQuizFinishedQuestionSelectedAnswer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("FinishedQuestionId");

                    b.Property<long?>("QuizQuestionAnswerId");

                    b.HasKey("Id");

                    b.HasIndex("FinishedQuestionId");

                    b.HasIndex("QuizQuestionAnswerId");

                    b.ToTable("SolvingQuizFinishedQuestionSelectedAnswerItems");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizAccess", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.Quiz", "Quiz")
                        .WithMany("Creators")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizLink", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.Quiz", "Quiz")
                        .WithMany()
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizQuestion", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.Quiz", "Quiz")
                        .WithMany("QuizQuestions")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizQuestionAnswer", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.QuizQuestion", "QuizQuestion")
                        .WithMany("Answers")
                        .HasForeignKey("QuizQuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizQuestionAnswerVersion", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.QuizQuestionAnswer", "QuizQuestionAnswer")
                        .WithMany("Versions")
                        .HasForeignKey("QuizQuestionAnswerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizQuestionVersion", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.QuizQuestion", "QuizQuestion")
                        .WithMany("Versions")
                        .HasForeignKey("QuizQuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.SolvingQuiz", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.Quiz", "Quiz")
                        .WithMany()
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("quizer_backend.Data.Entities.Quiz")
                        .WithMany("SolvingQuizes")
                        .HasForeignKey("QuizId1");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.SolvingQuizFinishedQuestion", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.QuizQuestion", "QuizQuestion")
                        .WithMany()
                        .HasForeignKey("QuizQuestionId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("quizer_backend.Data.Entities.SolvingQuiz", "SolvingQuiz")
                        .WithMany("FinishedQuestions")
                        .HasForeignKey("SolvingQuizId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.SolvingQuizFinishedQuestionSelectedAnswer", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.SolvingQuizFinishedQuestion", "FinishedQuestion")
                        .WithMany("SelectedAnswers")
                        .HasForeignKey("FinishedQuestionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("quizer_backend.Data.Entities.QuizQuestionAnswer", "QuizQuestionAnswer")
                        .WithMany()
                        .HasForeignKey("QuizQuestionAnswerId")
                        .OnDelete(DeleteBehavior.SetNull);
                });
#pragma warning restore 612, 618
        }
    }
}
