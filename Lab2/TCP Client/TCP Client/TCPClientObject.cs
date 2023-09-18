using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    internal class TCPClientObject : ClientObject
    {
        public TCPClientObject(Server server, Socket client) : base(server, client)
        {
        }

        public override void Send(byte[] data)
        {
            _client.Send(data);
        }

        public override async void ProcessAsync()
        {
            try
            {
                Console.Write("Enter your name: ");
                string? name = Console.ReadLine();
                string? message = $"{name} joined the chat";
                _server.BroadcastMessage(message, Id);
                Console.WriteLine(message);
                var buffer = new byte[256];
                while (true)
                {
                    try
                    {                  
                        var bytes = await _client.ReceiveAsync(buffer, SocketFlags.None);
                        message = Encoding.UTF8.GetString(buffer, 0, bytes);
                        if (message == null)
                            continue;
                        message = $"{name}: {message}";
                        Console.WriteLine(message);
                        _server.BroadcastMessage(message, Id);
                    }
                    catch
                    {
                        message = $"{name} left the chat";
                        Console.WriteLine(message);
                        _server.BroadcastMessage(message, Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _server.RemoveConnection(Id);
            }
        }
    }
}