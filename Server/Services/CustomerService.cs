namespace GrpcSandbox.Server.Services;

using Grpc.Core;
using GrpcSandbox.Core;
using GrpcSandbox.Server.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using gCustomerService = GrpcSandbox.Core.Protos.CustomerService;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CustomerService : gCustomerService.CustomerServiceBase
{
    private readonly CustomerRepository repository;
    private readonly JwtTokenValidator validator;

    public CustomerService(CustomerRepository repository, JwtTokenValidator validator)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this.repository = repository;
        this.validator = validator;
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

    [AllowAnonymous]
    public override async Task<Core.Protos.TokenResponse> GenerateToken(Core.Protos.TokenRequest request, ServerCallContext context)
    {
        var result = await this.validator.GenerateTokenAsync(request);

        return result;
    }
}
