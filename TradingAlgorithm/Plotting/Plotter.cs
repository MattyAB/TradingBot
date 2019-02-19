using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TradingAlgorithm
{
    public class Plotter
    {
        List<Plot> plots;
        public string fileName;
        private string textLog;

        public int valuesCount
        {
            get
            {
                int total = 0;

                foreach (Plot plot in plots)
                {
                    total += plot.values.Count;
                }

                return total / plots.Count;
            }
        }

        public Plotter(List<PlotterValues> setup, string fileName)
        {
            this.fileName = fileName;
            plots = new List<Plot>();

            foreach(PlotterValues plotVals in setup)
            {
                plots.Add(new Plot(plotVals));
            }
        }

        public async Task PushValues(Dictionary<string, double[]> values)
        {
            // For every set of values we have
            foreach (KeyValuePair<string, double[]> value in values)
            {
                // Find the right plot to put it in
                foreach (Plot plot in plots)
                {
                    if (plot.htmlName == value.Key)
                    {
                        // And put it in
                        plot.PushValues(value.Value);
                    }
                }
            }
        }

        public async Task BuildSite()
        {
            // Initialise html
            string html =
                "  <html> <head> <script type = \"text/javascript\" src = \"https://www.gstatic.com/charts/loader.js\"></script>" +
                "<script type = \"text/javascript\"> google.charts.load('current', { 'packages':['corechart'] });";

            // Call the js
            foreach (Plot plot in plots)
            {
                html += "google.charts.setOnLoadCallback(" + plot.jsName + ");";
            }

            // Write the js
            foreach (Plot plot in plots)
            {
                html += plot.BuildJS();
            }

            // Between JS and body
            html += "</script> </head> <body>";

            // Add text log
            html += "<p>" + textLog + "</p>";

            // Write the html for each chart
            foreach (Plot plot in plots)
            {
                html += "<div id=" + plot.htmlName + " style=\"width: 1800px; height: 500px\"></div>";
            }

            // Finish html
            html += "</body> </html>";

            // Write to file
            File.WriteAllText(Const.exportPath + fileName + ".html", html);
        }

        public void BuildSite(bool t)
        {
            // Initialise html
            string html =
                "  <html> <head> <script type = \"text/javascript\" src = \"https://www.gstatic.com/charts/loader.js\"></script>" +
                "<script type = \"text/javascript\"> google.charts.load('current', { 'packages':['corechart'] });";

            // Call the js
            foreach (Plot plot in plots)
            {
                html += "google.charts.setOnLoadCallback(" + plot.jsName + ");";
            }

            // Write the js
            foreach (Plot plot in plots)
            {
                html += plot.BuildJS();
            }

            // Between JS and body
            html += "</script> </head> <body>";

            // Add text log
            html += "<p>" + textLog + "</p>";

            // Write the html for each chart
            foreach (Plot plot in plots)
            {
                html += "<div id=" + plot.htmlName + " style=\"width: 1800px; height: 500px\"></div>";
            }

            // Finish html
            html += "</body> </html>";

            // Write to file
            File.WriteAllText(Const.exportPath + fileName + ".html", html);
        }

        public void addText(int TickNumber, string text)
        {
            textLog += TickNumber + ": " + text + "<br/>";
        }

        public void FixValues()
        {
            for (int i = 0; i < plots[0].values.Count; i++)
            {
                if (plots[0].values[i][2] == 0)
                    plots[0].values[i][2] = plots[0].values[plots[0].values.Count - 1][2];
                if (plots[0].values[i][3] == 0)
                    plots[0].values[i][3] = plots[0].values[plots[0].values.Count - 1][3];
                if (plots[0].values[i][4] == 0)
                    plots[0].values[i][4] = plots[0].values[plots[0].values.Count - 1][4];
            }
        }

        public void popValue()
        {
            foreach (Plot plot in plots)
            {
                plot.values.RemoveAt(0);
            }

            FixValues();

            BuildSite();
        }
    }

    public struct PlotterValues
    {
        public string title;
        public string jsName;
        public string htmlName;
        public string[] columnNames;
    }
}
