using System;
using System.Collections.Generic;
using System.IO;
using TradingAlgorithm;

namespace TradingBot
{
    public class Backtester
    {
        private Wallet currentPortfolio;
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
        TradingAlgorithm.TradingAlgorithm algorithm;
        private DataLoader dl;

        public Backtester(string path)
        {
            // Load Data
            dl = new DataLoader(path);

            // Create first portfolio, with $100000 of BTC and $100000 of $
            CurrentPortfolio = new Wallet(100000 / dl.GetFirst().open, 100000);

            // Create TA Object
            int startTime = Convert.ToInt32((dl.GetFirst().openTime - new DateTime(1970, 1, 1)).TotalSeconds);
            List<PositionOpener.PositionDecision> decisions = new List<PositionOpener.PositionDecision>();
            decisions.Add(RSIDecision);
            algorithm = new TradingAlgorithm.TradingAlgorithm(startTime, decisions);
        }

        public void Backtest(int Pstart, int Pend)
        {
            Const.plotStartPoint = Pstart;
            Const.plotFinishPoint = Pend;

            while (true) // For each tick we have stored
            {
                DataPoint Point;
                // Get the next data point
                Point = dl.GetNextPoint();
                if (Point == null)
                    break;

                Wallet interimWallet = CurrentPortfolio;

                List<PositionSignal> signals = algorithm.Tick(Point);

                foreach (PositionSignal signal in signals)
                {
                    if (signal.add)
                    {
                        // If it is a position creation signal
                        if (signal.longOrShort)
                        {
                            // Long creation - BUY
                            double TradeBTC = Const.TradeValue / Point.close;
                            interimWallet.USDTBalance -= Const.TradeValue;
                            interimWallet.BTCBalance += TradeBTC;
                        }
                        else
                        {
                            // Short creation - SELL
                            double TradeBTC = Const.TradeValue / Point.close;
                            interimWallet.BTCBalance -= TradeBTC;
                            interimWallet.USDTBalance += Const.TradeValue;
                        }

                        algorithm.AddPosition(signal.pos);
                    }
                    else
                    {
                        // Position removal signal
                        if (signal.longOrShort)
                        {
                            // Long removal - SELL
                            double TradeBTC = Const.TradeValue / Point.close;
                            interimWallet.BTCBalance -= TradeBTC;
                            interimWallet.USDTBalance += Const.TradeValue;
                        }
                        else
                        {
                            // Short removal - BUY
                            double TradeBTC = Const.TradeValue / Point.close;
                            interimWallet.USDTBalance -= Const.TradeValue;
                            interimWallet.BTCBalance += TradeBTC;
                        }

                        algorithm.RemovePosition(signal.id);
                    }
                }

                // Finish up by committing the current wallet to our history.
                currentPortfolio = interimWallet;
            }

            algorithm.FinishUp();

            Console.WriteLine(currentPortfolio.GetTotalBalance(dl.getPointAt(Const.Points).close));
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
