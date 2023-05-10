using EntityFrameworkCore.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Domain
{
    public class Coach : BaseDomainObject
    {
        //attributes
        public string Name { get; set; }
        
        //foreign keys and relationships
        public int? TeamID { get; set; } //should be nullable can be a couch but not employeed
        public virtual Team Team { get; set; }
    }
}
