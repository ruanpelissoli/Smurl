using MediatR;
using Microsoft.AspNetCore.SignalR;
using Smurl.Web.Events;
using Smurl.Web.Hubs;
using System.Threading;
using System.Threading.Tasks;

namespace Smurl.Web.Handlers
{
    public class NewUrlCreatedEventHandler : INotificationHandler<NewUrlCreatedEvent>
    {
        private readonly IHubContext<UrlCreatedHub> _hubContext;

        public NewUrlCreatedEventHandler(IHubContext<UrlCreatedHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task Handle(NewUrlCreatedEvent notification, CancellationToken cancellationToken)
        {
            return _hubContext.Clients.All.SendAsync("newUrlCreated", notification, cancellationToken);
        }
    }
}
