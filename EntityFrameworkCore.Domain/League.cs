using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Domain
{
    public class League
    {
        public int Id { get; set; } //primary key

        //attributes
        public string Name { get; set; }
        public List<Team> MyProperty { get; set; } //getter for teams list by either getting league by id or nmame
    }
}
