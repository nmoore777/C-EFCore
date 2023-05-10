using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Domain
{
    public class Team
    {
        public int Id { get; set; } //primary key
        
        //attibutes
        public string Name { get; set; }

        //foreign keys and relationships
        public int LeagueId { get; set; } //league foreign key
        public virtual League League { get; set; } //related league table 
    }
}
