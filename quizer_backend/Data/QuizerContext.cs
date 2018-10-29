using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Entities.QuizObjectVersion;

namespace quizer_backend.Data {
    public class QuizerContext : DbContext {

        public DbSet<QuizLink> QuizLinks { get; set; }

        public DbSet<QuizAccess> QuizAccessItems { get; set; }

        public DbSet<Quiz> QuizItems { get; set; }
        public DbSet<QuizQuestion> QuizQuestionItems { get; set; }
        public DbSet<QuizQuestionAnswer> QuizQuestionAnswerItems { get; set; }
        
        public DbSet<QuizQuestionVersion> QuizQuestionVersionItems { get; set; }
        public DbSet<QuizQuestionAnswerVersion> QuizQuestionAnswerVersionItems { get; set; }

        public DbSet<SolvingQuiz> SolvingQuizItems { get; set; }
        public DbSet<SolvingQuizFinishedQuestion> SolvingQuizFinishedQuestionItems { get; set; }
        public DbSet<SolvingQuizFinishedQuestionSelectedAnswer> SolvingQuizFinishedQuestionSelectedAnswerItems { get; set; }

        public QuizerContext(DbContextOptions<QuizerContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<QuizQuestion>()
                .HasOne(s => s.Quiz)
                .WithMany(c => c.QuizQuestions)
                .HasForeignKey(s => s.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuizQuestionAnswer>()
                .HasOne(s => s.QuizQuestion)
                .WithMany(c => c.Answers)
                .HasForeignKey(s => s.QuizQuestionId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<QuizAccess>()
                .HasOne(s => s.Quiz)
                .WithMany(c => c.Creators)
                .HasForeignKey(s => s.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
            

            modelBuilder.Entity<QuizQuestionVersion>()
                .HasOne(s => s.QuizQuestion)
                .WithMany(c => c.Versions)
                .HasForeignKey(s => s.QuizQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuizQuestionAnswerVersion>()
                .HasOne(s => s.QuizQuestionAnswer)
                .WithMany(c => c.Versions)
                .HasForeignKey(s => s.QuizQuestionAnswerId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<SolvingQuiz>()
                .HasOne(s => s.Quiz)
                .WithMany()
                .HasForeignKey(s => s.QuizId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<SolvingQuizFinishedQuestion>()
                .HasOne(s => s.QuizQuestion)
                .WithMany()
                .HasForeignKey(s => s.QuizQuestionId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SolvingQuizFinishedQuestion>()
                .HasOne(q => q.SolvingQuiz)
                .WithMany(s => s.FinishedQuestions)
                .HasForeignKey(s => s.SolvingQuizId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<SolvingQuizFinishedQuestionSelectedAnswer>()
                .HasOne(s => s.QuizQuestionAnswer)
                .WithMany()
                .HasForeignKey(s => s.QuizQuestionAnswerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SolvingQuizFinishedQuestionSelectedAnswer>()
                .HasOne(s => s.FinishedQuestion)
                .WithMany(c => c.SelectedAnswers)
                .HasForeignKey(s => s.FinishedQuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
