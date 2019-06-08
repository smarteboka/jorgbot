using System.Threading.Tasks;

namespace JorgBot.Publishers
{
    public interface IPublisher
    {
        Task Publish(Notification notification);
    }
}