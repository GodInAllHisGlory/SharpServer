// Logs important information about the response and request objects as they come through
class LoggingMiddleware
{
    public static MiddlewareDelegate Factory(MiddlewareDelegate next)
    {
        return (request, responseHeaders) =>
        {
            Console.WriteLine("Request Received:");
            Console.WriteLine(request.Verb);
            Console.WriteLine(request.Path);

            Response response = next(request, responseHeaders);

            Console.WriteLine("Sending out response:");
            Console.WriteLine(response.Code + " " + response.Status);
            return response;
        };
    }
}