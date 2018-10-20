using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;

namespace quizer_backend.Data {
    public class QuizerContext : DbContext {

        public DbSet<QuizItem> QuizItems { get; set; }
        public DbSet<QuizQuestionItem> QuizQuestionItems { get; set; }
        public DbSet<QuizQuestionAnswerItem> QuizQuestionAnswerItems { get; set; }

        public QuizerContext(DbContextOptions<QuizerContext> options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<QuizQuestionItem>()
                .HasOne(s => s.Quiz)
                .WithMany(c => c.QuizQuestions)
                .HasForeignKey(s => s.QuizId);

            modelBuilder.Entity<QuizQuestionAnswerItem>()
                .HasOne(s => s.QuizQuestion)
                .WithMany(c => c.Answers)
                .HasForeignKey(s => s.QuizQuestionId);
        }
    }
}
