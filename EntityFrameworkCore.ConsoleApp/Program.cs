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
            // await altLINQSyntax();

            await UpdateRecord();
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

        static async Task UpdateRecord()
        {
            //Decide which table to alter
            Console.Write("Are you changing a league or team name? \nleague or team: ");
            var decision = Console.ReadLine();
            var ans = "";
            var cts = new CancellationTokenSource();

            if (decision != null && decision == "team")
            {
                //Get User Input
                Console.Write("Enter Team Id to Alter: "); //if user chooses to alter a team alter Teams table
                var teamId = Console.ReadLine();
                //Retrieve Record
                try
                {
                var teamRecord = await context.Teams.FindAsync(Int32.Parse(teamId));
                
                //Update Record
                //Get the User Input for new Team Name
                Console.Write("Enter New Team Name: ");
                var newTeamName = Console.ReadLine();
                var oldTeamName = teamRecord.Name;
                teamRecord.Name = newTeamName;
                //Save Record
                await context.SaveChangesAsync();
                //Print Changes to Console
                Console.WriteLine($"Successfully altered team Id {teamId} with name of: {oldTeamName} to: ");
                await GetRecord(decision, Int32.Parse(teamId));
                }
                catch
                {
                    Console.Write("Your input was not valid please try again");
                    await UpdateRecord();
                }

                //Ask for more input
                Console.WriteLine("Would you like to alter another entry? \n y or n: ");
                ans = Console.ReadLine();
            }
            else if (decision != null && decision == "league") //if user chooses to alter a league alter Leagues table
            {
                // Get User Input
                Console.Write("Enter League Id to Alter: ");
                var leagueId = Console.ReadLine();
                //Retrieve Record
                try
                {
                    var leagueRecord = await context.Leagues.FindAsync(Int32.Parse(leagueId));
                    //Update Record
                    //Get the User Input for new Team Name
                    Console.Write("Enter New League Name: ");
                    var newLeagueName = Console.ReadLine();
                    var oldLeagueName = leagueRecord.Name;
                    leagueRecord.Name = newLeagueName;
                    //Save Record
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Successfully altered league Id {leagueId} with name of: {oldLeagueName} to: ");
                    await GetRecord(decision, Int32.Parse(leagueId));
                }
                catch
                {
                   Console.Write("Your input was not valid please try again");
                    await UpdateRecord();
                }
                //Print the record that changed
                //Ask for more input
                Console.Write("Would you like to alter another entry? \ny or n: ");
                ans = Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Your input was not valid would you like to try again? \ny or n");
                ans = Console.ReadLine();
            }

            if(ans == "y") //if yes restart the method
            {
                await UpdateRecord();
            }
            else if(ans == "n" || ans == null) //else bail out
            {
                cts.Cancel();
            }
        }

        private static async Task GetRecord(string type, int id)
        {
            if (type == "team")
            {
                var record = await context.Teams.FindAsync(id);
                Console.WriteLine($"{record.Id} - {record.Name}");
            }
            else if (type == "league")
            {
                var record = await context.Leagues.FindAsync(id);
                Console.WriteLine($"{record.Id} - {record.Name}");
            }
            else
            {
                var cts = new CancellationTokenSource();
                Console.Write("The record was not found please try again");
                cts.Cancel();
            }
        }
    }
}