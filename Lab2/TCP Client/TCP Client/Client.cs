using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat
{
    internal class Client
    {
        private IPEndPoint _endPoint;
        private Socket _tcpClient;
        private string _name;

        public string Name { get => _name; set => _name = value; }

        public Client(IPEndPoint endPoint)
        {
            _endPoint = endPoint;
            _tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            _tcpClient.ConnectAsync(_endPoint);
            Console.WriteLine("Connected to server");
            

        }

        public Task SendAsync()
        {
            while (true)
            {                
                while (true)
                {
                    Console.Write("Enter your message: ");
                    string message = Console.ReadLine();
                    if (message == "/end") break;
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    _tcpClient.SendAsync(data);
                }
                

            }
        }

        public void Process() { }
    }

}
