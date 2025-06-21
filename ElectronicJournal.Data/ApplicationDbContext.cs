using ElectronicJournal.Domain.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Domain;

public class ApplicationDbContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<SubjectAssignment> SubjectAssignments { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Assessment> Assessments { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Homework> Homeworks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Schedule> Schedules { get; set; }

    /*public ApplicationDbContext(string connectionString) : this(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseNpgsql(connectionString)
        .Options)
    {
    }*/

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /*public ApplicationDbContext() : this(new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseNpgsql("User ID=postgres;Password=12345;Host=localhost;Port=5432;Database=postgres;")
        .Options)
    {
    }*/


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

        // --- User ↔ Student ---
        modelBuilder.Entity<Student>()
            .HasOne(s => s.User)
            .WithOne(u => u.Student)
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- User ↔ Teacher ---
        modelBuilder.Entity<Teacher>()
            .HasOne(t => t.User)
            .WithOne(u => u.Teacher)
            .HasForeignKey<Teacher>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- User.UserName уникален ---
        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
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

        // --- SubjectAssignment ↔ Lesson (1:M) ---
        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.SubjectAssignment)
            .WithMany(sa => sa.Lessons)
            .HasForeignKey(l => l.SubjectAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Lesson ↔ Grade (1:M) ---
        modelBuilder.Entity<Assessment>()
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
        modelBuilder.Entity<Assessment>()
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
        modelBuilder.Entity<Schedule>()
            .Property(se => se.Day)
            .HasConversion<string>();

        // --- SubjectAssignment ↔ ScheduleEntry (1:M) ---
        modelBuilder.Entity<Schedule>()
            .HasOne(se => se.SubjectAssignment)
            .WithMany(sa => sa.Schedules)
            .HasForeignKey(se => se.SubjectAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Assessment>()
            .HasIndex(a => new { a.LessonId, a.StudentId })
            .IsUnique();


        base.OnModelCreating(modelBuilder);
    }
}