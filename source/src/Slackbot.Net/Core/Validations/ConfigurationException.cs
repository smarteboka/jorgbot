using System;

namespace Slackbot.Net.Core.Validations
{
    internal class ConfigurationException : Exception
    {
        public ConfigurationException(string s) : base(s)
        {
        }
    }
}