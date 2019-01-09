using System;
using System.Collections.Generic;
using System.Text;
using TradingAlgorithm;

namespace TradingBot
{
    class Backtester
    {
        public Wallet _CurrentPortfolio;
        public Wallet CurrentPortfolio
        {
            // Standard GetSet, but when this is being written to, it is also added to the PortfolioHistory list
            get { return _CurrentPortfolio; }
            private set
            {
                _CurrentPortfolio = value;
                PortfolioHistory.Add(value);
            }
        }
        List<Wallet> PortfolioHistory = new List<Wallet>();

        public Backtester(string path)
        {
            // Load Data
            DataLoader dl = new DataLoader(path);

            // Create first portfolio, with $1000 of BTC and $1000 of $
            CurrentPortfolio = new Wallet(dl.GetFirst().open * 1000, 1000);

            TradingAlgorithm.TradingAlgorithm algorithm = new TradingAlgorithm.TradingAlgorithm();
        }
    }
}
