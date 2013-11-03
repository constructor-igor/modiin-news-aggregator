using ModiinNewsAggregator.Interfaces;
using ModiinNewsAggregator.Producers;
using Moq;
using NUnit.Framework;

namespace AggregatorTests
{
    [TestFixture]
    public class UpDownProducerTests
    {
        [Test]
        public void TestUp()
        {
            var mock = new Mock<IProducer>();
            PogodaMessage pogodaMessage = new PogodaMessage("+18", "11:50");
            mock.Setup(foo => foo.GetMessage()).Returns(() => pogodaMessage);

            var producer = new UpDownProducer(mock.Object);
            IMessage firstMessage = producer.GetMessage();
            Assert.AreEqual("#Modiin - now 18°C. Source: http://pogoda.co.il/israel/modiin, updated 11:50", firstMessage.Text);

            pogodaMessage = new PogodaMessage("+19", "12:10");
            IMessage secondMessage = producer.GetMessage();
            Assert.AreEqual("↑ #Modiin - now 19°C. Source: http://pogoda.co.il/israel/modiin, updated 12:10", secondMessage.Text);
        }

        [Test]
        public void TestDown()
        {
            var mock = new Mock<IProducer>();
            PogodaMessage pogodaMessage = new PogodaMessage("+18", "11:50");
            mock.Setup(foo => foo.GetMessage()).Returns(() => pogodaMessage);

            var producer = new UpDownProducer(mock.Object);
            IMessage firstMessage = producer.GetMessage();
            Assert.AreEqual("#Modiin - now 18°C. Source: http://pogoda.co.il/israel/modiin, updated 11:50", firstMessage.Text);

            pogodaMessage = new PogodaMessage("+17", "12:10");
            IMessage secondMessage = producer.GetMessage();
            Assert.AreEqual("↓ #Modiin - now 17°C. Source: http://pogoda.co.il/israel/modiin, updated 12:10", secondMessage.Text);
        }
    }
}