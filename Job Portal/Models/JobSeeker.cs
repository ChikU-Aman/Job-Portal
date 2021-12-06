using System;
using System.Collections.Generic;

#nullable disable

namespace Job_Portal.Models
{
    public partial class JobSeeker
    {
        public JobSeeker()
        {
            SubmittedJobs = new HashSet<SubmittedJob>();
        }

        public int ApplicantId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public string EmailId { get; set; }

        public virtual ICollection<SubmittedJob> SubmittedJobs { get; set; }
    }
}
