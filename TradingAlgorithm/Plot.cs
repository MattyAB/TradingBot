using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgorithm
{
    class Plot
    {
        public string title;
        public string jsName;
        public string htmlName;
        public string[] columnNames;

        public Plot(PlotterValues values)
        {
            this.title = values.title;
            this.jsName = values.jsName;
            this.htmlName = values.htmlName;
            this.columnNames = values.columnNames;
        }
    }
}
