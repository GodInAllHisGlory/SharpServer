class Response
{
    private Dictionary<string, string> headers;

    public Response(string version, int code, string status, Dictionary<string, string> headers)
    {
        Version = version;
        Code = code;
        Status = status;
        this.headers = headers;
    }

    public string Version { get; }
    public int Code { get; }
    public string Status { get; }
    public IReadOnlyDictionary<string, string> Headers => headers;

}