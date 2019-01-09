using System;

namespace TradingAlgorithm
{
    public class TradingAlgorithm
    {
        private Indicators indicators;

        public TradingAlgorithm()
        {
            indicators = new Indicators();
        }

        public void Tick(DataPoint Point)
        {
            Point = indicators.Tick(Point);
        }
    }
}
