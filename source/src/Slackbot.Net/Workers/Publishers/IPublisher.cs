using System.Threading.Tasks;

namespace Slackbot.Net.Workers.Publishers
{
    public interface IPublisher
    {
        Task Publish(Notification notification);
    }
}