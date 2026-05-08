class Endpoint
{
    public Endpoint(string path, string resourcePath)
    {
        Path = path;
        ResourcePath = resourcePath;
    }

    public string Path { get; }
    public string ResourcePath { get; }
}