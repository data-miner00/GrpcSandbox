namespace GrpcSandbox.Core.Repositories;

using GrpcSandbox.Core.Protos;

/// <summary>
/// The abstraction for a customer repository.
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// Gets all the customers.
    /// </summary>
    /// <returns>The list of customers.</returns>
    IEnumerable<Customer> GetAll();

    /// <summary>
    /// Retrieve customer by Id.
    /// </summary>
    /// <param name="id">The customer Id.</param>
    /// <returns>The customer.</returns>
    Customer? GetById(int id);

    /// <summary>
    /// Edit a customer.
    /// </summary>
    /// <param name="customer">The customer to be replaced.</param>
    void Edit(Customer customer);

    /// <summary>
    /// Adds a customer.
    /// </summary>
    /// <param name="customer">The customer to be added.</param>
    void Add(Customer customer);

    /// <summary>
    /// Deletes a customer.
    /// </summary>
    /// <param name="id">The customer Id.</param>
    void Delete(int id);
}
