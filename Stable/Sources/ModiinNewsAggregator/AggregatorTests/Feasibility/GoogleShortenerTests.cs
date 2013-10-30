using System;
using GoogleAPI.UrlShortener;
using NUnit.Framework;

namespace AggregatorTests.Feasibility
{
    [TestFixture]
    public class GoogleShortenerTests
    {
        [Test]
        [Explicit]
        public void Test()
        {
            UrlResource client = new UrlResource();
            // Shorten url according the parameter below.
            var response = client.Insert( new ShortenRequest { LongUrl = "http://gshortener.codeplex.com" });

            // Print short url. Ex: http://goo.gl/sOme
            Console.WriteLine(response.Id);

            // Print long url. Ex: http://gshortener.codeplex.com
            Console.WriteLine(response.LongUrl);
        }
    }
}