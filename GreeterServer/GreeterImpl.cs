using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Helloworld;

namespace GreeterServer
{
    class GreeterImpl : Greeter.GreeterBase
    {
        // Server side handler of the SayHello RPC
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply { Message = $"Hello {request.Name}" });
        }

        public async override Task LotsOfReplies(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            var responses = new[] { "hello", "again", request.Name };
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(new HelloReply { Message = response });
            }
        }

        public async override Task<HelloReply> GreetingsLog(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            var greetings = new List<string>();
            while (await requestStream.MoveNext())
            {
                greetings.Add(requestStream.Current.Name);
            }

            return new HelloReply { Message = $"Hello {string.Join(", ", greetings)}" };
        }

        public async override Task GreetingsPong(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var name = requestStream.Current.Name;
                await responseStream.WriteAsync(new HelloReply{ Message = $"Hello {name}"});
            }
        }
    }
}
