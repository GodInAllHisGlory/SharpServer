using System.ComponentModel.Design;

class Request
{
    private string verb, path, version;
    private Dictionary<string, string> headers = new Dictionary<string, string>();

    public Request(string verb, string path, string version, List<string> headers)
    {
        this.verb = verb;
        this.path = path;
        this.version = version;

        foreach(string header in headers)
        {
            string[] data = header.Split(":");
            this.headers.Add(data[0], data[1]);
        }
    }

    public string getVerb()
    {
        return this.verb;
    }

    public string getPath()
    {
        return this.path;
    }

    public string getVersion()
    {
        return this.version;
    }

}