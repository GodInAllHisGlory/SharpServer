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
    public Dictionary<string, string> Headers => headers;

}