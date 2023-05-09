using EntityFrameworkCore.Data;
using EntityFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Identity.Client;

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

            // await SelectLeagues();
            // await QueryFilter();
            // await AdditionalExecutionMethods();
            await altLINQSyntax();

            Console.WriteLine("Press any key to end...");
            Console.ReadKey();
        }

        static async Task AddNewLeague(League league)
        {
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


        static async Task SelectLeagues()
        {
            var leagues = await context.Leagues.ToListAsync();

            foreach (League league in leagues)
            {
                Console.WriteLine($"{league.Id} - {league.Name}");
            }
        }

        static async Task QueryFilter()
        {
            Console.Write($"Enter the League Name: ");
            var leagueName = Console.ReadLine();
            var leagueContains = await context.Leagues.Where(q => q.Name.Contains(leagueName)).ToListAsync(); //populate a list with any leagues that contain the users input as a substring -- ignores case
            var leagueContainsOpt2 = await context.Leagues.Where(q => EF.Functions.Like(q.Name, $"%{leagueName}%")).ToListAsync(); //populate a list with any leagues that contain the users input as a substring using a more sql modolus based query -- ignores case
            var leagueIs = await context.Leagues.Where(q => q.Name == leagueName).ToListAsync(); //populate a list with any leagues that contain the users input is equal to string

            foreach (League league in leagueIs)
            {
                Console.WriteLine($"is = {league.Id} - {league.Name}");
            }

            foreach (League league in leagueContains)
            {
                Console.WriteLine($"contains = {league.Id} - {league.Name}");
            }
            foreach (League league in leagueContainsOpt2)
            {
                Console.WriteLine($"opt2 = {league.Id} - {league.Name}");
            }
        }

        static async Task AdditionalExecutionMethods()
        {
            var leagues = context.Leagues;
            //execute search for populating list with leagues which contain a specified char as first char
            var first = await leagues.Where(q => q.Name.Contains("A")).FirstOrDefaultAsync();
            var first2 = await leagues.FirstOrDefaultAsync(q => q.Name.Contains("A"));

            
            //execute search expecting only one record to return true if more than one record will return default
                // var single = await leagues.SingleAsync();
            //execute search expecting only one record to return true if more than one record will return error
                //var single2 = await leagues.SingleOrDefaultAsync();

            //traditional aggregate f(x)
            var count = await leagues.CountAsync();
            var longCount = await leagues.LongCountAsync();
            var min = await leagues.MinAsync();
            var max = await leagues.MaxAsync();

            var league = await leagues.FindAsync(1);
        }

        //other way to write queries in other than lambda expressions and more like sql
        static async Task altLINQSyntax()
        {
            Console.Write("Input Team Name: ");
            var teamName = Console.ReadLine();
            var teams = await (from i in context.Teams
                               where i.Name.Contains("A")
                               select i).ToListAsync();

            foreach (var team in teams)
            {
                Console.WriteLine($"{team.Id} - {team.Name}");
            }
        }
    }
}