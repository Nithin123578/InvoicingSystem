using Application.Interfaces;
using Application.Models;
using System.Text.RegularExpressions;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly List<Customer> _customers = new List<Customer>();
        private int _nextId = 1;

        public IEnumerable<Customer> GetCustomers()
        {
            return _customers;
        }

        public Customer GetCustomerById(int id)
        {
            return _customers.FirstOrDefault(c => c.Id == id);
        }

        public Customer AddCustomer(Customer customer)
        {
            ValidateCustomer(customer);
            customer.Id = _nextId++;
            _customers.Add(customer);
            return customer;
        }

        public void UpdateCustomer(Customer customer)
        {
            ValidateCustomer(customer);
            var existingCustomer = _customers.FirstOrDefault(c => c.Id == customer.Id);
            if (existingCustomer != null)
            {
                existingCustomer.Name = customer.Name;
                existingCustomer.Email = customer.Email;
                existingCustomer.Address = customer.Address;
                existingCustomer.ContactNumber = customer.ContactNumber;
            }
        }

        public void DeleteCustomer(int id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                _customers.Remove(customer);
            }
        }

        private void ValidateCustomer(Customer customer)
        {
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
