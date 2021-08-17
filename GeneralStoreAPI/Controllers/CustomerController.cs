using GeneralStoreAPI.Models;
using GeneralStoreAPI.Models.Creation_Models;
using GeneralStoreAPI.Models.Data_POCOs;
using GeneralStoreAPI.Models.Request_Parameters;
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
    public class CustomerController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        // Create (POST)
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] CustomerCreate customer)
        {
            if (customer is null)
            {
                return BadRequest("No customer information was provided. (Your request body was empty.)");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer newCustomer = new Customer
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName
            };

            _context.Customers.Add(newCustomer);

            if (await _context.SaveChangesAsync() == 1)
            {
                return Ok($"Customer {newCustomer.FullName} has been successfully added.");
            }

            return InternalServerError();
        }

        // Get All Customers (GET)
        [Route("api/Customer/All")]
        public async Task<IHttpActionResult> GetAll()
        {
            List<Customer> customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] CustomerParameters parameters)
        {
            int skip = ((parameters.PageNumber < 1 ? 0 : parameters.PageNumber - 1)) * parameters.PageSize;
            int take = parameters.PageSize;
            var pageQuery = await _context.Customers.OrderBy(on => on.Id).Skip(skip).Take(take).ToListAsync();
            return Ok(pageQuery);
        }

        // Get a Customer by its ID (GET)
        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] int id)
        {
            if (id < 1)
            {
                return BadRequest("Customer ID must be greater than 0.");
            }

            Customer customer = await _context.Customers.FindAsync(id);

            if (customer is null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // Update an existing Customer by its ID (PUT)
        [HttpPut]
        public async Task<IHttpActionResult> Put([FromUri] int id, [FromBody] Customer updatedCustomer)
        {
            if (updatedCustomer is null)
            {
                return BadRequest("No customer information was provided. (Your request body was empty.)");
            }

            if (updatedCustomer.Id != id)
            {
                return BadRequest("The provided customer information did not match the given customer ID.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer customer = await _context.Customers.FindAsync(id);

            if (customer is null)
            {
                return NotFound();
            }

            customer.FirstName = updatedCustomer.FirstName;
            customer.LastName = updatedCustomer.LastName;

            if (await _context.SaveChangesAsync() == 1)
            {
                return Ok($"Customer {customer.FullName} has been successfully updated.");
            }

            return InternalServerError();
        }

        // Delete an existing Customer by its ID (DELETE)
        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromUri] int id)
        {
            if (id < 1)
            {
                return BadRequest("Customer ID must be greater than 0.");
            }

            Customer customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            if (await _context.SaveChangesAsync() == 1)
            {
                return Ok("The requested customer was removed from the database.");
            }

            return InternalServerError();
        }
    }
}
