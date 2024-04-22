namespace GrpcSandbox.Server.Repositories;

using System.Linq;
using GrpcSandbox.Core.Protos;

public sealed class CustomerRepository
{
    public List<Customer> Customers { get; } =
        [
            new Customer()
            {
                Id = 1,
                FirstName = "Ben",
                LastName = "Ten",
                EmailAddress = "ben.ten@gmail.com",
                Age = 23,
                IsAlive = true,
            },
            new Customer()
            {
                Id = 2,
                FirstName = "Louis",
                LastName = "McCarthy",
                EmailAddress = "louis.mcc@gmail.com",
                Age = 31,
                IsAlive = true,
            },
            new Customer()
            {
                Id = 3,
                FirstName = "Mariam",
                LastName = "House",
                EmailAddress = "maryam@gmail.com",
                Age = 28,
                IsAlive = true,
            }
        ];

    public IEnumerable<Customer> GetAll()
    {
        return this.Customers;
    }

    public Customer? GetById(int id)
    {
        return this.Customers.SingleOrDefault(x => x.Id == id);
    }

    public void Edit(Customer customer)
    {
        var found = this.Customers.Find(x => x.Id == customer.Id);

        if (found is not null)
        {
            this.Customers.Remove(found);
            this.Customers.Add(customer);
        }
    }

    public void Add(Customer customer)
    {
        this.Customers.Add(customer);
    }

    public void Delete(int id)
    {
        var found = this.Customers.Find(x => x.Id == id);

        if (found is not null)
        {
            this.Customers.Remove(found);
        }
    }
}
