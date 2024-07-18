using Microsoft.EntityFrameworkCore;
using P02_FootballBetting.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data
{
    public class FootballBettingContext:DbContext
    {
        private const string ConnectionString = "Server=DESKTOP-8Q2KJ1F\\SQLEXPRESS;Database=FootballBookmakerSystem;Integrated Security=True";
        public FootballBettingContext(DbContextOptions dbcontextoptions) : base(dbcontextoptions)
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<PlayerStatistic> PlayersStatistics { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Bet> Bets { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConnectionString);
            }

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Decimal VALUES

            modelBuilder.Entity<Bet>()
                .Property(b => b.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Game>()
                .Property(g => g.AwayTeamBetRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Game>()
                .Property(g => g.DrawBetRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Game>()
                .Property(g => g.HomeTeamBetRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Team>()
                .Property(t => t.Budget)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<User>()
                .Property(u => u.Balance)
                .HasColumnType("decimal(18,2)");

            //GAME
            modelBuilder.Entity<Game>()
            .HasOne(g => g.HomeTeam)
            .WithMany(t => t.HomeGames)
            .HasForeignKey(g => g.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Game>()
                .HasOne(g => g.AwayTeam)
                .WithMany(t => t.AwayGames)
                .HasForeignKey(g => g.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            //PLAYErStATISTICS

            modelBuilder.Entity<PlayerStatistic>()
           .HasKey(ps => new { ps.PlayerId, ps.GameId });

            modelBuilder.Entity<PlayerStatistic>()
                .HasOne(ps => ps.Player)
                .WithMany(p => p.PlayersStatistics)
                .HasForeignKey(ps => ps.PlayerId);

            modelBuilder.Entity<PlayerStatistic>()
                .HasOne(ps => ps.Game)
                .WithMany(g => g.PlayersStatistics)
                .HasForeignKey(ps => ps.GameId);


            //BET

            modelBuilder.Entity<Bet>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bets)
            .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Bet>()
                .HasOne(b => b.Game)
                .WithMany(g => g.Bets)
                .HasForeignKey(b => b.GameId);

            //Color

            modelBuilder.Entity<Team>()
               .HasOne(t => t.PrimaryKitColor)
               .WithMany(c => c.PrimaryKitTeams)
               .HasForeignKey(t => t.PrimaryKitColorId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Team>()
                .HasOne(t => t.SecondaryKitColor)
                .WithMany(c => c.SecondaryKitTeams)
                .HasForeignKey(t => t.SecondaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);
            // Town
            modelBuilder.Entity<Player>()
       .HasOne(p => p.Town)
       .WithMany(t => t.Players)
       .HasForeignKey(p => p.TownId)
       .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
