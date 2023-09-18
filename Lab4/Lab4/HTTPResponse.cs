using System.Text;

namespace Lab4;

internal class HTTPResponse : HTTPMessage
{
    public HTTPResponse()
    {
        _headers = new List<HTTPHeader>();
    }

    public HTTPResponse(byte[] responseBytes)
    {
        var responseString = Encoding.UTF8.GetString(responseBytes).TrimEnd('\0');
        Parse(responseString);
    }

    public string Version { get; set; } = "HTTP/1.0";
    public int StatusCode { get; set; } = 200;
    public string StatusMessage { get; set; } = "OK";

    protected override void ParseFirstHeader(string firstHeader)
    {
        var headerParts = firstHeader.Split(" ");
        Version = headerParts[0];
        StatusCode = int.Parse(headerParts[1]);
        StatusMessage = headerParts[2];
    }

    protected override string GetFirstHeader()
    {
        return $"{Version} {StatusCode} {StatusMessage}";
    }
}