using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bekami.Models
{
    public class Branch
    {
        [Key]
        public int LocationId { get; set; }
        [Required]
        public string City { get; set; }

        //for creating the google maps api 
        [Display(Name = "Longitube *")]
        [Required(ErrorMessage = "Please enter longitube")]
        public double LocationLongitude { get; set; }
        [Display(Name = "Latitude *")]
        [Required(ErrorMessage = "Please enter latitude")]
        public double LocationLatitude { get; set; }

        //More details :
        [Display(Name = "Opening Hours *")]
        [Required(ErrorMessage = "Opening hours is required.")]
        [StringLength(200, ErrorMessage = "Maximum length allowed is 200 characters.")]
        [DataType(DataType.MultilineText)]
        public string Openingtime { get; set; }
        [Display(Name = "Phone Number *")]
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(20, ErrorMessage = "Maximum length allowed is 20 characters.")]
        public string Phonenumber { get; set; }

    }
}
