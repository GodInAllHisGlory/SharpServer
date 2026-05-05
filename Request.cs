using System.Collections.Generic;

//Holds all the information of a http request 
class Request
{
    private string verb;
    private string path;
    private string version;
    private Dictionary<string, string> headers = new Dictionary<string, string>();

    public Request(string verb, string path, string version, List<string> headers)
    {
        this.Verb = verb;
        this.Path = path;
        this.Version = version;

        foreach (string header in headers)
        {
            string[] data = header.Split(":",2);
            this.headers.Add(data[0].Trim(), data[1].Trim());
        }
    }

    public string Verb { get => verb; set => verb = value; }
    public string Path { get => path; set => path = value; }
    public string Version { get => version; set => version = value; }

    //Returns http resqest verb

    //Returns a specific header vaule
    public string? GetHeader(string key)
    {
        if (headers.TryGetValue(key, out string? header))
        {
            return header;
        }
        return null;
    }

}