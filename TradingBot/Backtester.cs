using System;
using System.Collections.Generic;
using System.Text;
using TradingAlgorithm;

namespace TradingBot
{
    class Backtester
    {
        private Wallet CurrentPortfolio
        {
            // Standard GetSet, but when this is being written to, it is also added to the PortfolioHistory list
            get { return CurrentPortfolio; }
            set
            {
                CurrentPortfolio = value;
                PortfolioHistory.Add(value);
            }
        }
        List<Wallet> PortfolioHistory = new List<Wallet>();

        public Backtester(string path)
        {
            DataLoader dl = new DataLoader(path);

            TradingAlgorithm.TradingAlgorithm algorithm = new TradingAlgorithm.TradingAlgorithm();
        }
    }
}
