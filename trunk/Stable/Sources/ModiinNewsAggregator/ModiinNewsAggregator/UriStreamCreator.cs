using System.IO;
using System.Net;
using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator
{
    public class UriStreamCreator : IStreamCreator
    {
        private readonly string uri;
        public UriStreamCreator(string uri)
        {
            this.uri = uri;
        }
        public Stream CreateStream()
        {
            var client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            Stream dataStream = client.OpenRead(uri);
            return dataStream;
        }
    }
}
