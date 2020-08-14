using MediatR;

namespace Smurl.Web.Events
{
    public class NewUrlCreatedEvent : INotification
    {
        public string Url { get; set; }
    }
}
