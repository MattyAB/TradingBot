using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgorithm
{
    class Plotter
    {
        public Plotter(List<PlotterValues> setup)
        {

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
