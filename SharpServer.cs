using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Sharpserver
{
    static void Main(string[] args)
    {
        const int PORT = 8000;
        Uri uri = new Uri("http://127.0.0.1");
        Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        
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

    //Turns the request from the client into a request object 
    public static Request Decoder(byte[] buffer, int bytesReceived){
        char[] responseChars = new char[256];

        Encoding.ASCII.GetChars(buffer, 0, bytesReceived, responseChars, 0);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        string[] request = new string(responseChars).Split("\n");
        string[] requestHead = request[0].Trim().Split(" ");
        
        //Splits the header value pair and puts the into a dictionary for future use
        for (int i=1; i<request.Length; i++)
        {
            string[] data = request[i].Split(":",2);
            headers.Add(data[0].Trim(), data[1].Trim());
        }

        return new Request(requestHead[0], requestHead[1], requestHead[2], headers);
    }
}