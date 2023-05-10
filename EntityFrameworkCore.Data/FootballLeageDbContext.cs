using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using EntityFrameworkCore.Domain.Views;
namespace EntityFrameworkCore.Data
{
    public class FootballLeageDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=FootballLeague_EfCore; Encrypt=False")
                .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        //overriding model creation to give custom rules for many-to-many relationship between teams and matches
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .HasMany(m => m.HomeMatches)
                .WithOne(m => m.AwayTeam)
                .HasForeignKey(m => m.HomeTeamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); //cannot remove team unless removed matches before
            modelBuilder.Entity<Team>()
                .HasMany(m => m.AwayMatches)
                .WithOne(m => m.HomeTeam)
                .HasForeignKey(m => m.AwayTeamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);//cannot remove team unless removed matches before

            modelBuilder.Entity<TeamsCoachesLeaguesView>().HasNoKey().ToView("TeamsCoachesLeagues");

        }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Match> Matches { get; set; }

        public DbSet<TeamsCoachesLeaguesView> teamsCoachesLeagues { get; set; }

    }
}
