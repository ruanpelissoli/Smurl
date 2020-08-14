using Microsoft.Azure.Cosmos.Table;

namespace Smurl.Web.Models
{
    public class UrlModel : TableEntity
    {
        public string Code { get; set; }
        public string RedirectUrl { get; set; }

        public UrlModel() { }
        public UrlModel(string code, string redirectUrl)
        {
            Code = code;
            RedirectUrl = redirectUrl;

            PartitionKey = code;
            RowKey = code;
        }
    }
}
