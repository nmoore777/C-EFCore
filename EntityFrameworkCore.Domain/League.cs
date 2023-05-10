using EntityFrameworkCore.Domain.Common;

namespace EntityFrameworkCore.Domain
{
    public class League : BaseDomainObject
    {

        //attributes
        public string Name { get; set; }
        public List<Team> MyProperty { get; set; } //getter for teams list by either getting league by id or nmame
    }
}
