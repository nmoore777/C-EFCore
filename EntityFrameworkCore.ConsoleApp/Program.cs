using EntityFrameworkCore.Data;
using EntityFrameworkCore.Domain;

namespace EntityFrameworkCore.ConsoleAppv
{
    internal class Program
    {
        private static FootballLeageDbContext context = new FootballLeageDbContext();
        static async Task Main(string[] args)
        {
            var league = new League { Name = "Red Premiere League" };
            
           // await AddNewLeague(league);
           // await AddTeamsWithLeagueId(league);


            Console.WriteLine("Press any key to end...");
            Console.ReadKey();
        }

        static async Task AddNewLeague(League league)
        {
            if(context.Leagues.)
            await context.Leagues.AddAsync(league); //define db context - table to interact with - option (object) -- ac tion happens in memory
            await context.SaveChangesAsync(); //generate sql and execute action on db
        }

        static async Task AddTeamsWithLeagueId(League league)
        {
            var teams = new List<Team> { 
            new Team
            {
                Name = "A",
                LeagueId = league.Id
            },
            new Team
            {
                Name = "B",
                LeagueId = league.Id
            },
            new Team
            {
                Name = "C",
                LeagueId = league.Id
            }
            };
            await context.AddRangeAsync(teams);
            await context.SaveChangesAsync(); //generate sql and execute action on db
        }
    }
}