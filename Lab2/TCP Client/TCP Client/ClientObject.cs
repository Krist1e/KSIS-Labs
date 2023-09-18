using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chat
{
    internal abstract class ClientObject
    { 
        protected readonly Server _server;
        protected readonly Socket _client;
        public int Id { get; private set; }                
        protected ClientObject(Server server, Socket client)
        {
            _server = server;
            _client = client;
            Id = _client.GetHashCode();
            _server.AddConnection(this);
        }        
        public abstract void ProcessAsync();
        protected internal void Disconnect()
        {
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
            _server.RemoveConnection(Id);
        }
        public abstract void Send(byte[] data);
    }
}