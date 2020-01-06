using System;
using System.Runtime.Serialization;

namespace SlackConnector.Exceptions
{
    public class HandshakeException : Exception
    {
        public HandshakeException(string message) : base(message)
        { }
    }
}