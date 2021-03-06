using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models.Data_POCOs
{
    public class Product
    {
        [Key]
        [Required]
        public string SKU { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public double Cost { get; set; }
        [Required]
        public int NumberInInventory { get; set; }

        public bool IsInStock => NumberInInventory > 0;
    }
}