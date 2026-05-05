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
        int bytesRecived = clientSocket.Receive(buffer);

        //Fufill request
        Request httpRequest = Decoder(buffer, bytesRecived);
        Console.WriteLine("Verb: " + httpRequest.Verb + " Path: " + httpRequest.Path + " Version: " + httpRequest.Version);
        Console.WriteLine("Accept-Encoding: " + httpRequest.GetHeader("Accept-Encoding"));
        Console.WriteLine("Client connected: " + clientSocket.RemoteEndPoint);
        
        // Close sockets
        clientSocket.Close();
        socket.Close();
    }

    //Turns the request from the client into a request object 
    public static Request Decoder(byte[] buffer, int bytesRecived){
        char[] responseChars = new char[256];

        Encoding.ASCII.GetChars(buffer, 0, bytesRecived, responseChars, 0);

        List<string> request = new string(responseChars).Split("\n").ToList<string>();
        string[] requestHead = request[0].Trim().Split(" ");
        request.RemoveAt(0);

        return new Request(requestHead[0], requestHead[1], requestHead[2], request);
    }
}