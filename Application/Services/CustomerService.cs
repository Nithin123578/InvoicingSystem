using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        // In-memory storage for customers
        private readonly List<Customer> _customers = new List<Customer>();

        // Logger for logging errors and other information
        private readonly ILogger<CustomerService> _logger;

        // Counter for generating unique IDs for customers
        private int _nextId = 1;

        // Initializes the service with the logger dependency
        public CustomerService(ILogger<CustomerService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Retrieves all customers
        // Returns a list of customers
        public IEnumerable<Customer> GetCustomers()
        {
            try
            {
                // Returns the list of customers
                return _customers;
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while getting customers
                _logger.LogError(ex, "Error occurred while getting customers.");
                // Returns an empty list in case of error
                return new List<Customer>();
            }
        }

        // Retrieves a specific customer by its ID
        // Returns the customer or throws an exception if not found
        public Customer GetCustomerById(int id)
        {
            try
            {
                var customer = _customers.FirstOrDefault(c => c.Id == id);
                if (customer == null)
                {
                    // Throws an exception if the customer with the specified ID is not found
                    throw new KeyNotFoundException($"Customer with ID {id} not found");
                }
                // Returns the found customer
                return customer;
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while getting customer by ID
                _logger.LogError(ex, "Error occurred while getting customer by ID.");
                throw;
            }
        }

        // Adds a new customer
        // Returns the created customer
        public Customer AddCustomer(Customer customer)
        {
            try
            {
                // Validates the customer to ensure it has valid properties
                ValidateCustomer(customer);

                // Checks if the customer email already exists
                if (_customers.Any(c => c.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ArgumentException("Customer email already exists");
                }

                // Assigns a unique ID to the customer and adds it to the list
                customer.Id = _nextId++;
                _customers.Add(customer);

                // Returns the added customer
                return customer;
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while adding the customer
                _logger.LogError(ex, "Error occurred while adding customer.");
                throw;
            }
        }

        // Updates an existing customer
        public void UpdateCustomer(Customer customer)
        {
            try
            {
                // Validates the customer to ensure it has valid properties
                ValidateCustomer(customer);

                // Finds the existing customer to update
                var existingCustomer = _customers.FirstOrDefault(c => c.Id == customer.Id);
                if (existingCustomer == null)
                {
                    throw new KeyNotFoundException($"Customer with ID {customer.Id} not found");
                }

                // Updates the customer details
                existingCustomer.Name = customer.Name;
                existingCustomer.Email = customer.Email;
                existingCustomer.Address = customer.Address;
                existingCustomer.ContactNumber = customer.ContactNumber;
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while updating the customer
                _logger.LogError(ex, "Error occurred while updating customer.");
                throw;
            }
        }

        // Deletes a specific customer by its ID
        public void DeleteCustomer(int id)
        {
            try
            {
                // Finds and removes the customer with the specified ID
                var customer = _customers.FirstOrDefault(c => c.Id == id);
                if (customer == null)
                {
                    throw new KeyNotFoundException($"Customer with ID {id} not found");
                }
                _customers.Remove(customer);
            }
            catch (Exception ex)
            {
                // Logs any errors that occur while deleting the customer
                _logger.LogError(ex, "Error occurred while deleting customer.");
                throw;
            }
        }

        // Validates the customer to ensure it has valid properties
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

        // Validates email format
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

        // Validates contact number format
        private bool IsValidContactNumber(string contactNumber)
        {
            var regex = new Regex(@"^\d{10}$");
            return regex.IsMatch(contactNumber);
        }
    }
}
