using System;
using System.IO;
using System.Text;
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
        [Test]
        public void LoadInstagramModiinJson_2LJ_ExpectedContent()
        {
            string jsonContent = File.ReadAllText(@"..\..\Data\instagramMedia.json");

            var actualLiveJournalPostAsHtml = new StringBuilder();
            actualLiveJournalPostAsHtml.AppendLine();

            int counter = 0;
            dynamic dyn = JsonConvert.DeserializeObject(jsonContent);
            foreach (var data in dyn.data)
            {
                string filter = data.filter;
                string link = data.link;
                string caption = data.caption.text;
                string author = data.caption.from.username;
                string fileUrl = data.images.standard_resolution.url;

                string location_latitude = null;
                string location_longitude = null;
                try
                {
                    location_latitude = data.location.latitude;
                    location_longitude = data.location.longitude;
                }
                catch
                {}

                counter++;

                actualLiveJournalPostAsHtml.AppendFormat("{0} by {1}", caption, author);
                if (!String.IsNullOrEmpty(location_latitude))
                {
                    actualLiveJournalPostAsHtml
                        .AppendLine(String.Format(@" <a href=""https://www.google.com/maps?q={0},{1}&amp;num=1&amp;t=m&amp;z=18"" target=""_blank"">on Google Map</a>", location_latitude, location_longitude));
                }
                actualLiveJournalPostAsHtml.AppendLine(String
                        .Format(@"<img alt="""" border=""0"" src=""{0}"" title=""{1}"" />", fileUrl, caption)
                        .Replace(@"\/", @"/"));
            }

            //? how to convert \u0421 to html string

            File.WriteAllText(@"d:\test.txt", actualLiveJournalPostAsHtml.ToString());
        }
        // <img alt="" border="0" src="http://codeprojecttv.s3.amazonaws.com/thumbnails_production/1/3583/thumb/Jquery-100.jpg" title="" />
        // <img alt="" border="0" src="http://distilleryimage11.s3.amazonaws.com/1cb4f7e030af11e3a77d22000a9f4ddd_8.jpg" title="" />
        // https://www.google.com/maps/preview#!q=%2B38%C2%B0+34'+24.00%22%2C+-109%C2%B0+32'+57.00 by "+38° 34' 24.00", -109° 32' 57.00"
        // https://www.google.com/maps?q=31.897722,35.016774&num=1&t=m&z=18
        // <a href="https://www.google.com/maps?q=31.897722,35.016774&amp;num=1&amp;t=m&amp;z=18" target="_blank">on Google Map</a>
    }
}