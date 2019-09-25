using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Helloworld;

namespace GreeterClient
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine();
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var client = new Greeter.GreeterClient(channel);

            var request = new HelloRequest { Name = "Thomas" };

            #region Unary RPC
            Log("Unary RPC");

            var reply = client.SayHello(request);
            Log("Greeting: " + reply.Message);

            try
            {
                var secondReply = client.SayHello(request, deadline: DateTime.UtcNow.AddMilliseconds(2));
                Log("Greeting: " + secondReply.Message);
            }
            catch (RpcException e)
            {
                Log(e.Status.Detail);
            }

            Console.WriteLine();
            #endregion

            #region Server streaming RPC
            Log("Server streaming RPC");

            using (var call = client.LotsOfReplies(request))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var streamReply = call.ResponseStream.Current;
                    Log(streamReply.ToString());
                }
            }

            Console.WriteLine();
            #endregion

            #region Client streaming RPC
            Log("Client streaming RPC");

            using (var call = client.GreetingsLog())
            {

                var names = new[] { "Thomas", "Max", "Ben" };
                foreach (var name in names)
                {
                    await call.RequestStream.WriteAsync(new HelloRequest { Name = name });
                }
                await call.RequestStream.CompleteAsync();

                var greetingsReply = await call.ResponseAsync;
                Log(greetingsReply.Message);
            }

            Console.WriteLine();
            #endregion

            #region Bidirectional RPC
            Log("Bidirectional RPC");

            using (var call = client.GreetingsPong())
            {
                var readerTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var msg = call.ResponseStream.Current.Message;
                        Log(msg);
                    }
                });
                var names = new[] { "Thomas", "Max", "Ben" };
                foreach (var name in names)
                {
                    await call.RequestStream.WriteAsync(new HelloRequest { Name = name });
                }
                await call.RequestStream.CompleteAsync();
                await readerTask;
            }

            Console.WriteLine();
            #endregion

            await channel.ShutdownAsync();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        private static void Log(string message)
        {
            Console.WriteLine($"\t{message}");
        }

        private static void WaitForKey()
        {
            Console.Write("...");
            Console.ReadKey(true);
        }
    }
}
