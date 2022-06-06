using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace ClassJournal.Models
{
    public partial class ModelClassJournal : DbContext
    {
        public ModelClassJournal()
            : base("name=ModelClassJournal")
        {
        }

        public virtual DbSet<Attendance> Attendances { get; set; }
        public virtual DbSet<Audience> Audiences { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<LessonHour> LessonHours { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Speciality> Specialities { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Audience>()
                .HasMany(e => e.Lessons)
                .WithRequired(e => e.Audience)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Teachers)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasOptional(e => e.User)
                .WithRequired(e => e.Employee);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Positions)
                .WithMany(e => e.Employees)
                .Map(m => m.ToTable("EmployeePositions").MapLeftKey("EmployeeID").MapRightKey("PositionID"));

            modelBuilder.Entity<Group>()
                .HasMany(e => e.Lessons)
                .WithRequired(e => e.Group)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Group>()
                .HasMany(e => e.Students)
                .WithRequired(e => e.Group)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LessonHour>()
                .HasMany(e => e.Lessons)
                .WithRequired(e => e.LessonHour)
                .HasForeignKey(e => e.LessonNumber)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lesson>()
                .HasMany(e => e.Attendances)
                .WithRequired(e => e.Lesson)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Speciality>()
                .HasMany(e => e.Groups)
                .WithRequired(e => e.Speciality)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Speciality>()
                .HasMany(e => e.Subjects)
                .WithMany(e => e.Specialities)
                .Map(m => m.ToTable("SpecialtiesSubjects").MapLeftKey("SpecialityCode").MapRightKey("SubjectID"));

            modelBuilder.Entity<Student>()
                .HasMany(e => e.Attendances)
                .WithRequired(e => e.Student)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Subject>()
                .HasMany(e => e.Teachers)
                .WithRequired(e => e.Subject)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Teacher>()
                .HasMany(e => e.Lessons)
                .WithRequired(e => e.Teacher)
                .WillCascadeOnDelete(false);
        }
    }
}
