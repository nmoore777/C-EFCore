using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

#nullable disable

namespace EntityFrameworkCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingTeamDetailsViewAndEarlyMatchFunctionUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW [dbo].[TeamsCoachesLeagues]
									AS
									SELECT t.Name, c.Name AS CoachName, l.Name AS LeagueName
									FROM  dbo.Teams AS t LEFT OUTER JOIN
													  dbo.Coaches AS c ON t.Id = c.TeamId INNER JOIN
													  dbo.Leagues AS l ON t.LeagueId = l.Id"
            );
            migrationBuilder.Sql(@"DROP VIEW [dbo].[View_1]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW [dbo].[TeamsCoachesLeagues]");
            migrationBuilder.Sql(@"DROP Function [db].[GetEarliestMatch]");
        }
    }
}
