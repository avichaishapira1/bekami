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
    public enum Gender
    {
        woman ,
        man
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
        [EnumDataType(typeof(Gender))]
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public double Price { get; set; }
        [Display(Name = "Is available")]
        public bool IsAvailable { get; set; }
        [Display(Name = "Image")]
        [Required]
        [DataType(DataType.ImageUrl)]
        public string Imagepath { get; set; }
        [Display(Name = " Seconde image")]
        public string Imagepath2 { get; set; }
        [Required]
        public string Description { get; set; }

        [Display(Name = "Color")]
        public int ColorId { get; set; }
        public Color Color { get; set; }
        public IEnumerable<Tags> Tags { get; set; }
    }

}
