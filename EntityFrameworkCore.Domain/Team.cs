using EntityFrameworkCore.Domain.Common;

namespace EntityFrameworkCore.Domain
{
    public class Team : BaseDomainObject
    {
        //attibutes
        public string Name { get; set; }

        //foreign keys and relationships
        public int LeagueId { get; set; } //league key
        public virtual League League { get; set; } //league table 

        public virtual List<Match> HomeMatches { get; set; }
        public virtual List<Match> AwayMatches { get; set; }
    }
}
