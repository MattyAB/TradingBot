using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
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

            int longWin = 0;
            int longLoss = 0;
            int shortWin = 0;
            int shortLoss = 0;

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
                        algorithm.AddPosition(signal.pos);
                    }
                    else
                    {
                        algorithm.RemovePosition(signal.id);

                        if (signal.longOrShort)
                            if (signal.win)
                                longWin++;
                            else
                                longLoss++;
                        else
                            if (signal.win)
                                shortWin++;
                            else
                                shortLoss++;

                    }
                }

                if (CurrentPortfolio.USDTBalance < 0 | CurrentPortfolio.BTCBalance < 0)
                    Console.WriteLine("One of the portfolio balances went below 0...");

                // Finish up by committing the current wallet to our history.
                CurrentPortfolio = interimWallet;
            }

            AlgorithmData finishingData = algorithm.FinishUp();
            FinalPlots(finishingData);

            TradingAlgorithm.Log.FinishUp(PortfolioHistory[0].GetTotalBalance(dl.GetFirst().close),
                PortfolioHistory[PortfolioHistory.Count - 1].GetTotalBalance(dl.GetFirst().close),
                PortfolioHistory[PortfolioHistory.Count - 1].GetTotalBalance(dl.getPointAt(Const.Points).close),
                longWin, longLoss, shortWin, shortLoss);
        }

        public int RSIDecision(DataPoint Point)
        {
            if (Point.RSI < 10)
                return 1;
            if (Point.RSI > 90)
                return -1;
            return 0;
        }

        public void FinalPlots(AlgorithmData data)
        {
            PortfolioHistory.RemoveAt(0);
            PortfolioHistory.RemoveAt(0);

            // Prepare data by cutting it down to one data point every n points (taking an average)
            List<double[]> portfolios = TruncatePortfolios(PortfolioHistory, data.points, 500);
            data = TruncateData(data, 500);

            Plotter finalPlot = new Plotter(FinalPlotSetup(), "FinalPlot");

            for (int i = 0; i < data.points.Count; i++)
            {
                Dictionary<string, double[]> plotValues = new Dictionary<string, double[]>();

                plotValues.Add("price_graph",
                    new[]{ (Int32)(data.points[i].openTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                        data.points[i].close
                    });

                plotValues.Add("pos_graph",
                    new[]{ (Int32)(data.points[i].openTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                        (double)data.longPos[i],
                        (double)data.shortPos[i],
                        (double)(data.shortPos[i] + data.longPos[i])
                    });

                plotValues.Add("value_graph",
                    new[]{ (Int32)(data.points[i].openTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                        portfolios[i][0], // BTC
                        portfolios[i][1], // USD
                        portfolios[i][2] // Total
                    });

                finalPlot.PushValues(plotValues);
            }

            finalPlot.BuildSite(true);
        }

        private List<double[]> TruncatePortfolios(List<Wallet> portfolioHistory, List<DataPoint> points, int n)
        {
            List<double[]> output = new List<double[]>();

            for (int i = n; i < portfolioHistory.Count; i += n)
            {
                // Initialise our values for the averages
                double a = 0;
                double b = 0;
                double c = 0;

                // Go across the last time period and add up the values
                for (int j = i - n; j < i; j++)
                {
                    a += portfolioHistory[j].BTCBalance;
                    b += portfolioHistory[j].USDTBalance;
                    c += portfolioHistory[j].GetTotalBalance(points[0].close);
                }

                output.Add(new double[]
                {
                    a / n, b / n, c / n
                });
            }

            return output;
        }

        private AlgorithmData TruncateData(AlgorithmData data, int n)
        {
            AlgorithmData output = new AlgorithmData();
            output.points = new List<DataPoint>();
            output.longPos = new List<double>();
            output.shortPos = new List<double>();

            for (int i = n; i < data.points.Count; i += n)
            {
                // Initialise our values for the averages
                double a = 0; // Price
                double b = 0; // Long
                double c = 0; // Short

                // Go across the last time period and add up the values
                for (int j = i - n; j <= i; j++)
                {
                    a += data.points[j].close;
                    b += data.longPos[j];
                    c += data.shortPos[j];
                }

                DataPoint outPoint = new DataPoint();
                outPoint.close = a / n;
                outPoint.openTime = data.points[i].openTime;
                output.points.Add(outPoint);

                output.longPos.Add(b / n);
                output.shortPos.Add(c / n);
            }

            return output;
        }

        public List<PlotterValues> FinalPlotSetup()
        {
            List<PlotterValues> plotterSetup = new List<PlotterValues>();

            PlotterValues Price = new PlotterValues();
            Price.title = "Price";
            Price.jsName = "drawPrice";
            Price.htmlName = "price_graph";
            Price.columnNames = new string[] { "Timestamp", "Price" };

            PlotterValues Positions = new PlotterValues();
            Positions.title = "Positions";
            Positions.jsName = "drawPos";
            Positions.htmlName = "pos_graph";
            Positions.columnNames = new string[] { "Timestamp", "LongPos", "ShortPos", "TotalPos" };

            PlotterValues Value = new PlotterValues();
            Value.title = "Value";
            Value.jsName = "drawValue";
            Value.htmlName = "value_graph";
            Value.columnNames = new string[] { "Timestamp", "BTCValue", "USDTValue", "TotalValue" };

            plotterSetup.Add(Price);
            plotterSetup.Add(Positions);
            plotterSetup.Add(Value);

            return plotterSetup;
        }
    }
}
