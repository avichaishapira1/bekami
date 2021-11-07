using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using bekami.Models;

namespace Bekami.Models
{
    public class CartSession
    {
        public int Id { get; set; }
        //acount sessionID used to bind cart to customer
        public String AccountSessionID { get; set; }

        public ICollection<OrderProduct> Items { get; set; }


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayName("Date created")]
        public DateTime DateCreated { get; set; }


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayName("Last modified")]
        public DateTime LastModified { get; set; }


    }
}
