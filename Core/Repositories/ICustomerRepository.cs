namespace GrpcSandbox.Core.Repositories;

using GrpcSandbox.Core.Protos;

public interface ICustomerRepository
{
    IEnumerable<Customer> GetAll();

    Customer? GetById(int id);

    void Edit(Customer customer);

    void Add(Customer customer);

    void Delete(int id);
}
