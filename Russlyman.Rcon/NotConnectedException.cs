using System;

namespace Russlyman.Rcon
{
    public class NotConnectedException : Exception
    {
        public NotConnectedException(string message) : base(message) {}
    }
}