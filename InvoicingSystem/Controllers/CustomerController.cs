using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
namespace InvoicingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetCustomers()
        {
            return Ok(_customerService.GetCustomers());
        }

        [HttpGet("{id}")]
        public ActionResult<Customer> GetCustomer(int id)
        {
            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpPost]
        public ActionResult<Customer> AddCustomer(Customer customer)
        {
            var createdCustomer = _customerService.AddCustomer(customer);
            return Ok(createdCustomer);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(Customer customer)
        {
            if (customer.Id == 0)
            {
                return BadRequest();
            }
            _customerService.UpdateCustomer(customer);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            _customerService.DeleteCustomer(id);
            return NoContent();
        }
    }
}
