using EntityFrameworkCore.Data;
using EntityFrameworkCore.Data.Migrations;
using EntityFrameworkCore.Domain;
using EntityFrameworkCore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.ConsoleAppv
{
    internal class Program
    {
        private static FootballLeageDbContext context = new FootballLeageDbContext();
        private static League RECORD_LEAGUE = new League { };
        private static Team RECORD_TEAM = new Team { };
        static async Task Main(string[] args)
        {
            var league = new League { Name = "Red Premiere League" };
            var testRemoveLeage = new League { Name = "Delete Me" };

            // await AddNewLeague(league);
            // await AddTeamsWithLeagueId(league);

            // await SelectLeagues();
            // await QueryFilter();
            // await AdditionalExecutionMethods();
            // await altLINQSyntax();

            //await UpdateRecord();



            // methods for removing records
            /*
            await AddNewLeague(testRemoveLeage); //insert league into table
            await GetRecord("league", testRemoveLeage.Id); //sanity check get record from table
            await SimpleDelete(testRemoveLeage); //remove record
            //sanity check record is removed from table -- expecting exception to occur
            try
            {
                await GetRecord("league", testRemoveLeage.Id); 
            }
            catch
            {
                Console.WriteLine("Sorry this record cannot be found");
            }
            */
            //await DeleteWithRelationship();

            /* same thing but for a record that currently exists and has ties, this must be set up beforehand for this method to work
            try
            {
                await GetRecord("league", 4);
                await DeleteWithRelationship(RECORD_LEAGUE);
            }
            catch
            {
                Console.WriteLine("Sorry this record cannot be found");
            }
            */

            //await TrackingVsNoTracking();

            /* loading some test relationship data
            await AddNewTeamWithLeague("Test Team", 5);
            await AddNewLeagueWithTeams("Test League");

            await AddNewMatches();

            await AddNewCoachWithoutTeam("Coach W_O_Team");
            await AddNewCoachWithTeam("Coach W_Team", 4);
            */

            /* Including Related Data- Eager Loading */
            //await QueryRelatedRecords(4);

            //await AnonymousProjection();
            //await StronglyTypedProjection();


            //await FilteringWithRelatedData();

            await QueryView();

            Console.WriteLine("Press any key to end...");
            Console.ReadKey();
        }

        static async Task AddNewLeague(League league)
        {
            await context.Leagues.AddAsync(league); //define db context - table to interact with - option (object) -- action happens in memory
            await context.SaveChangesAsync(); //generate sql and execute action on db
        }

        static async Task AddNewCoachWithoutTeam(string coachName)
        {
            var coach = new Coach { Name = coachName };

            await context.Coaches.AddRangeAsync(coach);
            await context.SaveChangesAsync();
        }

        static async Task AddNewCoachWithTeam(string coachName, int teamId)
        {
            var coach = new Coach { Name = coachName , TeamID = teamId};

            await context.Coaches.AddRangeAsync(coach);
            await context.SaveChangesAsync();
        }

        static async Task AddNewTeamWithLeague(string teamName, int leagueId)
        {
            var team = new Team { Name = teamName, LeagueId = leagueId };

            await context.Teams.AddAsync(team);
            await context.SaveChangesAsync();
        }
      
        static async Task AddNewLeagueWithTeams(string leagueName)
        {
            var teams = new List<Team>
            {
                new Team { Name = "Team 1"},
                new Team { Name = "Team 2"},
            };

            var league = new League { Name = leagueName , Teams = teams};
            await context.Leagues.AddAsync(league);
            await context.SaveChangesAsync();

        }

        static async Task AddNewMatches()
        {
            var matches = new List<Match>
            {
                new Match
                {
                    AwayTeamId = 4, HomeTeamId = 5, Date = DateTime.Now
                },
                new Match
                {
                    AwayTeamId = 5, HomeTeamId = 4, Date = DateTime.Now
                },
                new Match
                {
                    AwayTeamId = 5, HomeTeamId = 6, Date = DateTime.Now
                },
            };

            await context.AddRangeAsync(matches);
            await context.SaveChangesAsync();
        }

        static async Task SelectLeagues()
        {
            var leagues = await context.Leagues.ToListAsync();

            foreach (League league in leagues)
            {
                Console.WriteLine($"{league.Id} - {league.Name}");
                RECORD_LEAGUE = league;
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

            if (ans == "y") //if yes restart the method
            {
                await UpdateRecord();
            }
            else if (ans == "n" || ans == null) //else bail out
            {
                cts.Cancel();
            }
        }
        
        static async Task QueryRelatedRecords(int teamId)
        {
            //all records
            var leagues = await context.Leagues
                .Include(q => q.Teams)
                .ToListAsync();

            //one record
            var team = await context.Teams
                .Include(t => t.Coach)
                .FirstOrDefaultAsync(q => q.Id == teamId);
            /*
            //grand-children inclusion
            var teamsWithMatchesAndOpponents = await context.Teams
                .Include(t => t.AwayMatches).ThenInclude(t => t.HomeTeam).ThenInclude(t => t.Coach) //if away match include home team details
                //.Include(t => t.HomeMatches).ThenInclude(t => t.AwayTeam).ThenInclude(t => t.Coach) //if home match include away team details
                .FirstOrDefaultAsync(q => q.Id == teamId); 
            */


            //grandchildren inclusion with filters
            var teams = await context.Teams
                .Where(q => q.HomeMatches.Count > 0)
                .Include(q => q.Coach)
                .ToListAsync();
        }

        static async Task SelectOneProp()
        {
            var teams = await context.Teams.Select(q => q.Name).ToListAsync();
        } 

        static async Task AnonymousProjection()
        {
            var teams = await context.Teams
                .Include(q => q.Coach)
                .Select(q => 
                    new { 
                        TeamName = q.Name,
                        CoachName = q.Coach.Name,
                    })
                .ToListAsync();
            foreach( var team in teams)
            {
                Console.WriteLine($"Team: {team.TeamName} | Coach: {team.CoachName}");
            }

        }

        static async Task StronglyTypedProjection()
        {
            var teams = await context.Teams
                .Include(q => q.Coach)
                .Include(q => q.League)
                .Select(q =>
                    new TeamDetail {
                        TeamName = q.Name,
                        CoachName = q.Coach.Name,
                        LeagueName = q.League.Name,
                    })
                .ToListAsync();
            foreach (var team in teams)
            {
                Console.WriteLine($"Team: {team.TeamName} | Coach: {team.CoachName}");
            }
        }

        static async Task FilteringWithRelatedData() //produces league name based on search for a partial team name
        {
            var leagues = await context.Leagues.
                Where(q => q.Teams.Any(x => x.Name.Contains("This")))
                .ToListAsync();

            foreach(var league in leagues)
            {
                Console.WriteLine(league.Name); 
            }
        }

        static async Task GetRecord(string type, int id)
        {
            if (type == "team")
            {
                var record = await context.Teams.FindAsync(id);
                Console.WriteLine($"{record.Id} - {record.Name}");
                RECORD_TEAM = record;
            }
            else if (type == "league")
            {
                var record = await context.Leagues.FindAsync(id);
                Console.WriteLine($"{record.Id} - {record.Name}");
                RECORD_LEAGUE = record;
            }
            else
            {
                var cts = new CancellationTokenSource();
                Console.Write("The record was not found please try again");
                cts.Cancel();
            }
        }

        static async Task SimpleDelete(League league)
        {
            context.Leagues.Remove(league);
            await context.SaveChangesAsync();
        }

        static async Task DeleteWithRelationship(League league)
        {
            context.Leagues.Remove(league);
            await context.SaveChangesAsync();
        }

        static async Task TrackingVsNoTracking()
        {
            var with = await context.Teams.FirstOrDefaultAsync(q => q.Id == 4);
            var withOut = await context.Teams.AsNoTracking().FirstOrDefaultAsync(q => q.Id == 6);

            with.Name = "This Team";
            withOut.Name = "That Team";

            var entriesBeforeSave = context.ChangeTracker.Entries();
            foreach (var i in entriesBeforeSave)
            {
                Console.WriteLine(i.ToString());

            }
            await context.SaveChangesAsync();

            var entriesAfterSave = context.ChangeTracker.Entries();
            foreach (var i in entriesAfterSave)
            {
                Console.WriteLine(i.ToString());

            }

        }

        static async Task QueryView()
        {
            var details = await context.teamsCoachesLeagues.ToListAsync();
        }
    }
}