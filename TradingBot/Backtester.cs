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

            int longPos = 0;
            int shortPos = 0;

            while (true) // For each tick we have stored
            {
                DataPoint Point;
                // Get the next data point
                Point = dl.GetNextPoint();
                if (Point == null)
                    break;

                Wallet interimWallet = (Wallet)CurrentPortfolio.Clone();

                List<PositionSignal> signals = algorithm.Tick(Point, CurrentPortfolio.GetTotalBalance(dl.GetFirst().close));

                foreach (PositionSignal signal in signals)
                {
                    bool buyOrSell; // True if buy, False if sell

                    if(signal.longOrShort)
                        if (signal.add) // long Buy
                            buyOrSell = true;
                        else // long Sell
                            buyOrSell = false;
                    else
                        if (signal.add) // short Buy
                            buyOrSell = false;
                        else // short Sell
                            buyOrSell = true;

                    // Check we have enough capital to carry out the change
                    if (buyOrSell)
                    {
                        // Buy
                        if (interimWallet.USDTBalance < Const.TradeValue)
                            break;
                    }
                    else
                    {
                        // Sell
                        if (interimWallet.BTCBalance < Const.TradeValue / Point.close)
                            break;
                    }

                    // Carry out the change in our portfolio
                    if (buyOrSell)
                    {
                        // BUY signal

                        double TradeBTC = Const.TradeValue / Point.close;
                        interimWallet.USDTBalance -= Const.TradeValue;
                        interimWallet.BTCBalance += TradeBTC * (1 - 0.001);
                    }
                    else
                    {
                        // SELL signal

                        double TradeBTC = Const.TradeValue / Point.close;
                        interimWallet.BTCBalance -= TradeBTC;
                        interimWallet.USDTBalance += Const.TradeValue * (1 - 0.001);
                    }

                    // And update the stats.
                    if (signal.add)
                    {
                        // If it is a position creation signal
                        if (signal.longOrShort)
                            longPos++;
                        else
                            shortPos++;

                        algorithm.AddPosition(signal.pos);
                    }
                    else
                        algorithm.RemovePosition(signal.id);
                }

                if (CurrentPortfolio.USDTBalance < 0 | CurrentPortfolio.BTCBalance < 0)
                    Console.WriteLine("One of the portfolio balances went below 0...");

                // Finish up by committing the current wallet to our history.
                CurrentPortfolio = interimWallet;
            }

            algorithm.FinishUp();

            TradingAlgorithm.Log.FinishUp(PortfolioHistory[0].GetTotalBalance(dl.GetFirst().close),
                PortfolioHistory[PortfolioHistory.Count - 1].GetTotalBalance(dl.GetFirst().close),
                PortfolioHistory[PortfolioHistory.Count - 1].GetTotalBalance(dl.getPointAt(Const.Points).close),
                longPos, shortPos);
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
