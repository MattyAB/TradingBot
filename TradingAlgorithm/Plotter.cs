using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgorithm
{
    class Plotter
    {
        public Plotter(List<PlotterValues> setup)
        {
            List<Plot> plots = new List<Plot>();

            foreach(PlotterValues plotVals in setup)
            {
                Plot plot = new Plot(plotVals);
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
