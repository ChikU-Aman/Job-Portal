using System;
using System.Collections.Generic;

#nullable disable

namespace Job_Portal.Models
{
    public partial class Status
    {
        public Status()
        {
            SubmittedJobs = new HashSet<SubmittedJob>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<SubmittedJob> SubmittedJobs { get; set; }
    }
}
