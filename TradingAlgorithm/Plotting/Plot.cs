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
    }
}
