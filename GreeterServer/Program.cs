using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Helloworld;

namespace GreeterServer
{
    class Program
    {
        const int Port = 50051;

        public static async Task Main(string[] args)
        {
            Server server = new Server
            {
                Services = { Greeter.BindService(new GreeterImpl()).Intercept(new LoggerInterceptor()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Server listening on port " + Port);
            Console.WriteLine("Press enter to stop the server...");

            while (Console.ReadLine().ToLower() == "cls")
            {
                Console.Clear();
            }

            await server.ShutdownAsync();
        }
    }
}
