using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AggregatorTests
{
    [TestFixture]
    public class InstagramJsonParserTests
    {
        [Test]
        public void LoadInstagramModiinJson_MediaCount_13()
        {
            string jsonContent = File.ReadAllText(@"..\..\Data\instagramMedia.json");

            int counter = 0;
            dynamic dyn = JsonConvert.DeserializeObject(jsonContent);
            foreach (var data in dyn.data)
            {
                string filter = data.filter;
                string link = data.link;
                string fileUrl = data.images.standard_resolution.url;
                counter++;
            }
            Assert.AreEqual(14, counter);
        }
    }
}