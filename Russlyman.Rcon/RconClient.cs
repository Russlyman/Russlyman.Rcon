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
        private string _password;
        private int _receiveTimeoutMs;

        private IPEndPoint _ipEndPoint;
        private UdpClient _udpClient;

        private byte[] _header;
        private readonly string[] _uglyList = { "????print", "^1", "^2", "^3", "^4", "^5", "^6", "^7", "^8", "^9" };

        public RconClient(int receiveTimeoutMs = 3000)
        {
            _receiveTimeoutMs = receiveTimeoutMs;
        }

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
            if (_udpClient == null)
            {
                throw new NotConnectedException("You need to connect before you can send commands.");
            }

            var oob = PrepareOob(command);
            _udpClient.Send(oob, oob.Length);

            var replyBytes = _udpClient.Receive(ref _ipEndPoint);

            return CleanReply(replyBytes);
        }

        public async Task<string> SendAsync(string command)
        {
            if (_udpClient == null)
            {
                throw new NotConnectedException("You need to connect before you can send commands.");
            }

            var oob = PrepareOob(command);
            await _udpClient.SendAsync(oob, oob.Length);

            var replyTask = _udpClient.ReceiveAsync();
            var tasks = await Task.WhenAny(replyTask, Task.Delay(_receiveTimeoutMs));

            if (tasks is Task<UdpReceiveResult>)
            {
                return CleanReply(replyTask.Result.Buffer);
            }
            
            throw new SocketException(10060);
        }

        public void Connect(string ip, int port, string password)
        {
            if (password.Contains(' ') || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Invalid RCon password provided", nameof(password));

            var newIpAddress = IPAddress.Parse(ip);

            Close();

            var newUdpClient = new UdpClient();
            newUdpClient.Client.ReceiveTimeout = _receiveTimeoutMs;
            newUdpClient.Connect(newIpAddress, port);

            _udpClient = newUdpClient;
            Ip = newIpAddress;
            Port = port;
            _password = password;

            _ipEndPoint = new IPEndPoint(Ip, Port);

            PrepareHeader();
        }

        public void Close()
        {
            if (_udpClient != null)
                _udpClient.Close();
        }
    }
}