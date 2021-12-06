using System;
using System.Collections.Generic;

#nullable disable

namespace Job_Portal.Models
{
    public partial class SubmittedJob
    {
        public int Id { get; set; }
        public int ApplicantId { get; set; }
        public int JobId { get; set; }
        public int? StatusId { get; set; }

        public virtual JobSeeker Applicant { get; set; }
        public virtual Jobe Job { get; set; }
        public virtual Status Status { get; set; }
    }
}
