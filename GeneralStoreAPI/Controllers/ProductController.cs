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
    public class ProductController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        // Create (POST)
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] ProductCreate product)
        {
            if (product is null)
            {
                return BadRequest("No product information was provided. (Your request body was empty.)");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Product newProduct = new Product
            {
                SKU = product.SKU,
                Name = product.Name,
                Cost = product.Cost,
                NumberInInventory = product.NumberInInventory
            };

            _context.Products.Add(newProduct);

            if (await _context.SaveChangesAsync() == 1)
            {
                return Ok($"Product {newProduct.Name} ({newProduct.NumberInInventory} in inventory @ {newProduct.Cost.ToString("C2")}) has been successfully added.");
            }

            return InternalServerError();
        }

        // Get All Products (GET)
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            List<Product> products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        // Get a Product by its ID (GET)
        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] string sku)
        {
            Product product = await _context.Products.FindAsync(sku);

            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }
    }
}
