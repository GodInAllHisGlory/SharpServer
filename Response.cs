class Response
{
    private string version;
    private int code;
    private string status;
    private Dictionary<string, string> headers;

    public Response(string version, int code, string status, Dictionary<string, string> headers)
    {
        this.Version = version;
        this.Code = code;
        this.Status = status;
        this.headers = headers;
    }

    public string Version { get => version; set => version = value; }
    public int Code { get => code; set => code = value; }
    public string Status { get => status; set => status = value; }

    public string? GetHeader(string key)
    {
        if (headers.TryGetValue(key, out string? header))
        {
            return header;
        }
        return null;
    }
}