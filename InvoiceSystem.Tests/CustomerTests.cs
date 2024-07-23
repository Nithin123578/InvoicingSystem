using NUnit.Framework;
using Application.Interfaces;
using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using InvoicingSystem.Controllers;
using Moq;
using Microsoft.Extensions.Logging;

namespace InvoicingSystem.Tests
{
    [TestFixture]
    public class CustomerTests
    {
        private ICustomerService _customerService;
        private CustomerController _controller;
        private Mock<ILogger<CustomerService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<CustomerService>>();
            _customerService = new CustomerService(_loggerMock.Object);
            _controller = new CustomerController(_customerService);
        }

        [Test]
        public void AddCustomer_ShouldThrowException_CustomerIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _controller.AddCustomer(null));
            Assert.AreEqual("Customer is empty (Parameter 'customer')", ex.Message);
        }

        [Test]
        public void UpdateCustomer_ShouldThrowException_CustomerIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _controller.UpdateCustomer(null));
            Assert.AreEqual("Customer is empty (Parameter 'customer')", ex.Message);
        }

        [Test]
        public void GetCustomers_Should_ReturnCorrectCustomerList()
        {
            var customer1 = new Customer { Name = "John Doe", Email = "john@example.com", Address = "123 Street", ContactNumber = "1234567890" };
            _controller.AddCustomer(customer1);

            var customer2 = new Customer { Name = "Jane Smith", Email = "jane@example.com", Address = "456 Avenue", ContactNumber = "0987654321" };
            _controller.AddCustomer(customer2);

            var getResult = _controller.GetCustomers() as ActionResult<IEnumerable<Customer>>;
            var okResult = getResult.Result as OkObjectResult;
            var retrievedCustomers = okResult.Value as List<Customer>;

            Assert.AreEqual(2, retrievedCustomers.Count);
            Assert.Contains(customer1, retrievedCustomers);
            Assert.Contains(customer2, retrievedCustomers);
        }

        [Test]
        public void GetCustomerById_Should_ReturnCorrectCustomer()
        {
            var customer = new Customer { Name = "John Doe", Email = "john@example.com", Address = "123 Street", ContactNumber = "1234567890" };
            var result = _controller.AddCustomer(customer);
            var okResult = result.Result as OkObjectResult;
            var addedCustomer = okResult.Value as Customer;

            var getResult = _controller.GetCustomer(addedCustomer.Id) as ActionResult<Customer>;
            okResult = getResult.Result as OkObjectResult;
            var retrievedCustomer = okResult.Value as Customer;

            Assert.AreEqual(addedCustomer, retrievedCustomer);
        }
        [Test]
        public void AddCustomer_Should_AddCustomerSuccessfully()
        {
            var customer = new Customer { Name = "John Doe", Email = "john@example.com", Address = "123 Street", ContactNumber = "1234567890" };
            var result = _controller.AddCustomer(customer);
            var okResult = result.Result as OkObjectResult;
            var createdCustomer = okResult.Value as Customer;

            Assert.AreEqual(1, createdCustomer.Id);
            Assert.AreEqual(customer.Name, createdCustomer.Name);
        }

        [Test]
        public void UpdateCustomer_Should_UpdateCustomerSuccessfully()
        {
            var customer = new Customer { Name = "John Doe", Email = "john@example.com", Address = "123 Street", ContactNumber = "1234567890" };
            var result = _controller.AddCustomer(customer);
            var okResult = result.Result as OkObjectResult;
            var addedCustomer = okResult.Value as Customer;

            addedCustomer.Name = "Jane Doe";
            _controller.UpdateCustomer(addedCustomer);

            var updatedResult = _controller.GetCustomer(addedCustomer.Id) as ActionResult<Customer>;
            okResult = updatedResult.Result as OkObjectResult;
            var updatedCustomer = okResult.Value as Customer;

            Assert.AreEqual("Jane Doe", updatedCustomer.Name);
        }

        [Test]
        public void DeleteCustomer_Should_RemoveCustomerSuccessfully()
        {
            var customer = new Customer { Name = "John Doe", Email = "john@example.com", Address = "123 Street", ContactNumber = "1234567890" };
            var result = _controller.AddCustomer(customer);
            var okResult = result.Result as OkObjectResult;
            var addedCustomer = okResult.Value as Customer;

            _controller.DeleteCustomer(addedCustomer.Id);
            var getResult = _controller.GetCustomers() as ActionResult<IEnumerable<Customer>>;
            okResult = getResult.Result as OkObjectResult;
            var retrievedCustomers = okResult.Value as List<Customer>;

            Assert.IsFalse(retrievedCustomers.Any(p => p.Id == addedCustomer.Id));
        }

        [Test]
        public void AddCustomer_Should_ThrowException_ForInvalidCustomer()
        {
            var invalidCustomer = new Customer { Name = "", Email = "john@example.com", Address = "123 Street", ContactNumber = "1234567890" };
            var ex = Assert.Throws<ArgumentException>(() => _customerService.AddCustomer(invalidCustomer));
            Assert.AreEqual("Customer name cannot be empty", ex.Message);

            invalidCustomer.Name = "John Doe";
            invalidCustomer.Email = "";
            ex = Assert.Throws<ArgumentException>(() => _customerService.AddCustomer(invalidCustomer));
            Assert.AreEqual("Customer email cannot be empty", ex.Message);

            invalidCustomer.Email = "invalidemail";
            ex = Assert.Throws<ArgumentException>(() => _customerService.AddCustomer(invalidCustomer));
            Assert.AreEqual("Customer email is not in a valid format", ex.Message);

            invalidCustomer.Email = "john@example.com";
            invalidCustomer.ContactNumber = "";
            ex = Assert.Throws<ArgumentException>(() => _customerService.AddCustomer(invalidCustomer));
            Assert.AreEqual("Customer contact number cannot be empty", ex.Message);

            invalidCustomer.ContactNumber = "invalidnumber";
            ex = Assert.Throws<ArgumentException>(() => _customerService.AddCustomer(invalidCustomer));
            Assert.AreEqual("Customer contact number is not in a valid format", ex.Message);
        }

        [Test]
        public void UpdateCustomer_Should_ThrowException_ForInvalidCustomer()
        {
            var customer = new Customer { Name = "John Doe", Email = "john@example.com", Address = "123 Street", ContactNumber = "1234567890" };
            var addedCustomer = _customerService.AddCustomer(customer);

            customer.Id = addedCustomer.Id;
            customer.Name = "";
            var ex = Assert.Throws<ArgumentException>(() => _customerService.UpdateCustomer(customer));
            Assert.AreEqual("Customer name cannot be empty", ex.Message);

            customer.Name = "John Doe";
            customer.Email = "";
            ex = Assert.Throws<ArgumentException>(() => _customerService.UpdateCustomer(customer));
            Assert.AreEqual("Customer email cannot be empty", ex.Message);

            customer.Email = "invalidemail";
            ex = Assert.Throws<ArgumentException>(() => _customerService.UpdateCustomer(customer));
            Assert.AreEqual("Customer email is not in a valid format", ex.Message);

            customer.Email = "john@example.com";
            customer.ContactNumber = "";
            ex = Assert.Throws<ArgumentException>(() => _customerService.UpdateCustomer(customer));
            Assert.AreEqual("Customer contact number cannot be empty", ex.Message);

            customer.ContactNumber = "invalidnumber";
            ex = Assert.Throws<ArgumentException>(() => _customerService.UpdateCustomer(customer));
            Assert.AreEqual("Customer contact number is not in a valid format", ex.Message);
        }

        [Test]
        public void UpdateCustomer_Should_ThrowException_ForInvalidCustomerId()
        {
            var customer = new Customer { Name = "John Doe", Email = "john@example.com", Address = "123 Street", ContactNumber = "1234567890" };
            var addedCustomer = _customerService.AddCustomer(customer);

            var invalidCustomerId = 999;
            var ex = Assert.Throws<KeyNotFoundException>(() => _customerService.GetCustomerById(invalidCustomerId));
            Assert.AreEqual($"Customer with ID {invalidCustomerId} not found", ex.Message);
        }

        [Test]
        public void GetCustomers_Should_ReturnEmptyList_WhenNoCustomers()
        {
            var getResult = _controller.GetCustomers() as ActionResult<IEnumerable<Customer>>;
            var okResult = getResult.Result as OkObjectResult;
            var retrievedCustomers = okResult.Value as List<Customer>;

            Assert.AreEqual(0, retrievedCustomers.Count);
        }

        [Test]
        public void AddCustomer_Should_ThrowException_ForDuplicateEmail()
        {
            var customer1 = new Customer { Name = "John Doe", Email = "john@example.com", Address = "123 Street", ContactNumber = "1234567890" };
            _controller.AddCustomer(customer1);

            var customer2 = new Customer { Name = "Jane Smith", Email = "john@example.com", Address = "456 Avenue", ContactNumber = "0987654321" };
            var ex = Assert.Throws<ArgumentException>(() => _customerService.AddCustomer(customer2));
            Assert.AreEqual("Customer email already exists", ex.Message);
        }
    }
}
