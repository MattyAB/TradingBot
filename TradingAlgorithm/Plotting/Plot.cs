using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingAlgorithm
{
    class Plot
    {
        public string title;
        public string jsName;
        public string htmlName;
        public string[] columnNames;

        public List<double[]> values;

        public Plot(PlotterValues values)
        {
            this.title = values.title;
            this.jsName = values.jsName;
            this.htmlName = values.htmlName;
            this.columnNames = values.columnNames;
            this.values = new List<double[]>();
        }

        public void PushValues(double[] values)
        {
            if(values.Length != columnNames.Length)
                throw new Exception("Values  passed to Plot was incorrect length");
            else
                this.values.Add(values);
        }

        public List<double[]> GetValues()
        {
            return values;
        }

        public string BuildJS()
        {
            // Get data in json format
            string data = BuildData();

            // Intro bit
            string js = "function " + jsName + "() { var data = google.visualization.arrayToDataTable(";
            // And add data in
            js += data + ");    ";
            // Options
            js += "var options = { title: '" + title + "', legend: { position: 'bottom' } };";
            // Line to push chart to html
            js += "var chart = new google.visualization.LineChart(document.getElementById('" + htmlName + "'));";
            // Finish up
            js += "chart.draw(data, options); }";

            return js;
        }

        public string BuildData()
        {
            List<string> dataPoints = new List<string>();
            
            dataPoints.Add("['" + String.Join("','", columnNames) + "']");

            foreach(double[] value in values)
            {
                dataPoints.Add("[" + String.Join(",", value) + "]");
            }

            return "[" + String.Join(",", dataPoints) + "]";
        }
    }
}
