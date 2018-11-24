using System;

namespace RageServers.Exceptions
{
    public class WrongDatabasePathException : Exception
    {
        public WrongDatabasePathException()
        {
        }

        public WrongDatabasePathException(string message) : base($"Wrong database path: {message}")
        {
        }

        public WrongDatabasePathException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
