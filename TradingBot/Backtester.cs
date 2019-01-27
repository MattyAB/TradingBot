using System;
using System.Collections.Generic;
using System.IO;
using TradingAlgorithm;

namespace TradingBot
{
    public class Backtester
    {
        public Wallet currentPortfolio;
        public Wallet CurrentPortfolio
        {
            // Standard GetSet, but when this is being written to, it is also added to the PortfolioHistory list
            get { return currentPortfolio; }
            private set
            {
                currentPortfolio = value;
                PortfolioHistory.Add(value);
            }
        }
        List<Wallet> PortfolioHistory = new List<Wallet>();

        public Backtester(string path, int start, int end)
        {
            Const.plotStartPoint = start;
            Const.plotFinishPoint = end;

            // Load Data
            DataLoader dl = new DataLoader(path);

            // Create first portfolio, with $100000 of BTC and $100000 of $
            CurrentPortfolio = new Wallet(100000 / dl.GetFirst().open, 100000);

            // Create TA Object
            int startTime = Convert.ToInt32((dl.GetFirst().openTime - new DateTime(1970, 1, 1)).TotalSeconds);
            List<PositionOpener.PositionDecision> decisions = new List<PositionOpener.PositionDecision>();
            decisions.Add(RSIDecision);
            TradingAlgorithm.TradingAlgorithm algorithm = new TradingAlgorithm.TradingAlgorithm(startTime, decisions);

            while(true) // For each tick we have stored
            {
                DataPoint Point;
                // Get the next data point
                Point = dl.GetNextPoint();
                if (Point == null)
                    break;

                Wallet interimWallet = currentPortfolio;

                int tickSignal = algorithm.Tick(Point);
                // Buy BTC
                if (tickSignal > 0)
                {
                    double TradeBTC = Const.TradeValue / Point.close;
                    interimWallet.USDTBalance -= Const.TradeValue;
                    interimWallet.BTCBalance += TradeBTC;
                }

                // Sell BTC
                if (tickSignal < 0)
                {
                    double TradeBTC = Const.TradeValue / Point.close;
                    interimWallet.BTCBalance -= TradeBTC;
                    interimWallet.USDTBalance += Const.TradeValue;
                }

                // Finish up by committing the current wallet to our history.
                currentPortfolio = interimWallet;
            }

            algorithm.FinishUp();

            currentPortfolio.GetTotalBalance(dl.getPointAt(Const.Points).close);
        }

        public int RSIDecision(DataPoint Point)
        {
            if (Point.RSI < 30)
                return 1;
            if (Point.RSI > 70)
                return -1;
            return 0;
        }
    }
}
