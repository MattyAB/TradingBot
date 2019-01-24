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

        public Backtester(string path)
        {
            // Load Data
            DataLoader dl = new DataLoader(path);

            // Create first portfolio, with $1000 of BTC and $1000 of $
            CurrentPortfolio = new Wallet(1000 / dl.GetFirst().open, 1000);

            // Create TA Object
            int startTime = Convert.ToInt32((dl.GetFirst().openTime - new DateTime(1970, 1, 1)).TotalSeconds);
            List<PositionOpener.PositionDecision> decisions = new List<PositionOpener.PositionDecision>();
            decisions.Add(RSIDecision);
            TradingAlgorithm.TradingAlgorithm algorithm = new TradingAlgorithm.TradingAlgorithm(startTime, decisions);

            while(true) // For each tick we have stored
            {
                DataPoint Point;
                try
                {
                    // Get the next data point
                    Point = dl.GetNextPoint();
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // When it gets to the end of the list
                    break;
                }

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

            Directory.CreateDirectory(@"./../../../../Exports");

            List<string[]> export = algorithm.Export();
            using (StreamWriter outfile = new StreamWriter(@"./../../../../Exports/" + DateTime.Now.ToFileTimeUtc() + ".csv"))
            {
                for (int x = 0; x < export.Count; x++)
                {
                    string content = "";
                    for (int y = 0; y < export[x].Length; y++)
                    {
                        content += export[x][y].ToString() + ",";
                    }
                    //trying to write data to csv
                    outfile.WriteLine(content);
                }
            }
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
