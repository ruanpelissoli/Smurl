using MediatR;

namespace Smurl.Web.Queries
{
    public class GetUrlQuery : IRequest<string>
    {
        public string Code { get; set; }
    }
}
