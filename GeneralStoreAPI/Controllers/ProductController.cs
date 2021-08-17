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
        [Route("api/Product/{sku}")]
        public async Task<IHttpActionResult> Get([FromUri] string sku)
        {
            Product product = await _context.Products.FindAsync(sku);

            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // Get out of stock products (GET)
        [Route("api/Product/GetOutOfStock")]
        public async Task<IHttpActionResult> GetOutOfStock()
        {
            List<Product> products = await _context.Products.Where(prod => prod.NumberInInventory <= 0).ToListAsync();

            if (products.Count == 0)
            {
                return Ok("All products are in stock.");
            }

            return Ok(products);
        }

        // Update an existing Product by its SKU (PUT)
        [HttpPut]
        [Route("api/Product/{sku}")]
        public async Task<IHttpActionResult> Put([FromUri] string sku, [FromBody] Product updatedProduct)
        {
            if (updatedProduct is null)
            {
                return BadRequest("No product information was provided. (Your request body was empty.)");
            }

            if (updatedProduct.SKU != sku)
            {
                return BadRequest("The provided product information did not match the given product SKU.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Product product = await _context.Products.FindAsync(sku);

            if (product is null)
            {
                return NotFound();
            }

            product.Name = updatedProduct.Name;
            product.Cost = updatedProduct.Cost;
            product.NumberInInventory = updatedProduct.NumberInInventory;

            if (await _context.SaveChangesAsync() == 1)
            {
                return Ok($"Product {product.Name} has been successfully updated.");
            }

            return InternalServerError();
        }
    }
}
