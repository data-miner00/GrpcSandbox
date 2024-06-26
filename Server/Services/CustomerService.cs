namespace GrpcSandbox.Server.Services;

using Grpc.Core;
using GrpcSandbox.Server.Repositories;
using System.Threading.Tasks;
using gCustomerService = GrpcSandbox.Core.Protos.CustomerService;

public class CustomerService : gCustomerService.CustomerServiceBase
{
    private readonly CustomerRepository repository;

    public CustomerService(CustomerRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this.repository = repository;
    }

    public override Task<GrpcSandbox.Core.Protos.Customer> GetCustomerInfo(GrpcSandbox.Core.Protos.CustomerLookupRequest request, ServerCallContext context)
    {
        return Task.Run(() => this.repository.GetById(request.UserId));
    }

    public override async Task GetNewCustomers(GrpcSandbox.Core.Protos.NewCustomerRequest request, IServerStreamWriter<GrpcSandbox.Core.Protos.Customer> responseStream, ServerCallContext context)
    {
        var customers = this.repository.GetAll();

        foreach (var customer in customers)
        {
            await Task.Delay(1000);
            await responseStream.WriteAsync(customer);
        }
    }
}
