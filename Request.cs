using System.Collections.Generic;

//Holds all the information of a http request 
class Request
{
    private readonly Dictionary<string, string> headers;

    public Request(string verb, string path, string version, IDictionary<string, string> headers)
    {
        Verb = verb;
        Path = path;
        Version = version;
        this.headers = new Dictionary<string, string>(headers, StringComparer.OrdinalIgnoreCase);
    }

    public string Verb { get; }
    public string Path { get; }
    public string Version { get; }
    public IReadOnlyDictionary<string, string> Headers => headers;


    //Returns a specific header value given a key value. 
    //Returns null if header field doesn't exist.
    public string? GetHeader(string key){ return this.headers.TryGetValue(key, out var value) ? value : null; }

}