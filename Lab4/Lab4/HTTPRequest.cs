using System.Text;

namespace Lab4;

internal class HTTPRequest : HTTPMessage
{
    public enum HTTPRequestMethod
    {
        GET,
        HEAD,
        POST
    }

    public HTTPRequest(byte[] requestBytes)
    {
        var requestString = Encoding.UTF8.GetString(requestBytes).TrimEnd('\0');
        Parse(requestString);
    }

    public HTTPRequestMethod Method { get; set; } = HTTPRequestMethod.GET;
    public string Uri { get; set; } = "/";
    public string Version { get; set; } = "HTTP/1.0";

    protected override void ParseFirstHeader(string firstHeader)
    {
        var headerParts = firstHeader.Split(" ");
        Method = Enum.Parse<HTTPRequestMethod>(headerParts[0]);
        Uri = headerParts[1];
        Version = headerParts[2];
    }

    protected override string GetFirstHeader()
    {
        return $"{Method} {Uri} {Version}";
    }
}