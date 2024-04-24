using Grpc.Net.Client;
using GrpcSandbox.Core.Protos;
using static GrpcSandbox.Core.Protos.CustomerService;


var channel = GrpcChannel.ForAddress("http://localhost:5117");

var client = new CustomerServiceClient(channel);

var input = new CustomerLookupRequest
{
    UserId = 4
};

var reply = await client.GetCustomerInfoAsync(input);
Console.WriteLine($"{reply.FirstName} {reply.LastName}");

using var call = client.GetNewCustomers(new NewCustomerRequest());
while (await call.ResponseStream.MoveNext(default))
{
    var current = call.ResponseStream.Current;
    Console.WriteLine($"{current.FirstName} {current.LastName}");
}
