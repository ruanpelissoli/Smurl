using MediatR;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Smurl.Web.Commands;
using Smurl.Web.Events;
using Smurl.Web.Models;
using Smurl.Web.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Smurl.Web.Handlers
{
    public class NewShortUrlRequestHandler 
        : IRequestHandler<NewShortUrlCommand>,
        IRequestHandler<GetUrlQuery, string>
    {
        private readonly CloudTable _table;
        private readonly IMediator _mediator;
        private readonly string _baseUrl;

        public NewShortUrlRequestHandler(IConfiguration configuration, IMediator mediator)
        {
            var account = CloudStorageAccount.Parse(configuration.GetConnectionString("AzureTables"));

            var client = account.CreateCloudTableClient(new TableClientConfiguration());
            _table = client.GetTableReference("Urls");
            _table.CreateIfNotExists();

            _baseUrl = configuration.GetValue<string>("RedirectBaseUrl");

            _mediator = mediator;
        }

        public async Task<Unit> Handle(NewShortUrlCommand request, CancellationToken cancellationToken)
        {
            string code;
            var url = request.Url;

            do
            {
                code = GenerateCode();
            } while ((await GetUrlFromCode(code)) != null);

            var urlModel = new UrlModel(code, url);

            TableOperation insertOperation = TableOperation.Insert(urlModel);
            _table.Execute(insertOperation);

            await _mediator.Publish(new NewUrlCreatedEvent
            {
                Url = $"{_baseUrl}{code}"
            }, cancellationToken);

            return Unit.Value;
        }

        public async Task<string> Handle(GetUrlQuery request, CancellationToken cancellationToken)
        {
            var url = await GetUrlFromCode(request.Code);

            if (string.IsNullOrEmpty(url)) return null;

            if (!url.Contains("http"))
                url = $"http://{url}";

            return url;
        }

        private string GenerateCode()
        {
            var stringLength = 6;
            var rd = new Random();
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
            char[] chars = new char[stringLength];

            for (int i = 0; i < stringLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        private async Task<string> GetUrlFromCode(string code)
        {
            var condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, code);
            var query = new TableQuery<UrlModel>().Where(condition);
            var result = await _table.ExecuteQuerySegmentedAsync(query, new TableContinuationToken());

            return result.FirstOrDefault()?.RedirectUrl;
        }

        
    }
}
