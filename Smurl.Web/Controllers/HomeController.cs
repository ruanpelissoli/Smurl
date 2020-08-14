using Microsoft.AspNetCore.Mvc;
using MediatR;
using Smurl.Web.Commands;
using System.Threading.Tasks;
using Smurl.Web.Queries;

namespace Smurl.Web.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client, NoStore = true)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("{code}")]
        public async Task<IActionResult> GoTo(string code)
        {
            var url = await _mediator.Send(new GetUrlQuery { Code = code });

            return Redirect(url);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string url)
        {
            if (string.IsNullOrEmpty(url)) return View();

            await _mediator.Send(new NewShortUrlCommand { Url = url });

            return Ok();
        }
    }
}
