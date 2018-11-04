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
    [Migration("20181103155755_intnotlong")]
    partial class intnotlong
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("quizer_backend.Data.Entities.LearningQuiz.LearningQuiz", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreationTime");

                    b.Property<long?>("FinishedTime");

                    b.Property<bool>("IsFinished");

                    b.Property<long>("LearningTime");

                    b.Property<long>("NumberOfBadAnswers");

                    b.Property<long>("NumberOfCorrectAnswers");

                    b.Property<long>("NumberOfLearnedQuestions");

                    b.Property<long>("NumberOfQuestions");

                    b.Property<long?>("QuizId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("QuizId");

                    b.ToTable("LearningQuizzes");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.LearningQuiz.LearningQuizQuestion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("BadUserAnswers");

                    b.Property<long>("GoodUserAnswers");

                    b.Property<long>("LearningQuizId");

                    b.Property<long>("QuestionId");

                    b.Property<long>("Reoccurrences");

                    b.HasKey("Id");

                    b.HasIndex("LearningQuizId");

                    b.HasIndex("QuestionId");

                    b.ToTable("LearningQuizQuestions");
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

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizObject.Answer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreationTime");

                    b.Property<bool>("IsDeleted");

                    b.Property<long>("QuestionId");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizObject.AnswerVersion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreationTime");

                    b.Property<bool>("IsCorrect");

                    b.Property<long>("QuizQuestionAnswerId");

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("QuizQuestionAnswerId");

                    b.ToTable("AnswerVersions");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizObject.Question", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreationTime");

                    b.Property<bool>("IsDeleted");

                    b.Property<long>("QuizId");

                    b.HasKey("Id");

                    b.HasIndex("QuizId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizObject.QuestionVersion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreationTime");

                    b.Property<long>("QuizQuestionId");

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("QuizQuestionId");

                    b.ToTable("QuestionVersions");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizObject.Quiz", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CreationTime");

                    b.Property<long>("LastModifiedTime");

                    b.Property<int?>("MinutesInSolvingQuiz");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("OwnerId");

                    b.Property<int?>("QuestionsInSolvingQuiz");

                    b.HasKey("Id");

                    b.ToTable("Quizzes");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.UserSettings", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("MaxReoccurrences");

                    b.Property<long>("ReoccurrencesIfBad");

                    b.Property<long>("ReoccurrencesOnStart");

                    b.HasKey("UserId");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.LearningQuiz.LearningQuiz", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.QuizObject.Quiz", "Quiz")
                        .WithMany()
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.LearningQuiz.LearningQuizQuestion", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.LearningQuiz.LearningQuiz", "LearningQuiz")
                        .WithMany("LearningQuizQuestions")
                        .HasForeignKey("LearningQuizId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("quizer_backend.Data.Entities.QuizObject.Question", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizAccess", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.QuizObject.Quiz", "Quiz")
                        .WithMany("Creators")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizObject.Answer", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.QuizObject.Question", "Question")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizObject.AnswerVersion", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.QuizObject.Answer", "Answer")
                        .WithMany("Versions")
                        .HasForeignKey("QuizQuestionAnswerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizObject.Question", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.QuizObject.Quiz", "Quiz")
                        .WithMany("Questions")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("quizer_backend.Data.Entities.QuizObject.QuestionVersion", b =>
                {
                    b.HasOne("quizer_backend.Data.Entities.QuizObject.Question", "Question")
                        .WithMany("Versions")
                        .HasForeignKey("QuizQuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
