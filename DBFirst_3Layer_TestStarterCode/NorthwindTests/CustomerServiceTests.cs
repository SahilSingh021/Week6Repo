using Microsoft.EntityFrameworkCore;
using NorthwindData;
using NorthwindData.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindTests;
public class CustomerServiceTests
{
    private CustomerService _sut;
    private NorthwindContext _context;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var options = new DbContextOptionsBuilder<NorthwindContext>()
            .UseInMemoryDatabase(databaseName: "Example_DB")
            .Options;

        _context = new NorthwindContext(options);
        _sut = new CustomerService(_context);

        // Seed the database
        _sut.CreateCustomer(new Customer { CustomerId = "Phill", ContactName = "Philip Windridge", CompanyName = "Sparta Global", City = "Birmingham" });
        _sut.CreateCustomer(new Customer { CustomerId = "Manda", ContactName = "Nish Mandal", CompanyName = "Sparta Global", City = "Birmingham" });
    }

    [Test]
    public void GiveAValidId_CorrectCustomerIsReturned()
    {
        var result = _sut.GetCustomerById("Phill");

        Assert.That(result, Is.TypeOf<Customer>());
        Assert.That(result.ContactName, Is.EqualTo("Philip Windridge"));
        Assert.That(result.CompanyName, Is.EqualTo("Sparta Global"));
        Assert.That(result.City, Is.EqualTo("Birmingham"));
    }

    [Test]
    public void GivenANewCustomer_CreateCustomerAddsItToTheDatabase()
    {
        var numberOfCustomersBefore = _context.Customers.Count();
        var newCustomer = new Customer
        {
            CustomerId = "ODELL",
            ContactName = "Max Odell",
            CompanyName = "Sparta Global",
            City = "Surrey"
        };

        _sut.CreateCustomer(newCustomer);
        var numberOfCustomersAfter = _context.Customers.Count();
        var result = _sut.GetCustomerById("ODELL");

        Assert.That(numberOfCustomersBefore + 1, Is.EqualTo(numberOfCustomersAfter));
        Assert.That(result, Is.TypeOf<Customer>());
        Assert.That(result.ContactName, Is.EqualTo("Max Odell"));
        Assert.That(result.CompanyName, Is.EqualTo("Sparta Global"));
        Assert.That(result.City, Is.EqualTo("Surrey"));
        Assert.That(newCustomer, Is.EqualTo(result));

        // Cleanup
        _context.Customers.Remove(newCustomer);
        _context.SaveChanges();
    }

    // GetCustomerList
    [Test]
    public void GetCustomerList_ReturnsTheCorrectNumberOfCustomers()
    {
        int countOfAllCustomers = _context.Customers.Count();
        var customerList = _sut.GetCustomerList();

        Assert.That(customerList, Is.TypeOf<List<Customer>>());
        Assert.That(customerList.Count, Is.EqualTo(countOfAllCustomers));
    }

    [Test]
    public void GetCustomerList_ReturnsTheCorrect_FirstAndLast_Customers()
    {
        var customerList = _sut.GetCustomerList();

        Assert.That(customerList[0], Is.EqualTo(_context.Customers.First()));
        Assert.That(customerList[customerList.Count - 1], Is.EqualTo(_context.Customers.Last()));
    }

    // RemoveCustomer
    [Test]
    public void GivenACustomer_RemoveCustomer_RemovesTheCustomer()
    {
        Customer Sahil = new Customer { CustomerId = "Sahil", ContactName = "Sahil Singh", CompanyName = "Sparta Global", City = "London" };

        _sut.CreateCustomer(Sahil);

        Assert.That(Sahil, Is.EqualTo(_context.Customers.Where(c => c.CustomerId == Sahil.CustomerId).FirstOrDefault()));

        _sut.RemoveCustomer(Sahil);

        Assert.That(_context.Customers.Where(c => c.CustomerId == Sahil.CustomerId), Is.Empty);
    }
}
