using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

public delegate Response MiddlewareDelegate(Request request, Dictionary<string, string> responseHeaders);

class Sharpserver
{
    static void Main(string[] args)
    {
        const int PORT = 8000;
        Uri uri = new Uri("http://127.0.0.1");
        Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        List<Endpoint> endpoints = new List<Endpoint>();

        // Creates the endpoints found in endpoints.json when the server is first started.
        // If the endpoints cannot be created then the server does not start.
        if (!CreateEndpoints("endpoints", endpoints)) return;
        Console.WriteLine(endpoints[0].Path);

        // Bind the socket to the local endpoint
        socket.Bind(new IPEndPoint(IPAddress.Parse(uri.Host), PORT));

        // Listen for incoming connections
        socket.Listen(10);
        Console.WriteLine("Server listening on " + uri.Host + ":" + PORT);

        // Accept a connection
        byte[] buffer = new byte[4096];
        Socket clientSocket = socket.Accept();
        int bytesReceived = clientSocket.Receive(buffer);

        // Fulfill request
        Request httpRequest = Decoder(buffer, bytesReceived);
        Console.WriteLine("Verb: " + httpRequest.Verb + " Path: " + httpRequest.Path + " Version: " + httpRequest.Version);
        Console.WriteLine("Accept-Encoding: " + httpRequest.GetHeader("Accept-Encoding"));
        Console.WriteLine("Client connected: " + clientSocket.RemoteEndPoint);

        MiddlewareDelegate app = BuildPipeline(
            new List<Func<MiddlewareDelegate, MiddlewareDelegate>>
            {
                HeadersMiddleware.Factory
            },
            (request, responseHeaders) => router(endpoints, request, responseHeaders)
        );

        Response response = app(httpRequest, new Dictionary<string, string>());
        byte[] responseBytes = Encoder(response);
        clientSocket.Send(responseBytes);

        // Close sockets
        clientSocket.Close();
        socket.Close();
    }

    public static MiddlewareDelegate BuildPipeline(List<Func<MiddlewareDelegate, MiddlewareDelegate>> middleware, MiddlewareDelegate terminal)
    {
        MiddlewareDelegate pipeline = terminal;
        for (int i = middleware.Count - 1; i >= 0; i--)
        {
            pipeline = middleware[i](pipeline);
        }
        return pipeline;
    }

    public static Response router(List<Endpoint> endpoints, Request request, Dictionary<string, string> responseHeaders)
    {
        string resource = string.Empty;
        foreach (Endpoint endpoint in endpoints)
        {
            if (request.Path == endpoint.Path)
            {
                resource = endpoint.ResourcePath;
                break;
            }
        }

        Response response;
        switch (request.Verb)
        {
            case "GET":
                response = GET.Get(resource, responseHeaders);
                break;
            default:
                response = new Response("HTTP/1.1", 501, "Not Implemented", string.Empty, responseHeaders);
                break;
        }
        return response;
    }

    // Turns the request from the client into a request object
    public static Request Decoder(byte[] buffer, int bytesReceived)
    {
        char[] responseChars = new char[bytesReceived];
        Encoding.ASCII.GetChars(buffer, 0, bytesReceived, responseChars, 0);

        Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string rawRequest = new string(responseChars);
        string[] requestLines = rawRequest.Split("\n");
        string[] requestHead = requestLines[0].Trim().Split(' ');

        for (int i = 1; i < requestLines.Length; i++)
        {
            string line = requestLines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] data = line.Split(":", 2);
            headers[data[0].Trim()] = data[1].Trim();
            
        }

        return new Request(requestHead[0], requestHead[1], requestHead[2], headers);
    }

    // Turns the response object into bytes
    public static byte[] Encoder(Response response)
    {
        var builder = new StringBuilder();
        builder.AppendFormat("{0} {1} {2}\r\n", response.Version, response.Code, response.Status);

        foreach (KeyValuePair<string, string> header in response.Headers)
        {
            builder.AppendFormat("{0}: {1}\r\n", header.Key, header.Value);
        }

        builder.Append("\r\n");
        if (!string.IsNullOrEmpty(response.Body))
        {
            builder.Append(response.Body);
        }

        return Encoding.ASCII.GetBytes(builder.ToString());
    }

    // Goes through a specified directory and creates endpoints from the JSON files inside.
    // Returns true if the endpoints are created without incident, false otherwise.
    public static bool CreateEndpoints(string endpointDir, List<Endpoint> endpoints)
    {
        FileInfo[] endpointFiles;
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(endpointDir);
            endpointFiles = directoryInfo.GetFiles();

            foreach (FileInfo file in endpointFiles)
            {
                string endpointJson = file.OpenText().ReadToEnd();
                endpoints.Add(JsonSerializer.Deserialize<Endpoint>(endpointJson));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        return true;
    }
}