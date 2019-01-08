using System;
using System.Collections.Generic;
using System.Text;
using TradingAlgorithm;

namespace TradingBot
{
    class Backtester
    {
        public Backtester(string path)
        {
            DataLoader dl = new DataLoader(path);

            TradingAlgorithm.TradingAlgorithm algorithm = new TradingAlgorithm.TradingAlgorithm();
        }
    }
}
