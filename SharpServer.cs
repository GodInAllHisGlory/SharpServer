using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class Sharpserver
{
    static void Main(string[] args)
    {
        const int PORT = 8000;
        Uri uri = new Uri("http://127.0.0.1");
        Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        List<Endpoint> endpoints = new List<Endpoint>();

        //Creates the endpoints found in endpoints.json when the server is first started.
        //If the endpoint's cannot be created then the server does not start.
        if(!CreateEndpoints("endpoints", endpoints)) return;
        Console.WriteLine(endpoints[0].Path);
                
        // Bind the socket to the local endpoint
        socket.Bind(new IPEndPoint(IPAddress.Parse(uri.Host), PORT));
        
        // Listen for incoming connections
        socket.Listen(10);
        Console.WriteLine("Server listening on " + uri.Host + ":" + PORT);
        
        // Accept a connection
        byte[] buffer = new byte[256];
        Socket clientSocket = socket.Accept();
        int bytesReceived = clientSocket.Receive(buffer);

        //Fulfill request
        Request httpRequest = Decoder(buffer, bytesReceived);
        Console.WriteLine("Verb: " + httpRequest.Verb + " Path: " + httpRequest.Path + " Version: " + httpRequest.Version);
        Console.WriteLine("Accept-Encoding: " + httpRequest.GetHeader("Accept-Encoding"));
        Console.WriteLine("Client connected: " + clientSocket.RemoteEndPoint);
        
        // Close sockets
        clientSocket.Close();
        socket.Close();
    }

    public static Endpoint router(List<Endpoint> endpoints, Request request)
    {
        foreach(Endpoint endpoint in endpoints)
        {
            if(request.Path == endpoint.Path) return endpoint;
        }

        //If path isn't found return 404 page
        return new Endpoint(null,"template/404.html"); 
    }

    //Turns the request from the client into a request object 
    public static Request Decoder(byte[] buffer, int bytesReceived){
        char[] responseChars = new char[256];

        Encoding.ASCII.GetChars(buffer, 0, bytesReceived, responseChars, 0);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        string[] request = new string(responseChars).Split("\n");
        string[] requestHead = request[0].Trim().Split(" ");
        
        //Splits the header value pair and puts the into a dictionary for future reference
        for (int i=1; i<request.Length; i++)
        {
            string[] data = request[i].Split(":",2);
            headers.Add(data[0].Trim(), data[1].Trim());
        }

        return new Request(requestHead[0], requestHead[1], requestHead[2], headers);
    }

    public byte[] Encoder(Response response)
    {
        string responseHead = response.Version + response.Code + response.Status;
        string headers = "";

        foreach(KeyValuePair<string, string> header in response.Headers)
        {
            headers += $"\n + {header.Key}: {header.Value}";
        }

        string fullResponse = $"{responseHead} {headers} \n \n {response.Body}";

        return Encoding.ASCII.GetBytes(fullResponse);

    }

    //Goes through a specified directory and creates endpoints from the JSON files inside.
    //Returns true is the endpoints are created without incident, false otherwise.
    public static bool CreateEndpoints(string endpointDir,  List<Endpoint> endpoints)
    {
        FileInfo[] endpointFiles;
        // endpoints = new List<Endpoint>();
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(endpointDir);
            endpointFiles = directoryInfo.GetFiles();

            foreach(FileInfo file in endpointFiles)
            {
                string endpointJson = file.OpenText().ReadToEnd();
                endpoints.Add(JsonSerializer.Deserialize<Endpoint>(endpointJson));
            }
        } catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        return true;
    }
}