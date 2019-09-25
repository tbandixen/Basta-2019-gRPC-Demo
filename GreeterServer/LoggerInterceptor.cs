using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Newtonsoft.Json;

namespace GreeterServer
{
    public class LoggerInterceptor : Interceptor
    {
        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            Console.WriteLine($"\nRequest {context.Method}\n{request.ToJson()}");
            var response = base.UnaryServerHandler(request, context, continuation);
            Console.WriteLine($"\nResponse {context.Method}\n{response.ToJson()}");
            return response;
        }
    }
    internal static class RequestHelper
    {
        public static string ToJson<T>(this T obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
}
