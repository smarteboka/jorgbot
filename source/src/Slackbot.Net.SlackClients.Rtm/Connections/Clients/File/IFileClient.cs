using System.IO;
using System.Threading.Tasks;

namespace Slackbot.Net.SlackClients.Rtm.Connections.Clients.File
{
    internal interface IFileClient
    {
        Task PostFile(string slackKey, string channel, string filePath);
        Task PostFile(string slackKey, string channel, Stream stream, string fileName);
    }
}
