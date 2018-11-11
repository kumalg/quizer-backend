using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Entities.LearningQuiz;
using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data {
    public class QuizerContext : DbContext {

        public DbSet<QuizAccess> QuizAccessItems { get; set; }

        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        
        public DbSet<QuestionVersion> QuestionVersions { get; set; }
        public DbSet<AnswerVersion> AnswerVersions { get; set; }
        
        public DbSet<LearningQuiz> LearningQuizzes { get; set; }
        public DbSet<LearningQuizQuestion> LearningQuizQuestions { get; set; }

        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<AnonymousUser> AnonymousUsers { get; set; }

        public QuizerContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Question>()
                .HasOne(s => s.Quiz)
                .WithMany(c => c.Questions)
                .HasForeignKey(s => s.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
                .HasOne(s => s.Question)
                .WithMany(c => c.Answers)
                .HasForeignKey(s => s.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<QuizAccess>()
                .HasOne(s => s.Quiz)
                .WithMany(c => c.Creators)
                .HasForeignKey(s => s.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
            

            modelBuilder.Entity<QuestionVersion>()
                .HasOne(s => s.Question)
                .WithMany(c => c.Versions)
                .HasForeignKey(s => s.QuizQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnswerVersion>()
                .HasOne(s => s.Answer)
                .WithMany(c => c.Versions)
                .HasForeignKey(s => s.QuizQuestionAnswerId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<LearningQuiz>()
                .HasOne(s => s.Quiz)
                .WithMany()
                .HasForeignKey(s => s.QuizId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<LearningQuizQuestion>()
                .HasOne(s => s.Question)
                .WithMany()
                .HasForeignKey(s => s.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LearningQuizQuestion>()
                .HasOne(q => q.LearningQuiz)
                .WithMany(s => s.LearningQuizQuestions)
                .HasForeignKey(s => s.LearningQuizId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<UserSettings>()
                .HasKey(q => q.UserId);

            //modelBuilder.Entity<Quiz>()
            //    .HasKey(q => q.GuidId);
        }
    }
}
