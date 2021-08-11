using GeneralStoreAPI.Models.Data_POCOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models.Creation_Models
{
    public class TransactionCreate
    {
        [Required]
        public int? CustomerId { get; set; }

        [Required]
        public string ProductSKU { get; set; }

        [Required]
        public int ItemCount { get; set; }

        [Required]
        public DateTime DateOfTransaction { get; set; }
    }
}