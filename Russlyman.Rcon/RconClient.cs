using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Russlyman.Rcon
{
    public class RconClient
    {
        public IPAddress Ip { get; private set; }
        public int Port { get; private set; }

        private IPEndPoint _ipEndPoint;
        private UdpClient _udpClient;

        private byte[] _header;
        private string _password;
        private readonly string[] _uglyList = { "????print", "^1", "^2", "^3", "^4", "^5", "^6", "^7", "^8", "^9" };

        // http://jonjohnston.co.uk/articles/3/how-to-access-quake-iii-arena-dedicated-server-from-linux-command-line
        private void PrepareHeader()
        {
            var header = new List<byte> { 0xFF, 0xFF, 0xFF, 0xFF };
            header.AddRange( Encoding.ASCII.GetBytes($"rcon {_password} "));

            _header = header.ToArray();
        }

        private byte[] PrepareOob(string command)
        {
            var oob = new List<byte>();
            oob.AddRange(_header);
            oob.AddRange(Encoding.ASCII.GetBytes(command));

            return oob.ToArray();
        }

        private string CleanReply(byte[] reply)
        {
            var replyString = Encoding.ASCII.GetString(reply);

            return _uglyList.Aggregate(replyString, (current, item) => current.Replace(item, ""))
                .Replace("\n", Environment.NewLine)
                .TrimStart()
                .TrimEnd();
        }

        public string Send(string command)
        {
            var oob = PrepareOob(command);
            _udpClient.Send(oob, oob.Length);

            var replyBytes = _udpClient.Receive(ref _ipEndPoint);
            return CleanReply(replyBytes);
        }

        public async Task<string> SendAsync(string command)
        {
            var oob = PrepareOob(command);
            await _udpClient.SendAsync(oob, oob.Length);

            var replyBytes = (await _udpClient.ReceiveAsync()).Buffer;
            return CleanReply(replyBytes);
        }

        public void Connect(string ip, int port, string password)
        {
            if (password.Contains(' ') || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Invalid RCon password provided", nameof(password));

            var newIpAddress = IPAddress.Parse(ip);

            var newUdpClient = new UdpClient();
            newUdpClient.Connect(newIpAddress, port);

            _udpClient = newUdpClient;
            Ip = newIpAddress;
            Port = port;
            _password = password;

            _ipEndPoint = new IPEndPoint(Ip, Port);

            PrepareHeader();
        }
    }
}