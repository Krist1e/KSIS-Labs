using System.Net;

namespace Lab3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NTPClient client = new NTPClient("time1.google.com", 123);
            client.Start();
        }
    }
}