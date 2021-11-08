using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using bekami.Models;

namespace bekami.Models
{
    public class CartSession
    {
        [Key]
        public int CartId { get; set; }
        
        public String AccountSessionID { get; set; }

        public ICollection<CartItem> Items { get; set; }


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayName("Date created")]
        public DateTime Created { get; set; }


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayName("Last modified")]
        public DateTime LastUpdate { get; set; }


    }
}
