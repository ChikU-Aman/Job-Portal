using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Portal.Models
{
    public class MyJobApp
    {
        [Key]
        public string JobTitle { get; set; }
        public string JobDescription {get; set;}
        public string CompanyName { get; set; }
        public string JobLocation { get; set; }
        public int JobStatus { get; set; }
    }
}
