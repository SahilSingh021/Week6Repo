using Microsoft.EntityFrameworkCore;
using Moq;
using NorthwindBusiness;
using NorthwindData;
using NorthwindData.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindTests;
public class CustomerManagerShould
{
    private CustomerManager _sut;

    [Ignore("Should Fail")]
    [Test]
    public void BeAbleToConstructCustomerManager()
    {
        _sut = new CustomerManager(null);   // Use Moq in this case...
        Assert.That(_sut, Is.InstanceOf<CustomerManager>());
    }

    [Test]
    public void BeAbleToConstruct_UsingMoq()
    {
        var mockObject = new Mock<ICustomerService>();
        _sut = new CustomerManager(mockObject.Object);
        Assert.That(_sut, Is.InstanceOf<CustomerManager>());
    }

    // Stub
    [Category("Happy Path")]
    [Test]
    public void ReturnTrue_WhenUpdateIsCalled_WithValidId()
    {
        // Arrange
        var mockObject = new Mock<ICustomerService>();
        var originalCustomer = new Customer
        {
            CustomerId = "Manda"
        };

        mockObject.Setup(cs => cs.GetCustomerById("Manda")).Returns(originalCustomer);
        _sut = new CustomerManager(mockObject.Object);

        //Act
        var reault = _sut.Update("Manda", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.That(reault);
    }

    [Category("Happy Path")]
    [Test]
    public void UpdateSelectedCustomer_WhenUpdateIsCalled_WithValidId()
    {
        // Arrange
        var mockObject = new Mock<ICustomerService>();
        var originalCustomer = new Customer
        {
            CustomerId = "Manda",
            ContactName = "Nish Mandal",
            CompanyName = "Sparta Global",
            City = "Birmingham"
        };

        mockObject.Setup(cs => cs.GetCustomerById("Manda")).Returns(originalCustomer);

        _sut = new CustomerManager(mockObject.Object);

        //Act
        var reault = _sut.Update("Manda", "Nish Mandal", "UK", "London", null);
        Assert.That(_sut.SelectedCustomer.ContactName, Is.EqualTo("Nish Mandal"));
        Assert.That(_sut.SelectedCustomer.Country, Is.EqualTo("UK"));
        Assert.That(_sut.SelectedCustomer.City, Is.EqualTo("London"));
    }

    [Category("Sad Path")]
    [Test]
    public void ReturnsFalse_WhenUpdateIsCalled_WithInValidId()
    {
        // Arrange
        var mockObject = new Mock<ICustomerService>();

        mockObject.Setup(cs => cs.GetCustomerById(It.IsAny<string>())).Returns((Customer)null);

        _sut = new CustomerManager(mockObject.Object);

        //Act
        var reault = _sut.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

        Assert.That(reault, Is.False);
    }

    [Category("Sad Path")]
    [Test]
    public void NotChnageTheSelectedCustomer_WhenUpdateIsCalled_WithValidId()
    {
        var mockObject = new Mock<ICustomerService>();

        mockObject.Setup(cs => cs.GetCustomerById("Manda")).Returns((Customer)null);

        var originalCustomer = new Customer
        {
            CustomerId = "Manda",
            ContactName = "Nish Mandal",
            CompanyName = "Sparta Global",
            City = "Birmingham"
        };

        _sut = new CustomerManager(mockObject.Object);
        _sut.SetSelectedCustomer(originalCustomer);

        var reault = _sut.Update("Manda", "Nish Mandal", "UK", "London", null);
        Assert.That(_sut.SelectedCustomer.ContactName, Is.EqualTo("Nish Mandal"));
        Assert.That(_sut.SelectedCustomer.Country, Is.EqualTo(null));
        Assert.That(_sut.SelectedCustomer.City, Is.EqualTo("Birmingham"));
    }

    // Using mocks, create happy and sad path tests for the delete method
    [Category("Happy Path")]
    [Test]
    public void ReturnTrue_WhenDeleteIsCalled_WithValidId()
    {
        // Arrange
        var mockObject = new Mock<ICustomerService>();

        Customer Nish = new Customer
        {
            CustomerId = "Manda"
        };

        mockObject.Setup(mo => mo.GetCustomerById("Manda")).Returns(Nish);

        _sut = new CustomerManager(mockObject.Object);

        var result = _sut.Delete("Manda");

        Assert.That(result, Is.True);
    }

    [Category("Happy Path")]
    [Test]
    public void CustomerIsDeltedWhen_Delete_IsCalledWithValidId()
    {
        // Arrange
        var mockObject = new Mock<ICustomerService>();

        Customer Nish = new Customer
        {
            CustomerId = "Manda",
            ContactName = "Nish Mandal",
            CompanyName = "Sparta Global",
            City = "Birmingham"
        };

        mockObject.Setup(mo => mo.GetCustomerById("Manda")).Returns(Nish);

        _sut = new CustomerManager(mockObject.Object);

        var result = _sut.Delete("Manda");

        Assert.That(result, Is.True);

        var userEists = _sut.RetrieveAll();

        Assert.That(userEists, Is.Null);
    }

    [Category("Sad Path")]
    [Test]
    public void ReturnsFalse_WhenDeleteIsCalled_WithInValidId()
    {
        var mockObject = new Mock<ICustomerService>();

        mockObject.Setup(cs => cs.GetCustomerById(It.IsAny<string>())).Returns((Customer)null);

        _sut = new CustomerManager(mockObject.Object);

        var reault = _sut.Delete(It.IsAny<string>());

        Assert.That(reault, Is.False);
    }

    // Using Moq to throw exceptions
    [Category("Sad Path")]
    [Category("Update")]
    [Test]
    public void ReturnFalse_WhenUpdateIsCalled_AndDatabaseThrowsException()
    {
        var mockCustomerService = new Mock<ICustomerService>();

        mockCustomerService.Setup(cs => cs.GetCustomerById(It.IsAny<string>())).Returns(new Customer());
        mockCustomerService.Setup(cs => cs.SaveCustomerChnages()).Throws<DbUpdateConcurrencyException>();

        _sut = new CustomerManager(mockCustomerService.Object);

        var result = _sut.Update(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>());
        Assert.That(result, Is.False);
    }

    // Behaviour based testing
    [Test]
    public void CallSaveCustomerChnages_WhenUpdateIsCalled_WithId()
    {
        var mockCustomerService = new Mock<ICustomerService>();

        mockCustomerService.Setup(cs => cs.GetCustomerById(It.IsAny<string>())).Returns(new Customer());

        _sut = new CustomerManager(mockCustomerService.Object);

        var result = _sut.Update(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>());

        // Assert That
        mockCustomerService.Verify(cs => cs.SaveCustomerChnages(), Times.Once);
        //mockCustomerService.Verify(cs => cs.SaveCustomerChnages(), Times.Exactly(1));
        //mockCustomerService.Verify(cs => cs.SaveCustomerChnages(), Times.AtMostOnce);
        //mockCustomerService.Verify(cs => cs.GetCustomerList(), Times.Never);
    }

    [Test]
    public void LetsSeeWhatHappenes_WhenUpdateIsCalled_IfAllInvocations_ArentSetup()
    {
        var mockCustomerService = new Mock<ICustomerService>(MockBehavior.Strict);

        mockCustomerService.Setup(cs => cs.GetCustomerById(It.IsAny<string>())).Returns(new Customer());
        mockCustomerService.Setup(cs => cs.SaveCustomerChnages());

        _sut = new CustomerManager(mockCustomerService.Object);

        var result = _sut.Update(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>());

        Assert.That(result);
    }

    // Use stubs to verify that RetrieveAll and SetSelectedCustomer methods work as expected.
    // Dosnt Work
    [Ignore("Dosnt Work Yet")]
    [Test]
    public void Calling_RetrieveAll_RetrunsCorrectCountOfCustomersInDB()
    {
        var mockObject = new Mock<ICustomerService>();

        mockObject.Setup(cs => cs.GetCustomerList()).Returns(It.IsAny<List<Customer>>());

        _sut = new CustomerManager(mockObject.Object);

        _sut.Create("101", "Nish", "Sparta Gbobal");
        _sut.Create("102", "Phill", "Sparta Gbobal");
        var result = _sut.RetrieveAll();

        Assert.That(result, Is.TypeOf<List<Customer>>());
    }

    [Test]
    public void Calling_SetSelectedCustomer_RetrunsCorrectCountOfCustomersInDB()
    {
        var mockObject = new Mock<ICustomerService>();

        Customer Nish = new Customer
        {
            CustomerId = "Manda",
            ContactName = "Nish Mandal",
            CompanyName = "Sparta Global",
            City = "Birmingham"
        };

        mockObject.Setup(cs => cs.GetCustomerList()).Returns(new List<Customer> { Nish });

        _sut = new CustomerManager(mockObject.Object);

        _sut.SetSelectedCustomer(Nish);
        var result = _sut.SelectedCustomer;
        Assert.That(result, Is.TypeOf<Customer>());
        Assert.That(result, Is.EqualTo(Nish));
    }

    // Write a behavior-based tests to verify that that the CustomerManager Create and Delete methods call the expected CustomerService methods.
    [Test]
    public void Calling_Create_CallsTheExpectedCustomerServiceMethods()
    {
        var mockObject = new Mock<ICustomerService>();

        mockObject.Setup(cs => cs.CreateCustomer(It.IsAny<Customer>()));

        _sut = new CustomerManager(mockObject.Object);

        _sut.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

        mockObject.Verify(cs => cs.CreateCustomer(It.IsAny<Customer>()), Times.Once);
    }

    [Test]
    public void Calling_Delete_CallsTheExpectedCustomerServiceMethods()
    {
        var mockObject = new Mock<ICustomerService>();

        Customer Nish = new Customer
        {
            CustomerId = "Manda",
            ContactName = "Nish Mandal",
            CompanyName = "Sparta Global",
            City = "Birmingham"
        };

        mockObject.Setup(mo => mo.GetCustomerById("Manda")).Returns(Nish);

        _sut = new CustomerManager(mockObject.Object);

        _sut.Delete("Manda");

        mockObject.Verify(cs => cs.RemoveCustomer(Nish), Times.Once);
    }
}
