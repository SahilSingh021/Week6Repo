using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindData.Services;

public class CustomerService : ICustomerService
{
    public readonly NorthwindContext _context;

    public CustomerService()
    {
        _context = new NorthwindContext();
    }

    public CustomerService(NorthwindContext context)
    {
        _context = context;
    }

    public void CreateCustomer(Customer c)
    {
        _context.Customers.Add(c);
        _context.SaveChanges();
    }

    public Customer GetCustomerById(string customerId) => _context.Customers.Find(customerId);
    public List<Customer> GetCustomerList() => _context.Customers.ToList();
    public void RemoveCustomer(Customer c)
    {
        _context.Customers.Remove(c);
        _context.SaveChanges();
    }

    public void SaveCustomerChnages()
    {
        _context.SaveChanges();
    }
}
