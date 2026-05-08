class Response
{
    private Dictionary<string, string> headers;

    public Response(string version, int code, string status, string body, Dictionary<string, string> headers)
    {
        Version = version;
        Code = code;
        Status = status;
        Body = body;
        this.headers = headers;
    }

    public string Version { get; }
    public int Code { get; }
    public string Status { get; }
    public string Body { get; }
    public IReadOnlyDictionary<string, string> Headers => headers;

    public string ConstructResponse()
    {
        string responseHead = Version + Code + Status;
        string headers = "";

        foreach(KeyValuePair<string, string> header in Headers)
        {
            headers += "\n" + header.Key + ":" + header.Value;
        }

        return responseHead + 
                headers +
                "\n \n" +
                Body;

    }

}