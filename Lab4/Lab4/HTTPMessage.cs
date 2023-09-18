using System.Text;

namespace Lab4;

public readonly struct HTTPHeader
{
    public string Name { get; }
    public string Value { get; }

    public HTTPHeader(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public HTTPHeader(string header) : this(header.Split(": ")[0], header.Split(": ")[1])
    {
    }

    public IEnumerable<byte> ToBytes()
    {
        return Encoding.UTF8.GetBytes(ToString());
    }

    public override string ToString()
    {
        return $"{Name}: {Value}";
    }
}

internal abstract class HTTPMessage
{
    protected byte[] _body = Array.Empty<byte>();
    protected List<HTTPHeader> _headers = new();

    public IEnumerable<HTTPHeader> Headers => _headers;

    public IEnumerable<byte> Body
    {
        get => _body;
        set => _body = value.ToArray();
    }

    public void AddHeader(HTTPHeader header)
    {
        _headers.Add(header);
    }

    public void AddHeader(string name, string value)
    {
        _headers.Add(new HTTPHeader(name, value));
    }

    protected void Parse(string content)
    {
        var contentParts = content.Split("\r\n\r\n");
        if (contentParts.Length != 2)
            throw new ArgumentException($"Invalid HTTP message: {content}");
        var headersPart = contentParts[0];
        var bodyPart = contentParts[1];
        _body = Encoding.UTF8.GetBytes(bodyPart);
        _headers = new List<HTTPHeader>();
        var headersLines = headersPart.Split("\r\n");
        if (headersLines.Length == 0)
            throw new ArgumentException($"Invalid HTTP message: {headersPart}");
        ParseFirstHeader(headersLines[0]);
        for (var i = 1; i < headersLines.Length; i++) 
            _headers.Add(new HTTPHeader(headersLines[i]));
    }

    protected abstract void ParseFirstHeader(string firstHeader);

    protected abstract string GetFirstHeader();

    public byte[] ToBytes()
    {
        var result = new List<byte>();
        result.AddRange(Encoding.UTF8.GetBytes(GetFirstHeader()));
        result.AddRange("\r\n"u8.ToArray());
        foreach (var header in _headers)
        {
            result.AddRange(header.ToBytes());
            result.AddRange("\r\n"u8.ToArray());
        }

        result.AddRange("\r\n"u8.ToArray());
        result.AddRange(_body);
        return result.ToArray();
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        result.AppendLine(GetFirstHeader());
        foreach (var header in _headers) result.AppendLine(header.ToString());
        result.AppendLine();
        result.Append(Encoding.UTF8.GetString(_body.Take(10000).ToArray()));
        return result.ToString();
    }
}