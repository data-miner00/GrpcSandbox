namespace GrpcSandbox.WorkerService;

using Grpc.Core;
using Grpc.Net.Client;
using GrpcSandbox.Core;
using System.Security.Cryptography.X509Certificates;

using static GrpcSandbox.Core.Protos.CustomerService;
using static GrpcSandbox.Core.Protos.DummyService;

/// <summary>
/// Sample background worker.
/// </summary>
public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;
    private readonly int customerId;
    private readonly CustomerServiceClient client;
    private readonly DummyServiceClient dummy;

    private string token = string.Empty;
    private DateTime expiration = DateTime.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Worker"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="configuration">The configuration object.</param>
    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        this.logger = Guard.ThrowIfNull(logger);
        this.customerId = configuration.GetValue<int>("CustomerId");
        var serviceUrl = Guard.ThrowIfNullOrWhitespace(configuration["ServerUrl"]);
        var channel = GrpcChannel.ForAddress(serviceUrl);
        this.client = new CustomerServiceClient(channel);

        var certName = configuration["Certificates:Name"];
        var certPassword = configuration["Certificates:Password"];

        var cert = new X509Certificate2(certName, certPassword);
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(cert);

        var httpClient = new HttpClient(handler);
        var options = new GrpcChannelOptions
        {
            HttpClient = httpClient,
        };

        var certificateChannel = GrpcChannel.ForAddress(serviceUrl, options);
        this.dummy = new DummyServiceClient(certificateChannel);
    }

    private bool RequiresLogin => string.IsNullOrEmpty(this.token) || this.expiration <= DateTime.Now;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (!this.RequiresLogin || await this.RequestToken())
            {
                var headers = new Metadata
                {
                    { "Authorization", $"Bearer {this.token}" },
                };

                while (!stoppingToken.IsCancellationRequested)
                {
                    if (this.logger.IsEnabled(LogLevel.Information))
                    {
                        this.logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
                    }

                    this.LookupCustomer(headers);

                    await this.PushStream();

                    await Task.Delay(600_000, stoppingToken);
                }
            }
            else
            {
                this.logger.LogError("Failed to get JWT token");
            }
        }
        catch (RpcException ex)
        {
            this.logger.LogError(ex, "Error occurred: {Message}", ex.Message);
        }
    }

    private void LookupCustomer(Metadata metadata)
    {
        var customer = this.client.GetCustomerInfo(
            new Core.Protos.CustomerLookupRequest { UserId = this.customerId },
            metadata);

        Console.WriteLine(customer.FirstName + " " + customer.LastName);
    }

    private async Task PushStream()
    {
        using var stream = this.dummy.AcceptStreaming();

        for (var i = 0; i < 20; i++)
        {
            await stream.RequestStream.WriteAsync(new Core.Protos.DummyRequest
            {
                Payload = Random.Shared.Next(),
            });
        }
    }

    private async Task<bool> RequestToken()
    {
        try
        {
            var result = await this.client.GenerateTokenAsync(new Core.Protos.TokenRequest
            {
                Username = "shaun",
                Password = "password*",
            });

            if (result.Success)
            {
                this.token = result.Token;
                this.expiration = result.Expiration.ToDateTime();

                return true;
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Something wrong: {Message}", ex.Message);
        }

        return false;
    }

    [Obsolete("Unsafe to call in worker")]
    private async Task BidirectionStream()
    {
        using var stream = this.dummy.BidirectionStreaming();

        for (var i = 0; i < 20; i++)
        {
            await stream.RequestStream.WriteAsync(new Core.Protos.DummyRequest
            {
                Payload = Random.Shared.Next(),
            });
        }
    }
}
