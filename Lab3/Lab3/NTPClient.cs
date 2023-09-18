using System;
using System.Net;
using System.Net.Sockets;

namespace Lab3
{

    public class NTPClient
    {
        private Socket _client;
        private EndPoint _endPoint;

        public NTPClient(string endPoint, int port)
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress ipAddress;
            if (!IPAddress.TryParse(endPoint, out ipAddress))
                ipAddress = Dns.GetHostEntry(endPoint).AddressList[0];
            _endPoint = new IPEndPoint(ipAddress, port);
        }

        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        public static DateTime ConvertFromNtpTime(byte[] buffer)
        {
            const byte serverReplyTime = 40;
            ulong intPart = BitConverter.ToUInt32(buffer, serverReplyTime);
            ulong fractPart = BitConverter.ToUInt32(buffer, serverReplyTime + 4);
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);
            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x1_00_00_00_00L);
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
            return networkDateTime.ToLocalTime();
        }

        public void Start()
        {
            try
            {
                Console.WriteLine($"Connected to the server at {_endPoint}");
                NTPPacket packet = new NTPPacket();
                packet.VersionNumber = 3;
                packet.Mode = 3;
                var buffer = NTPPacket.Serialize(packet);
                _client.ReceiveTimeout = 5000;
                _client.SendTo(buffer, _endPoint);
                _client.ReceiveFrom(buffer, ref _endPoint);               
                packet = NTPPacket.Deserialize(buffer);
                Console.WriteLine(packet);
                Console.WriteLine(ConvertFromNtpTime(buffer));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
