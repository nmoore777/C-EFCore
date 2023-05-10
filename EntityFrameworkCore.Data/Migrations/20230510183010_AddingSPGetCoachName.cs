using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFrameworkCore.Data.Migrations
{
    public partial class AddingSPGetCoachName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE sp_GetTeamCoach
	                                @TeamId int
                                AS
                                BEGIN
	                                SELECT * FROM Coaches WHERE TeamId = @teamId
                                END
                                GO"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE [dbo].[sp_CoachName]");
        }
    }
}