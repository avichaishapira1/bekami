using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace bekami.Models
{
    [Table("CartItem")]
    public class CartItem
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        [DisplayName("Price")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        public virtual Cart Cart { get; set; }
        
        public virtual Product Product { get; set; }
        
        public int ProductId { get; set; }

        public int CartId { get; set; }

    }
}
