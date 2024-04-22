namespace GrpcSandbox.WorkerService;

using Grpc.Net.Client;
using static GrpcSandbox.Core.Protos.CustomerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly int customerId;
    private readonly string serviceUrl;
    private readonly CustomerServiceClient client;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        this.customerId = configuration.GetValue<int>("CustomerId");
        this.serviceUrl = configuration["ServerUrl"];
        var channel = GrpcChannel.ForAddress(this.serviceUrl);
        this.client = new CustomerServiceClient(channel);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            var customer = this.client.GetCustomerInfo(new Core.Protos.CustomerLookupRequest { UserId = this.customerId });

            Console.WriteLine(customer.FirstName + " " + customer.LastName);

            await Task.Delay(1000, stoppingToken);
        }
    }
}
