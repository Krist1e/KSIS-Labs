using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    internal class NTPPacket
    {        
        public byte LeapIndicator { get; set; } = 0;
        public byte VersionNumber { get; set; } = 0;
        public byte Mode { get; set; } = 0;
        public byte Stratum { get; set; } = 0;
        public byte Poll { get; set; } = 0;
        public byte Precision { get; set; } = 0;
        public uint RootDelay { get; set; } = 0;
        public uint RootDispersion { get; set; } = 0;
        public uint ReferenceID { get; set; } = 0;
        public ulong ReferenceTimestamp { get; set; } = 0;
        public ulong OriginTimestamp { get; set; } = 0;
        public ulong ReceiveTimestamp { get; set; } = 0;
        public ulong TransmitTimestamp { get; set; } = 0;

        public static byte[] Serialize(NTPPacket packet)
        {
            var buffer = new byte[48];
            buffer[0] = (byte)((packet.LeapIndicator << 6) | (packet.VersionNumber << 3) | packet.Mode);
            buffer[1] = packet.Stratum;
            buffer[2] = packet.Poll;
            buffer[3] = packet.Precision;
            BitConverter.GetBytes(packet.RootDelay).CopyTo(buffer, 4);
            BitConverter.GetBytes(packet.RootDispersion).CopyTo(buffer, 8);
            BitConverter.GetBytes(packet.ReferenceID).CopyTo(buffer, 12);
            BitConverter.GetBytes(packet.ReferenceTimestamp).CopyTo(buffer, 16);
            BitConverter.GetBytes(packet.OriginTimestamp).CopyTo(buffer, 24);
            BitConverter.GetBytes(packet.ReceiveTimestamp).CopyTo(buffer, 32);
            BitConverter.GetBytes(packet.TransmitTimestamp).CopyTo(buffer, 40);
            return buffer;
        }

        public static NTPPacket Deserialize(byte[] buffer)
        {
            var packet = new NTPPacket();
            packet.LeapIndicator = (byte)((buffer[0] >> 6) & 0x03);
            packet.VersionNumber = (byte)((buffer[0] >> 3) & 0x07);
            packet.Mode = (byte)(buffer[0] & 0x07);
            packet.Stratum = buffer[1];
            packet.Poll = buffer[2];
            packet.Precision = buffer[3];
            packet.RootDelay = BitConverter.ToUInt32(buffer, 4);
            packet.RootDispersion = BitConverter.ToUInt32(buffer, 8);
            packet.ReferenceID = BitConverter.ToUInt32(buffer, 12);
            packet.ReferenceTimestamp = BitConverter.ToUInt64(buffer, 16);
            packet.OriginTimestamp = BitConverter.ToUInt64(buffer, 24);
            packet.ReferenceTimestamp = BitConverter.ToUInt64(buffer, 32);
            packet.TransmitTimestamp = BitConverter.ToUInt64(buffer, 40);
            return packet;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("Leap Indicator: " + LeapIndicator);
            result.AppendLine("Version Number: " + VersionNumber);
            result.AppendLine("Mode: " + Mode);
            result.AppendLine("Stratum: " + Stratum);
            result.AppendLine("Poll: " + Poll);
            result.AppendLine("Precision: " + Precision);
            result.AppendLine("Root Delay: " + RootDelay);
            result.AppendLine("Root Dispersion: " + RootDispersion);
            result.AppendLine("Reference ID: " + ReferenceID);
            result.AppendLine("Reference Timestamp: " + ReferenceTimestamp);
            result.AppendLine("Origin Timestamp: " + OriginTimestamp);
            result.AppendLine("Receive Timestamp: " + ReceiveTimestamp);
            result.AppendLine("Transmit Timestamp: " + TransmitTimestamp);
            return result.ToString();
        }

    }
}
