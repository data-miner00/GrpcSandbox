namespace GrpcSandbox.Server.Services;

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcSandbox.Core.Protos;
using System.Threading.Tasks;
using static GrpcSandbox.Core.Protos.DummyService;

public class DummyService : DummyServiceBase
{
    public override async Task<Empty> AcceptStreaming(IAsyncStreamReader<DummyRequest> requestStream, ServerCallContext context)
    {
        while (await requestStream.MoveNext())
        {
            var current = requestStream.Current;
            Console.WriteLine(current.Payload);
        }

        return new();
    }

    public override async Task BidirectionStreaming(
        IAsyncStreamReader<DummyRequest> requestStream,
        IServerStreamWriter<DummyResponse> responseStream,
        ServerCallContext context)
    {
        while (await requestStream.MoveNext())
        {
            var current = requestStream.Current;
            Console.WriteLine(current.Payload);

            if (current.Payload % 5 == 0)
            {
                await responseStream.WriteAsync(new DummyResponse
                {
                    Response = current.Payload,
                });
            }
        }
    }
}
