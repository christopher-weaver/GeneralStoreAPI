using GeneralStoreAPI.Models;
using GeneralStoreAPI.Models.Creation_Models;
using GeneralStoreAPI.Models.Data_POCOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralStoreAPI.Controllers
{
    public class TransactionController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        // Create (POST)
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] TransactionCreate transaction)
        {
            if (transaction is null)
            {
                return BadRequest("No transaction information was provided. (Your request body was empty.)");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer customer = await _context.Customers.FindAsync(transaction.CustomerId);
            if (customer is null)
            {
                return BadRequest("Unable to locate the requested customer.");
            }

            Product product = await _context.Products.FindAsync(transaction.ProductSKU);
            if (product is null)
            {
                return BadRequest("Unable to locate the requested product.");
            }

            // Verify that the product is in stock
            if (!product.IsInStock)
            {
                return BadRequest($"Inform customer that {product.Name} is out of stock.");
            }

            // Check that there is enough product to complete the Transaction
            if (transaction.ItemCount > product.NumberInInventory)
            {
                return BadRequest($"Not enough product {product.Name} in inventory ({product.NumberInInventory}) to complete the requested transaction.");
            }

            Transaction newTransaction = new Transaction
            {
                Customer = customer,
                Product = product,
                ItemCount = transaction.ItemCount,
                DateOfTransaction = transaction.DateOfTransaction
            };

            _context.Transactions.Add(newTransaction);
            // Remove the Products that were bought
            product.NumberInInventory -= transaction.ItemCount;

            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok("Your transaction has been submitted.");
            }

            return InternalServerError();
        }

        // Get All Transactions (GET)
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            List<Transaction> transactions = await _context.Transactions.ToListAsync();
            return Ok(transactions);
        }


        // Get a Transaction by its ID (GET)
        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] int id)
        {
            Transaction transaction = await _context.Transactions.FindAsync(id);

            if (transaction is null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // Get Transactions by Customer ID (GET)
        [HttpGet]
        public async Task<IHttpActionResult> Get(string customerid)
        {
            if (!Int32.TryParse(customerid, out int intCustomerId))
            {
                return BadRequest("The customer ID must be an integer.");
            }

            List<Transaction> transactions = await _context.Transactions.Where(txn => txn.CustomerId.Equals(intCustomerId)).ToListAsync();

            if (transactions.Count == 0)
            {
                return NotFound();
            }

            return Ok(transactions);
        }

        // Get Transactions by date range (GET)
        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] DateTime startdate, [FromUri] DateTime enddate)
        {
            List<Transaction> transactions = await _context.Transactions.Where(txn => txn.DateOfTransaction >= startdate && txn.DateOfTransaction <= enddate).ToListAsync();

            if (transactions.Count == 0)
            {
                return NotFound();
            }

            return Ok(transactions);
        }

        // Get Total Sales of SKU (GET)
        [Route("api/Transaction/GetTotalSales/{sku}")]
        public async Task<IHttpActionResult> GetTotalSales([FromUri] string sku)
        {
            List <Transaction> transactions = await _context.Transactions.Where(txn => txn.ProductSKU.Equals(sku)).ToListAsync();

            if (transactions.Count == 0)
            {
                return NotFound();
            }

            return Ok(transactions.Sum(txn => txn.ItemCount * txn.Product.Cost));
        }
    }
}
