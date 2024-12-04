namespace GrpcSandbox.Server.Repositories;

using System.Linq;
using Bogus;
using GrpcSandbox.Core.Protos;
using Google.Protobuf.WellKnownTypes;
using GrpcSandbox.Core.Repositories;

/// <summary>
/// A dummy in-memory customer repository.
/// </summary>
public sealed class CustomerRepository : ICustomerRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
    /// </summary>
    public CustomerRepository()
    {
        if (false)
        {
            Randomizer.Seed = new Random();
        }

        var addressFake = new Faker<Address>()
            .RuleFor(x => x.Line1, s => s.Address.BuildingNumber())
            .RuleFor(x => x.Line2, s => s.Address.StreetAddress())
            .RuleFor(x => x.Line3, s => s.Address.SecondaryAddress())
            .RuleFor(x => x.Country, s => s.Address.Country())
            .RuleFor(x => x.Province, s => s.Address.State())
            .RuleFor(x => x.County, s => s.Address.County())
            .RuleFor(x => x.PostCode, s => s.Address.ZipCode());

        var customerFake = new Faker<Customer>()
            .RuleFor(x => x.Id, s => s.IndexFaker)
            .RuleFor(x => x.FirstName, s => s.Name.FirstName())
            .RuleFor(x => x.LastName, s => s.Name.LastName())
            .RuleFor(x => x.Age, s => s.Random.Int(20, 80))
            .RuleFor(x => x.EmailAddress, (s, o) => s.Internet.Email(o.FirstName, o.LastName))
            .RuleFor(x => x.Receipts, (s, o) =>
            {
                o.Receipts
                    .AddRange(
                        Enumerable
                            .Range(0, 3)
                            .Select(x => Guid.NewGuid().ToString()).ToList());
                return o.Receipts;
            })
            .RuleFor(x => x.Membership, s => s.PickRandom<Membership>())
            .RuleFor(x => x.BillingAddress, s => addressFake.Generate())
            .RuleFor(x => x.ShippingAddress, s => addressFake.Generate())
            .RuleForType(typeof(Timestamp), s => s.Date.Past().ToUniversalTime().ToTimestamp());

        this.Customers = customerFake.GenerateBetween(10, 20);
    }

    /// <summary>
    /// Gets the list of customers.
    /// </summary>
    public List<Customer> Customers { get; init; }

    /// <inheritdoc/>
    public IEnumerable<Customer> GetAll()
    {
        return this.Customers;
    }

    /// <inheritdoc/>
    public Customer? GetById(int id)
    {
        return this.Customers.SingleOrDefault(x => x.Id == id);
    }

    /// <inheritdoc/>
    public void Edit(Customer customer)
    {
        var found = this.Customers.Find(x => x.Id == customer.Id);

        if (found is not null)
        {
            this.Customers.Remove(found);
            this.Customers.Add(customer);
        }
    }

    /// <inheritdoc/>
    public void Add(Customer customer)
    {
        this.Customers.Add(customer);
    }

    /// <inheritdoc/>
    public void Delete(int id)
    {
        var found = this.Customers.Find(x => x.Id == id);

        if (found is not null)
        {
            this.Customers.Remove(found);
        }
    }
}
