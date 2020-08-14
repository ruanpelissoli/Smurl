using MediatR;

namespace Smurl.Web.Commands
{
    public class NewShortUrlCommand : IRequest
    {
        public string Url { get; set; }
    }
}
