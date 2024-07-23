using NUnit.Framework;
using Application.Interfaces;
using Application.Models;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using InvoicingSystem.Controllers;

namespace InvoicingSystem.Tests
{
    [TestFixture]
    public class CustomerControllerTests
    {
       private ICustomerService _customerService;
        private CustomerController _controller;

        [SetUp]
        public void Setup()
        {
            _customerService = new CustomerService();
            _controller = new CustomerController(new CustomerService());
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
            var result = _controller.AddCustomer(customer1);
            var okResult = result.Result as OkObjectResult;
            var firstCustomer = okResult.Value as Customer;

            var customer2 = new Customer { Name = "Jane Smith", Email = "jane@example.com", Address = "456 Avenue", ContactNumber = "0987654321" };
            result = _controller.AddCustomer(customer2);
            okResult = result.Result as OkObjectResult;
            var secondCustomer = okResult.Value as Customer;

            var getResult = _controller.GetCustomers() as ActionResult<IEnumerable<Customer>>;
            okResult = getResult.Result as OkObjectResult;
            var retrievedCustomers = okResult.Value as List<Customer>;

            Assert.AreEqual(2, retrievedCustomers.Count);
            Assert.Contains(firstCustomer, retrievedCustomers);
            Assert.Contains(secondCustomer, retrievedCustomers);
        }

        [Test]
        public void GetCustomerById_Should_ReturnCorrectCustomer()
        {
            var customer = new Customer { Name = "John Doe", Email = "john@example.com", Address = "123 Street", ContactNumber = "1234567890" };
            var result = _controller.AddCustomer(customer);
            var okResult = result.Result as OkObjectResult;
            var addedCustomer = okResult.Value as Customer;

             result = _controller.GetCustomer(addedCustomer.Id) as ActionResult<Customer>;
             okResult = result.Result as OkObjectResult;
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

            result = _controller.GetCustomer(addedCustomer.Id) as ActionResult<Customer>;
            okResult = result.Result as OkObjectResult;
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
            result = _controller.GetCustomer(addedCustomer.Id) as ActionResult<Customer>;
            okResult = result.Result as OkObjectResult;
            var retrievedCustomer = okResult;
            Assert.IsNull(retrievedCustomer);
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
    }
}
