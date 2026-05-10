class GetStatic
{
    public static MiddlewareDelegate Factory(MiddlewareDelegate next)
    {
        return (request, responseHeaders) =>
        {
            string path = request.Path;

            // Checks to make sure the request is for a static file
            if (!path.Contains(".")) return next(request, responseHeaders);

            Response response = GET.Get($"static{path}", responseHeaders);
            response.Headers["Content-Type"] = GetType(path);
            return response;
        };
    }

    public static string GetType(string file)
    {
        string extension = file.Split(".", 2)[1];

        switch (extension)
        {
            case "js":
                return "text/javascript";
            case "css":
                return "text/css";
            case "png":
                return "image/png";
            default:
                return "none";
        }
    }
}