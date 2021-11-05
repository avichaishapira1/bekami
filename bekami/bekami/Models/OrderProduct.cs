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
        public Order OrderId;

        public Product ProductId;

        
        [DisplayName("Total")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")] //rule: Price: 18 digits, 2 decimal points , without that we get an error 
        public decimal UnitPrice { get; set; }

        [Range(0, int.MaxValue)]
        [DisplayName("Amount")]
        public int Quantity { get; set; }

        


    }
}