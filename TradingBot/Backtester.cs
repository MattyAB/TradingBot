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

            // Create first portfolio, with $1000 of BTC and $1000 of $
            CurrentPortfolio = new Wallet(Const.PortfolioStartValue / dl.GetFirst().close, Const.PortfolioStartValue);
            Console.WriteLine(CurrentPortfolio.GetTotalBalance(dl.GetFirst().close));
            CurrentPortfolio = currentPortfolio; // Set this to make a second instance in PortfolioHistory
                                                 // - bit of a clunky solution but necessary because on line 
                                                 //    Wallet interimWallet = CurrentPortfolio;
                                                 // This passes a reference and so when interimWallet the PortfolioHistory[0] is edited
                                                 // Which means we don't get full coverage of our portfolio history...

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

                List<PositionSignal> signals = algorithm.Tick(Point, CurrentPortfolio.GetTotalBalance(Point.close));

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

                if(CurrentPortfolio.GetTotalBalance(Point.close) < 0)
                    throw new Exception("Portfolio cannot be lower than 0");

                // Finish up by committing the current wallet to our history.
                CurrentPortfolio = interimWallet;
            }

            algorithm.FinishUp();

            Console.WriteLine(PortfolioHistory[0].GetTotalBalance(dl.GetFirst().close));
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
