using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace bekami.Models
{
    public enum Size
    {
        S=1,
        M,
        L,
        XL,
    }
    public enum Sleeve
    {
        Short,
        Long
    }
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [EnumDataType(typeof(Size))]
        [Required]
        public Size Size { get; set; }
        [EnumDataType(typeof(Sleeve))]
        [Required]
        public Sleeve Sleeves { get; set; }

        [Required]
        public double Price { get; set; }
        [Display(Name = "Is available")]
        public bool IsAvailable { get; set; }
        [Display(Name = "Image")]
        [Required]
        [DataType(DataType.ImageUrl)]
        public string Imagepath { get; set; }



        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public IEnumerable<Tags> Tags { get; set; }
    }

}
