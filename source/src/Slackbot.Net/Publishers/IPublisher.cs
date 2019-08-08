using System.Threading.Tasks;

namespace Slackbot.Net.Publishers
{
    public interface IPublisher
    {
        Task Publish(Notification notification);
    }
}