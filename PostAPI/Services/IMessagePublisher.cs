using PostAPI.Models;

namespace PostAPI.Services
{
    public interface IMessagePublisher
    {
        void Publish(NewImageMessage message);
    }
}
