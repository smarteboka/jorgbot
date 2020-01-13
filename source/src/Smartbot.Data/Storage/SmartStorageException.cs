using System;

namespace Smartbot.Data.Storage
{
    public class SmartStorageException : Exception
    {
        public SmartStorageException(string eMessage) : base(eMessage)
        {
        }
    }
}