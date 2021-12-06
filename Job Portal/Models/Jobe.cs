using System;
using System.Collections.Generic;

#nullable disable

namespace Job_Portal.Models
{
    public partial class Jobe
    {
        public Jobe()
        {
            SubmittedJobs = new HashSet<SubmittedJob>();
        }

        public int JobId { get; set; }
        public int EmployerId { get; set; }
        public string Jobtitle { get; set; }
        public int JobLocation { get; set; }
        public string JobDescription { get; set; }
        public DateTime JobLastDate { get; set; }

        public virtual Location JobLocationNavigation { get; set; }
        public virtual ICollection<SubmittedJob> SubmittedJobs { get; set; }
    }
}
