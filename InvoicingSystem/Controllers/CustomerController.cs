using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace InvoicingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        // Service for handling customer-related operations
        private readonly ICustomerService _customerService;

        // Initializes the controller with the customer service dependency
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // Retrieves a list of all customers
        // Route: GET api/customer
        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetCustomers()
        {
            // Calls the service method to get all customers and returns them
            return Ok(_customerService.GetCustomers());
        }

        // Retrieves a specific customer by their ID
        // Route: GET api/customer/{id}
        [HttpGet("{id}")]
        public ActionResult<Customer> GetCustomer(int id)
        {
            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
            {
                // Returns a 404 Not Found response if the customer doesn't exist
                return NotFound();
            }
            // Returns the found customer
            return Ok(customer);
        }

        // Adds a new customer
        // Route: POST api/customer
        [HttpPost]
        public ActionResult<Customer> AddCustomer(Customer customer)
        {
            var createdCustomer = _customerService.AddCustomer(customer);
            // Returns the created customer with a 200 OK response
            return Ok(createdCustomer);
        }

        // Updates an existing customer
        // Route: PUT api/customer/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(Customer customer)
        {
            // Calls the service method to update the customer
            _customerService.UpdateCustomer(customer);
            // Returns a 204 No Content response as there's no content to return
            return NoContent();
        }

        // Deletes a specific customer by their ID
        // Route: DELETE api/customer/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            // Calls the service method to delete the customer
            _customerService.DeleteCustomer(id);
            // Returns a 204 No Content response as there's no content to return
            return NoContent();
        }
    }
}
