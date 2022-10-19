using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NorthwindData.Services;

public interface ICustomerService
{
    List<Customer> GetCustomerList();
    public Customer GetCustomerById(string CustomerId);
    void CreateCustomer(Customer c);
    void SaveCustomerChnages();
    void RemoveCustomer(Customer c);
}
