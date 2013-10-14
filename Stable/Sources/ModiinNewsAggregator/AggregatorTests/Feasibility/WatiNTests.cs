using System;
using HtmlAgilityPack;
using NUnit.Framework;
using WatiN.Core;

namespace AggregatorTests.Feasibility
{
    [TestFixture]
    public class WatiNTests
    {
        [Test]
        [Explicit]
        public void LoadHtmlWatiN_PogodaModiin_Grabbing()
        {
            using (var browser = new IE("http://pogoda.co.il/israel/modiin"))
            {
                Settings.WaitForCompleteTimeOut = 999999999; /*увеличиваем тайм-аут на всякий случай*/
                var doc = new HtmlDocument(); /* инсталляция объекта парсера HTML Agility Pack*/
                doc.LoadHtml(browser.Body.OuterHtml); /*помещаем в парсер полученный html с страницы yvison.kz*/
                Console.WriteLine(doc.ToString());
            }
            Assert.Pass();
        }
    }
}