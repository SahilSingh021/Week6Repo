using NUnit.Framework;
using NorthwindBusiness;
using NorthwindData;
using System.Linq;

namespace NorthwindTests
{
    public class CustomerTests
    {
        CustomerManager _customerManager;
        [SetUp]
        public void Setup()
        {
            _customerManager = new CustomerManager();
            // remove test entry in DB if present
            using (var db = new NorthwindContext())
            {
                var selectedCustomers =
                from c in db.Customers
                where c.CustomerId == "MANDA"
                select c;

                db.Customers.RemoveRange(selectedCustomers);
                db.SaveChanges();
            }
        }

        [Test]
        public void WhenANewCustomerIsAdded_TheNumberOfCustemersIncreasesBy1()
        {
            using (var db = new NorthwindContext())
            {
                var numberOfCustomersBefore = db.Customers.Count();
                _customerManager.Create("MANDA", "Nish Mandal", "Sparta Global");
                var numberOfCustomersAfter = db.Customers.Count();
                Assert.That(numberOfCustomersBefore + 1, Is.EqualTo(numberOfCustomersAfter));
            }
        }

        [Test]
        public void WhenANewCustomerIsAdded_TheirDetailsAreCorrect()
        {
            using (var db = new NorthwindContext())
            {
                _customerManager.Create("MANDA", "Nish Mandal", "Sparta Global");
                var selectedCustomer = db.Customers.Find("MANDA");
                Assert.That(selectedCustomer.ContactName, Is.EqualTo("Nish Mandal"));
                Assert.That(selectedCustomer.CompanyName, Is.EqualTo("Sparta Global"));
            }
        }

        [Test]
        public void WhenACustomersDetailsAreChanged_TheDatabaseIsUpdated()
        {
            using (var db = new NorthwindContext())
            {
                _customerManager.Create("MANDA", "Nish Mandal", "Sparta Global", "Berlin");
                _customerManager.Update("MANDA", "Nish Manda", "England", "London", "RM10 4JK");
                var selectedCustomer = db.Customers.Find("MANDA");
                Assert.That(selectedCustomer.City, Is.EqualTo("London"));
                Assert.That(selectedCustomer.PostalCode, Is.EqualTo("RM10 4JK"));
            }
        }

        [Test]
        public void WhenACustomerIsUpdated_SelectedCustomerIsUpdated()
        {
            using (var db = new NorthwindContext())
            {
                _customerManager.Create("MANDA", "Nish Mandal", "Sparta Global");
                _customerManager.Update("MANDA", "NISH MANDAL", "England", "London", "RM8 P69");
                var selectedCustomer = db.Customers.Find("MANDA");
                Assert.That(selectedCustomer.ContactName, Is.EqualTo("NISH MANDAL"));
                Assert.That(selectedCustomer.CompanyName, Is.EqualTo("Sparta Global"));
                Assert.That(selectedCustomer.Country, Is.EqualTo("England"));
                Assert.That(selectedCustomer.City, Is.EqualTo("London"));
                Assert.That(selectedCustomer.PostalCode, Is.EqualTo("RM8 P69"));
            }
        }

        [Test]
        public void WhenACustomerIsNotInTheDatabase_Update_ReturnsFalse()
        {
            using (var db = new NorthwindContext())
            {
                var hasUpdatedNish = _customerManager.Update("MANDA", "Nish", "England", "London", "RM8 P69");
                Assert.That(hasUpdatedNish, Is.False);
            }
        }

        [Test]
        public void WhenACustomerIsRemoved_TheNumberOfCustomersDecreasesBy1()
        {
            using (var db = new NorthwindContext())
            {
                _customerManager.Create("MANDA", "Nish Mandal", "Sparta Global");
                var customerCountBefore = db.Customers.Count();
                _customerManager.Delete("MANDA");
                var customerCountAfter = db.Customers.Count();
                Assert.That(customerCountBefore - 1, Is.EqualTo(customerCountAfter));
            }
        }

        [Test]
        public void WhenACustomerIsRemoved_TheyAreNoLongerInTheDatabase()
        {
            using (var db = new NorthwindContext())
            {
                _customerManager.Create("MANDA", "Nish Mandal", "Sparta Global");
                var selectedCustomer = db.Customers.Find("MANDA");
                Assert.That(selectedCustomer.ContactName, Is.EqualTo("Nish Mandal"));
            }

            using (var db = new NorthwindContext())
            {
                _customerManager.Delete("MANDA");
                var findCustomer = db.Customers.Find("MANDA");
                Assert.That(findCustomer, Is.EqualTo(null));
            }
        }

        [TearDown]
        public void TearDown()
        {
            using (var db = new NorthwindContext())
            {
                var selectedCustomers =
                from c in db.Customers
                where c.CustomerId == "MANDA"
                select c;

                db.Customers.RemoveRange(selectedCustomers);
                db.SaveChanges();
            }
        }
    }
}