using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bekami.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Display(Name = "Name")]
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
