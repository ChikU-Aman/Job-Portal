using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Job_Portal.Models
{
    public partial class Employeer
    {
        [Key]
        public int EmployerId { get; set; }

        [Required(ErrorMessage = "Cannot Be Blank")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Cannot Be Blank")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Cannot Be Blank")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage ="Cannot Be Blank")]
        [StringLength(10,ErrorMessage ="Not A Valid Phone Number",MinimumLength = 10)]
        [DataType(DataType.PhoneNumber)]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage ="Cannot Be Blank")]
        [DataType(DataType.EmailAddress)]
        public string EmailId { get; set; }
    }
}
