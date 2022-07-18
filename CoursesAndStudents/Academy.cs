using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using static System.Console;

namespace CoursesAndStudents;

public class Academy : DbContext
{
    public DbSet<Student>? Students { get; set; }
    public DbSet<Course>? Courses { get; set; } 

    protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
    {
        string path = Path.Combine(Environment.CurrentDirectory, "Academy.db");

        WriteLine($"Using {path} database file.");

        // optionBuilder.USeSqlite($"Filename={path});
        optionBuilder.UseSqlServer(@"Data source=.;Initial Catalog=Academy;Integrated Security=true;MultipleActiveResultSets=true;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API validation rules
        modelBuilder.Entity<Student>().Property(s => s.LastName).HasMaxLength(30).IsRequired();

        // Populate database with sample data
        Student alice = new Student()
        {
            StudentId = 1,
            FirstName = "Alice",
            LastName = "Jones"
        };

        Student bob = new Student()
        {
            StudentId = 2,
            FirstName = "Bob",
            LastName = "Smith"
        };

        Student cecillia = new Student()
        {
            StudentId = 3,
            FirstName = "Cecilia",
            LastName = "Ramirez"
        };

        Course csharp = new Course()
        {
            CourseId = 1,
            Title = "C# 10 and .NET6"
        };

        Course webdev = new Course()
        {
            CourseId = 2,
            Title = "Web Development"
        };

        Course python = new Course()
        {
            CourseId = 3,
            Title = "Python for Beginners"
        };

        modelBuilder.Entity<Student>().HasData(alice, bob, cecillia);
        modelBuilder.Entity<Course>().HasData(csharp, webdev, python);

        modelBuilder.Entity<Course>()
            .HasMany(c => c.Students)
            .WithMany(s => s.Courses)
            .UsingEntity(e => e.HasData(
                // all students signed up for C# course
                new { CoursesCourseId = 1, StudentsStudentId = 1},
                new { CoursesCourseId = 1, StudentsStudentId = 2},
                new { CoursesCourseId = 1, StudentsStudentId = 3},
                // only Bob signed up for web dev
                new { CoursesCourseId = 2, StudentsStudentId = 2},
                // only Cecilia signed up for python
                new { CoursesCourseId = 3, StudentsStudentId = 3 }
                ));
               // many to many type relationship naming convention is : NavigationPropertyNamePropertyName
    }
}