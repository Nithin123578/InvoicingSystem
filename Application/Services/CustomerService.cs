using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly List<Customer> _customers = new List<Customer>();
        private readonly ILogger<CustomerService> _logger;
        private int _nextId = 1;

        public CustomerService(ILogger<CustomerService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<Customer> GetCustomers()
        {
            try
            {
                return _customers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting customers.");
                return new List<Customer>();
            }
        }

        public Customer GetCustomerById(int id)
        {
            try
            {
                var customer = _customers.FirstOrDefault(c => c.Id == id);
                if (customer == null)
                {
                    throw new KeyNotFoundException($"Customer with ID {id} not found");
                }
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting customer by ID.");
                throw;
            }
        }

        public Customer AddCustomer(Customer customer)
        {
            try
            {
                ValidateCustomer(customer);
                customer.Id = _nextId++;
                _customers.Add(customer);
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding customer.");
                throw;
            }
        }

        public void UpdateCustomer(Customer customer)
        {
            try
            {
                ValidateCustomer(customer);
                var existingCustomer = _customers.FirstOrDefault(c => c.Id == customer.Id);
                if (existingCustomer == null)
                {
                    throw new KeyNotFoundException($"Customer with ID {customer.Id} not found");
                }
                existingCustomer.Name = customer.Name;
                existingCustomer.Email = customer.Email;
                existingCustomer.Address = customer.Address;
                existingCustomer.ContactNumber = customer.ContactNumber;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating customer.");
                throw;
            }
        }

        public void DeleteCustomer(int id)
        {
            try
            {
                var customer = _customers.FirstOrDefault(c => c.Id == id);
                if (customer == null)
                {
                    throw new KeyNotFoundException($"Customer with ID {id} not found");
                }
                _customers.Remove(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting customer.");
                throw;
            }
        }

        private void ValidateCustomer(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "Customer is empty");
            }

            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                throw new ArgumentException("Customer name cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(customer.Email))
            {
                throw new ArgumentException("Customer email cannot be empty");
            }

            if (!IsValidEmail(customer.Email))
            {
                throw new ArgumentException("Customer email is not in a valid format");
            }

            if (string.IsNullOrWhiteSpace(customer.Address))
            {
                throw new ArgumentException("Customer address cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(customer.ContactNumber))
            {
                throw new ArgumentException("Customer contact number cannot be empty");
            }

            if (!IsValidContactNumber(customer.ContactNumber))
            {
                throw new ArgumentException("Customer contact number is not in a valid format");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidContactNumber(string contactNumber)
        {
            var regex = new Regex(@"^\d{10}$");
            return regex.IsMatch(contactNumber);
        }
    }
}
