using System;

namespace Smartbot.Utilities.Storage
{
    public class SmartStorageException : Exception
    {
        public SmartStorageException(string eMessage) : base(eMessage)
        {
        }
    }
}