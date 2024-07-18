using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext :DbContext
    {
        private const string ConnectionString = "Server=DESKTOP-8Q2KJ1F\\SQLEXPRESS;Database=StudentSystem;Integrated Security=True";

        public StudentSystemContext(DbContextOptions dbContextOptions):base(dbContextOptions)
        {
            
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }

        public DbSet<StudentCourse> StudentsCourses { get; set; }

        public DbSet<Resource> Resources { get; set; }
        public DbSet<Homework> Homeworks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) 
            { 
                optionsBuilder.UseSqlServer(ConnectionString);
            }
            
        }

        // Config for composite Key in Mapping table
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc=> new
                {
                    sc.StudentId,
                    sc.CourseId
                });
            modelBuilder.Entity<Student>()
                .Property(s => s.PhoneNumber)
                .IsUnicode(false);
            modelBuilder.Entity<Resource>()
                .Property(r => r.Url)
                .IsUnicode(false);
            modelBuilder.Entity<Homework>()
                .Property(h => h.Content)
                .IsUnicode(false);
            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsFixedLength(true)
                    .IsUnicode(false)
                    .IsRequired(false); // Или просто не го включвайте, тъй като по подразбиране свойството е не е задължително
            });
        }
    }
}
