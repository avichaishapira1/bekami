using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bekami.Models
{
    public class Tags
    {

        [Key]
        public int TagId { get; set; }
        [Required]
        public string Name { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
