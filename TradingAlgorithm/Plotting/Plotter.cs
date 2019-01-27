using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TradingAlgorithm
{
    class Plotter
    {
        List<Plot> plots;
        private string fileName;

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

            // Write the html for each chart
            foreach (Plot plot in plots)
            {
                html += "<div id=" + plot.htmlName + " style=\"width: 1800px; height: 500px\"></div>";
            }

            // Finish html
            html += "</body> </html>";

            // Write to file
            File.WriteAllText("E:\\Documents\\Code\\C#\\TradingBot\\Exports\\" + fileName + ".html", html);
        }
    }

    struct PlotterValues
    {
        public string title;
        public string jsName;
        public string htmlName;
        public string[] columnNames;
    }
}
