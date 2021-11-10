using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace bekami.Models
{
    

    public class OrderProduct
    {
        [DisplayName("ID")]
        public int OrderProductId { get; set; }
        
        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }
        public int OrderId { get; set; }

        public int ProductId { get; set; }
        
        [DisplayName("Total")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")] //rule: Price: 18 digits, 2 decimal points , without that we get an error 
        public decimal UnitPrice { get; set; }

        [Range(0, int.MaxValue)]
        [DisplayName("Amount")]
        public int Quantity { get; set; }
        
    }
}