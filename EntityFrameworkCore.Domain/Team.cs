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
        public string Name { get; set; }
        public int LeagueId { get; set; } //foreign key
        public virtual League League { get; set; } //related table
    }
}
