using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace bekami.Models
{

    public enum OrderStatus
    {
        Canceled,
        Processing,
        Shipped,
        Delivered,
        Returned
    }


    public class Order
    {
        [DisplayName("ID")] 
        [Key]
        public int Id { get; set; }

        [DisplayName("Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Created { get; set; }

        [DisplayName("Total")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")] //rule: Price: 18 digits, 2 decimal points , without that we get an error 
        public decimal Total { get; set; }

        [Range(0, int.MaxValue)]
        [DisplayName("Items")]
        public int NumOfItems { get; set; }

        [DisplayName("Order status")] public OrderStatus Status { get; set; }
        [ForeignKey("User")]
        public User User { get; set; }

        public ICollection<OrderProduct> ProductsOrdered { get; set; }

        [DisplayName("Address")] 
        public String Address { get; set; }
        
        [Required]
        [DataType(DataType.PhoneNumber)]
        [DisplayName("Phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        [DisplayName("Credit card number")]
        public string CreditCardNum { get; set; }

    }
}
