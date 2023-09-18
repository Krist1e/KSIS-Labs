using Chat;
using System.Net;

string? option = Console.ReadLine();
int port = 5050;

IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port);
if (option == "-s")
{
    Server server = new Server(endPoint);
    await server.Run();
}
else if (option == "-c")
{
    Client client = new Client(endPoint);
    client.Start();
}
else
{
    Console.WriteLine("Invalid option");
}