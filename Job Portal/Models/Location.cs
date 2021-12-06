using System;
using System.Collections.Generic;

#nullable disable

namespace Job_Portal.Models
{
    public partial class Location
    {
        public Location()
        {
            Jobes = new HashSet<Jobe>();
        }

        public int LocationId { get; set; }
        public string LocationName { get; set; }

        public virtual ICollection<Jobe> Jobes { get; set; }
    }
}
