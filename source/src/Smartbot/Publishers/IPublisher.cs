using System.Threading.Tasks;

namespace Smartbot.Publishers
{
    public interface IPublisher
    {
        Task Publish(Notification notification);
    }
}