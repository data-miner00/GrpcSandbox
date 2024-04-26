namespace GrpcSandbox.WorkerService;

using Grpc.Net.Client;
using static GrpcSandbox.Core.Protos.CustomerService;
using static GrpcSandbox.Core.Protos.DummyService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly int customerId;
    private readonly string serviceUrl;
    private readonly CustomerServiceClient client;
    private readonly DummyServiceClient dummy;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        this.customerId = configuration.GetValue<int>("CustomerId");
        this.serviceUrl = configuration["ServerUrl"];
        var channel = GrpcChannel.ForAddress(this.serviceUrl);
        this.client = new CustomerServiceClient(channel);
        this.dummy = new DummyServiceClient(channel);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            this.LookupCustomer();

            await this.PushStream();

            await Task.Delay(3000, stoppingToken);
        }
    }

    private void LookupCustomer()
    {
        var customer = this.client.GetCustomerInfo(new Core.Protos.CustomerLookupRequest { UserId = this.customerId });

        Console.WriteLine(customer.FirstName + " " + customer.LastName);
    }

    private async Task PushStream()
    {
        using var stream = this.dummy.AcceptStreaming();

        for (var i = 0; i < 20; i++)
        {
            await stream.RequestStream.WriteAsync(new Core.Protos.DummyRequest
            {
                Payload = Random.Shared.Next()
            });
        }
    }
}
