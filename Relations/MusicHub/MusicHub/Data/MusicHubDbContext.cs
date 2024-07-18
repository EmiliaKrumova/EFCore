using Microsoft.EntityFrameworkCore;
using MusicHub.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicHub.Data
{
    public class MusicHubDbContext : DbContext
    {

        private const string ConnectionString = "Server=DESKTOP-8Q2KJ1F\\SQLEXPRESS;Database=MusicHub;Integrated Security=True";

        //public MusicHubDbContext(DbContextOptions dbContextOptions)
        //    : base(dbContextOptions)
        //{

        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConnectionString);
            }

        }

        public DbSet<Producer> Producers { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Performer> Performers { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<SongPerformer> SongsPerformers { get; set; }
        public DbSet<Writer> Writers { get; set; }

        // Config for composite Key in Mapping table

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<SongPerformer>()
                .HasKey(sp => new
                {
                     sp.SongId,
                     sp.PerformerId
                });
        }
    }
        
}
