using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace bekami.Models
{
    public enum Role
    {
        Admin,
        Customer
    }

    public class User
    {

        /// <--------------------->
        /// Account Details
        /// <--------------------->
        [DisplayName("ID")]
        public int UserId { get; set; }

        [Required]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8)]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The passwords you entered do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public Role Role { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Registration date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Created { get; set; }
        

        //[DisplayName("Your orders")]
        //public ICollection<Order> Orders { get; set; }
        
    }
}
