using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Domain;

public class ApplicationDbContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Semester> Semesters { get; set; }
    public DbSet<SubjectAssignment> SubjectAssignments { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Homework> Homeworks { get; set; }
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // enum Role → string
    modelBuilder.Entity<User>()
        .Property(u => u.Role)
        .HasConversion<string>();

    // --- Optional поля ---
    modelBuilder.Entity<Student>()
        .Property(s => s.MiddleName)
        .IsRequired(false);

    modelBuilder.Entity<Teacher>()
        .Property(t => t.MiddleName)
        .IsRequired(false);

    // --- User ↔ Student / Teacher ---
    modelBuilder.Entity<User>()
        .HasOne(u => u.Student)
        .WithOne()
        .HasForeignKey<User>(u => u.StudentId)
        .OnDelete(DeleteBehavior.SetNull);

    modelBuilder.Entity<User>()
        .HasOne(u => u.Teacher)
        .WithOne()
        .HasForeignKey<User>(u => u.TeacherId)
        .OnDelete(DeleteBehavior.SetNull);

    modelBuilder.Entity<User>()
        .HasIndex(u => u.Username)
        .IsUnique();

    // --- Group ↔ Student (1:M) ---
    modelBuilder.Entity<Student>()
        .HasOne(s => s.Group)
        .WithMany(g => g.Students)
        .HasForeignKey(s => s.GroupId)
        .OnDelete(DeleteBehavior.Restrict);

    // --- Group ↔ SubjectAssignment (1:M) ---
    modelBuilder.Entity<SubjectAssignment>()
        .HasOne(sa => sa.Group)
        .WithMany(g => g.SubjectAssignments)
        .HasForeignKey(sa => sa.GroupId)
        .OnDelete(DeleteBehavior.Restrict);

    // --- Teacher ↔ SubjectAssignment (1:M) ---
    modelBuilder.Entity<SubjectAssignment>()
        .HasOne(sa => sa.Teacher)
        .WithMany(t => t.SubjectAssignments)
        .HasForeignKey(sa => sa.TeacherId)
        .OnDelete(DeleteBehavior.Restrict);

    // --- Subject ↔ SubjectAssignment (1:M) ---
    modelBuilder.Entity<SubjectAssignment>()
        .HasOne(sa => sa.Subject)
        .WithMany(s => s.SubjectAssignments)
        .HasForeignKey(sa => sa.SubjectId)
        .OnDelete(DeleteBehavior.Restrict);

    // --- Semester ↔ SubjectAssignment (1:M) ---
    modelBuilder.Entity<SubjectAssignment>()
        .HasOne(sa => sa.Semester)
        .WithMany(s => s.SubjectAssignments)
        .HasForeignKey(sa => sa.SemesterId)
        .OnDelete(DeleteBehavior.Restrict);

    // --- SubjectAssignment ↔ Lesson (1:M) ---
    modelBuilder.Entity<Lesson>()
        .HasOne(l => l.SubjectAssignment)
        .WithMany(sa => sa.Lessons)
        .HasForeignKey(l => l.SubjectAssignmentId)
        .OnDelete(DeleteBehavior.Cascade);

    // --- Lesson ↔ Grade (1:M) ---
    modelBuilder.Entity<Grade>()
        .HasOne(g => g.Lesson)
        .WithMany(l => l.Grades)
        .HasForeignKey(g => g.LessonId)
        .OnDelete(DeleteBehavior.Cascade);

    // --- Lesson ↔ Attendance (1:M) ---
    modelBuilder.Entity<Attendance>()
        .HasOne(a => a.Lesson)
        .WithMany(l => l.AttendanceRecords)
        .HasForeignKey(a => a.LessonId)
        .OnDelete(DeleteBehavior.Cascade);

    // --- Lesson ↔ Homework (1:M) ---
    modelBuilder.Entity<Homework>()
        .HasOne(h => h.Lesson)
        .WithMany(l => l.Homeworks)
        .HasForeignKey(h => h.LessonId)
        .OnDelete(DeleteBehavior.Cascade);

    // --- Student ↔ Grade (1:M) ---
    modelBuilder.Entity<Grade>()
        .HasOne(g => g.Student)
        .WithMany(s => s.Grades)
        .HasForeignKey(g => g.StudentId)
        .OnDelete(DeleteBehavior.Cascade);

    // --- Student ↔ Attendance (1:M) ---
    modelBuilder.Entity<Attendance>()
        .HasOne(a => a.Student)
        .WithMany(s => s.AttendanceRecords)
        .HasForeignKey(a => a.StudentId)
        .OnDelete(DeleteBehavior.Cascade);

    // --- Индексы для уникальности ---
    modelBuilder.Entity<Group>()
        .HasIndex(g => new { g.Name, g.SpecialtyCode })
        .IsUnique();

    base.OnModelCreating(modelBuilder);
}

}
