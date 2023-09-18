using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Chat
{
    internal class Server
    {
        private readonly Socket _server;
        private readonly List<ClientObject> _clients;
        private readonly IPEndPoint _endPoint;
        
        public Server(IPEndPoint endPoint)
        {
            _endPoint = endPoint;
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clients = new List<ClientObject>();
        }
        
        public void AddConnection(ClientObject clientObject)
        {
            _clients.Add(clientObject);
        }

        public void RemoveConnection(int id)
        {
            var client = _clients.FirstOrDefault(obj => obj.Id == id);
            if (client != null)
                _clients.Remove(client);
        }

        public void Disconnect()
        {
            _server.Shutdown(SocketShutdown.Both);
            _server.Close();

            foreach (var client in _clients)
            {
                client.Disconnect();
            }

            Environment.Exit(0);
        }

        public void BroadcastMessage(string message, int id)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            foreach (var client in _clients)
            {
                if (client.Id != id)
                    client.Send(data);
            }
        }

        public async Task Run()
        {
            try
            {
                _server.Bind(_endPoint);
                _server.Listen();

                while (true)
                {
                    var client = await _server.AcceptAsync();
                    ClientObject clientObject = new TCPClientObject(this, client);
                    _clients.Add(clientObject);
                    await Task.Run(() => clientObject.ProcessAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
    }
}
