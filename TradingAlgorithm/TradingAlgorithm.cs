using System;
using System.Collections.Generic;

namespace TradingAlgorithm
{
    public class TradingAlgorithm
    {
        private Indicators indicators;
        private List<string[]> exports;

        public TradingAlgorithm(int startTimeStamp)
        {
            indicators = new Indicators(startTimeStamp, true);
            exports = new List<string[]>();
        }

        public void Tick(DataPoint Point)
        {
            Point = indicators.Tick(Point);
            exports.Add(Point.Export());
        }

        public List<string[]> Export()
        {
            return exports;
        }

        public void FinishUp()
        {
            indicators.FinishPlots();
        }
    }
}
