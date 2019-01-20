using System;
using System.Collections.Generic;

namespace TradingAlgorithm
{
    public class TradingAlgorithm
    {
        private Indicators indicators;
        private List<string[]> exports;
        private PositionOpener opener;

        public TradingAlgorithm(int startTimeStamp, List<PositionOpener.PositionDecision> decisions)
        {
            indicators = new Indicators(startTimeStamp, true);
            exports = new List<string[]>();
            opener = new PositionOpener(decisions);
        }

        public void Tick(DataPoint Point)
        {
            Point = indicators.Tick(Point);
            exports.Add(Point.Export());
            int choice = opener.Tick(Point);
            if (choice != 0)
            {
                Console.WriteLine(choice);
            }
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
