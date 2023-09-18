using System.Net;

namespace Lab4;

internal class Program
{
    private static async Task Main(string[] args)
    {
        HTTPServer server = new(new IPEndPoint(IPAddress.Loopback, 80),
            "D:\\2 курс\\КСиС\\Lab4\\html\\");
        await server.Start();
    }
}