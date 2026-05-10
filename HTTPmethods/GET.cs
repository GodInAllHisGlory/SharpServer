using System.Data.SqlTypes;

class GET
{
    public static Response Get(string resourcePath, Dictionary<string , string> headers)
    {
        Response response;
        byte[] responseBody;
        try
        {
            responseBody = File.ReadAllBytes(resourcePath);
            response = new Response("HTTP/1.1", 200, "ok", responseBody, headers);
        } catch
        {
            //If the resource is not found a 404 is returned
            Console.WriteLine($"Could not find {resourcePath}");
            responseBody = File.ReadAllBytes("templates/404.html");
            response = new Response("HTTP/1.1", 404, "Page Not Found", responseBody, headers);
        }
        response.Headers["Content-Length"] = responseBody.Length.ToString();
        response.Headers["Content-Type"] = "text/html";

        return response;
    }
}