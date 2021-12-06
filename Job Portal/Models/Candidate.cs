using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal.Models
{
    public class Candidate
    {
        [Key]
        public int id { get; set; }
        public string ApplicantName { get; set; }
        public int Status { get; set; }
    }
}
