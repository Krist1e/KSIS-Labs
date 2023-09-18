using System.Net.Sockets;

namespace Lab4;

internal class HTTPConnection
{
    private readonly Memory<byte> _buffer = new byte[4096];
    private readonly NetworkStream _stream;
    private readonly object _lockConsole;

    public HTTPConnection(NetworkStream stream, object lockObject)
    {
        _stream = stream;
        _lockConsole = lockObject;
    }

    public async Task<HTTPRequest> ReceiveAsync()
    {
        var memoryStream = new MemoryStream();
        int bytesRead;
        do
        {
            bytesRead = await _stream.ReadAsync(_buffer);
            await memoryStream.WriteAsync(_buffer[..bytesRead]);
        } while (bytesRead == _buffer.Length);
        if (memoryStream.Length == 0)
            throw new Exception("Empty request");

        var request = new HTTPRequest(memoryStream.ToArray());
        lock (_lockConsole)
        {
            Console.WriteLine("Received:");
            Console.WriteLine(request);
        }
        return request;
    }

    public async Task SendAsync(HTTPResponse response)
    {
        await _stream.WriteAsync(response.ToBytes());
        await _stream.FlushAsync();
        lock (_lockConsole)
        {
            Console.WriteLine("Sent:");
            Console.WriteLine(response);
        }
    }
}