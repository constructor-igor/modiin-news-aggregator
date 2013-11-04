using System.Collections.Generic;
using GoogleChartSharp;
using NUnit.Framework;

namespace AggregatorTests.Feasibility
{
    [TestFixture]
    public class GoogleChartTests
    {
        [Test]
        [Explicit]
        public void Test()
        {
            int[] line1 = new int[] { 10, 15, 50, 34, 10, 25 };
            int[] line2 = new int[] { 15, 20, 60, 44, 20, 35 };

            var dataSet = new List<int[]> {line1, line2};

            LineChart lineChart = new LineChart(250, 150);
            lineChart.SetTitle("Single DataSet Per Line", "0000FF", 14);
            lineChart.SetData(dataSet);
            ChartAxis chartAxis = new ChartAxis(ChartAxisType.Bottom);
            chartAxis.SetRange(0, 23);
            lineChart.AddAxis(chartAxis);
            lineChart.AddAxis(new ChartAxis(ChartAxisType.Left));

            string url = lineChart.GetUrl();
        }
    }
}