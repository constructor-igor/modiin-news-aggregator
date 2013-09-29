using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AggregatorTests.Feasibility
{
    [TestFixture]
    public class GoogleDirectionsAPITests
    {
        [Test]
        [Explicit]
        public void GoogleDirectionsAPI()
        {
            // https://developers.google.com/maps/documentation/directions/?csw=1
            // https://maps.google.com/maps?ll=31.890494,35.010595&spn=0.012298,0.026157&t=f&z=16&iwloc=ddw0&lci=com.panoramio.all
            //List<DirectionSteps> direction = getDistance("31.890549° N", "35.010592° E", "32.067083° N", "34.777558° E");
            //List<DirectionSteps> direction = getDistance("31.890549° N,35.010592° E", "32.067083° N,34.777558° E");
            List<DirectionSteps> direction = getDistance("Modi'in-Maccabim-Re'ut, Israel", "Tel Aviv, Israel");
            Assert.NotNull(direction);
        }

        public List<DirectionSteps> getDistance(string origin, string destination)
        {
            var requestUrl = String.Format("http://maps.google.com/maps/api/directions/xml?origin=" + origin + "&destination=" + destination + "&sensor=false&units=metric&alternatives=true&departure_time=true");
            try
            {
                var client = new WebClient();
                var result = client.DownloadString(requestUrl);
                //return ParseDirectionResults(result);
                var directionStepsList = new List<DirectionSteps>();
                var xmlDoc = new System.Xml.XmlDocument { InnerXml = result };
                if (xmlDoc.HasChildNodes)
                {
                    var directionsResponseNode = xmlDoc.SelectSingleNode("DirectionsResponse");
                    if (directionsResponseNode != null)
                    {
                        var statusNode = directionsResponseNode.SelectSingleNode("status");
                        if (statusNode != null && statusNode.InnerText.Equals("OK"))
                        {
                            var legs = directionsResponseNode.SelectNodes("route/leg");

                            int stepCount = 0;
                            foreach (System.Xml.XmlNode leg in legs)
                            {
                                //int stepCount = 1;
                                var stepNodes = leg.SelectNodes("step");
                                var steps = new List<DirectionStep>();

                                foreach (System.Xml.XmlNode stepNode in stepNodes)
                                {
                                    var directionStep = new DirectionStep();
                                    directionStep.Index = stepCount++;
                                    directionStep.Distance = stepNode.SelectSingleNode("distance/text").InnerText;
                                    directionStep.Duration = stepNode.SelectSingleNode("duration/text").InnerText;

                                    directionStep.Description = Regex.Replace(stepNode.SelectSingleNode("html_instructions").InnerText, "<[^<]+?>", "");
                                    steps.Add(directionStep);
                                }

                                var directionSteps = new DirectionSteps();
                                //directionSteps.OriginAddress = leg.SelectSingleNode("start_address").InnerText;
                                //directionSteps.DestinationAddress = leg.SelectSingleNode("end_address").InnerText;
                                directionSteps.TotalDistance = leg.SelectSingleNode("distance/text").InnerText;
                                directionSteps.TotalDuration = leg.SelectSingleNode("duration/text").InnerText;
                                directionSteps.Steps = steps;
                                directionStepsList.Add(directionSteps);
                            }
                        }
                    }
                }
                return directionStepsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class DirectionStep
    {
        public int Index { get; set; }
        public string Distance { get; set; }
        public string Duration { get; set; }
        public string Description { get; set; }
    }

    public class DirectionSteps
    {
        public string TotalDistance { get; set; }
        public string TotalDuration { get; set; }
        public List<DirectionStep> Steps { get; set; }
    }

}