using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bekami.Models
{
    public class Color
    {
        [Key]
        public int ColorId { get; set; }
        [Required]
        [RegularExpression(@"^[^<>.,?;:'()!~%\-_@#/*""\s]+$")]
        public string Name { get; set; }

    }
}
