// Adds important headers needed for each response object
class HeadersMiddleware
{
    public static MiddlewareDelegate Factory(MiddlewareDelegate next)
    {
        return (request, responseHeaders) =>
        {
            var headers = new Dictionary<string, string>(responseHeaders)
            {
                ["Connection"] = "close",
                ["Cache-Control"] = "max-age=20",
                ["Server"] = "Really Cool Server",
                ["Date"] = DateTime.UtcNow.ToString("r")
            };

            return next(request, headers);
        };
    }
}
