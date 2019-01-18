using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TradingAlgorithm;

namespace TradingBot
{
    public class Backtester
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
            CurrentPortfolio = new Wallet(1000 / dl.GetFirst().open, 1000);

            // Create TA Object
            TradingAlgorithm.TradingAlgorithm algorithm = new TradingAlgorithm.TradingAlgorithm();

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

                algorithm.Tick(Point);
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
    }
}
