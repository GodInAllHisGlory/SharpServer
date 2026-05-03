using System.Collections.Generic;

//Holds all the information of a http request 
class Request
{
    private string verb, path, version;
    private Dictionary<string, string> headers = new Dictionary<string, string>();
    
    public Request(string verb, string path, string version, List<string> headers)
    {
        this.verb = verb;
        this.path = path;
        this.version = version;

        foreach (string header in headers)
        {
            string[] data = header.Split(":",2);
            this.headers.Add(data[0].Trim(), data[1].Trim());
        }
    }

    //Returns http resqest verb
    public string GetVerb()
    {
        return verb;
    }

    //Returns the requested path
    public string GetPath()
    {
        return path;
    }

    //Returns the version of the http protorcol used
    public string GetVersion()
    {
        return version;
    }

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