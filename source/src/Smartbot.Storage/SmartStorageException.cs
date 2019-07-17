using System;

namespace Smartbot.Storage
{
    public class SmartStorageException : Exception
    {
        public SmartStorageException(string eMessage) : base(eMessage)
        {
        }
    }
}