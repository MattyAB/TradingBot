using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgorithm
{
    class Plotter
    {
        List<Plot> plots;

        public Plotter(List<PlotterValues> setup)
        {
            plots = new List<Plot>();

            foreach(PlotterValues plotVals in setup)
            {
                plots.Add(new Plot(plotVals));
            }
        }

        public void PushValues(Dictionary<string, double[]> values)
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
    }

    struct PlotterValues
    {
        public string title;
        public string jsName;
        public string htmlName;
        public string[] columnNames;
    }
}
