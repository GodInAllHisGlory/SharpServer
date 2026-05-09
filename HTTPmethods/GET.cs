class GET
{
    public static Response Get(string resourcePath, Dictionary<string , string> headers)
    {
        Response response;
        string responseBody;
        try
        {
            responseBody = File.ReadAllText($"templates/{resourcePath}");
            response = new Response("HTTP/1.1", 200, "ok", responseBody, headers);
        } catch
        {
            //If the resource is not found a 404 is returned
            Console.WriteLine($"Could not find templates/{resourcePath}");
            responseBody = File.ReadAllText("templates/404.html");
            response = new Response("HTTP/1.1", 404, "Page Not Found", responseBody, headers);
        }
        response.Headers.Add("Content-Length", responseBody.Length.ToString());
        response.Headers.Add("Content-Type", "text/html");

        return response;
    }
}