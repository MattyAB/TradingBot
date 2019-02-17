using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace TradingAlgorithm
{
    public class Position
    {
        private int openerUsed;
        private DataPoint OpeningPoint;
        private Plotter posPlot;
        private double close;
        private bool pulledStopLoss = false;
        private double LongBreakevenPoint; // Not quite a breakeven price but can train to see what the best value is
        private double ShortBreakevenPoint;
        private bool atLoss = false;

        public bool longOrShort { get; private set; }
        public int id;
        public double takeProfit;
        public double stopLoss;

        public Position(bool longOrShort, int opener, DataPoint OpeningPoint, int id)
        {
            this.longOrShort = longOrShort;
            openerUsed = opener;
            this.OpeningPoint = OpeningPoint;
            this.id = id;
            close = OpeningPoint.close;

            if (longOrShort)
            {
                // Long
                // Aim to make 5%
                takeProfit = OpeningPoint.close * (1 + Const.TPPercentage);
                // Cut losses at 2%
                stopLoss = OpeningPoint.close * (1 - Const.SLPercentage);
            }
            else
            {
                // Short
                // Aim to make 5%
                takeProfit = OpeningPoint.close * (1 - Const.TPPercentage);
                // Cut losses at 2%
                stopLoss = OpeningPoint.close * (1 + Const.SLPercentage);
            }

            LongBreakevenPoint = Math.Round(OpeningPoint.close * Const.LongBreakevenMultiplier, 4);
            ShortBreakevenPoint = Math.Round(OpeningPoint.close * Const.ShortBreakevenMultiplier, 4);

            posPlot = SetupPlot();
            PushPlotValues(OpeningPoint);

            posPlot = Log.logText(OpeningPoint.TickNumber, "Opened position with opener " + opener + " at Point.close " + OpeningPoint.close, posPlot);
        }

        // True when signalling to end position
        public int Tick(DataPoint Point)
        {
            PushPlotValues(Point);

            if (longOrShort)
            {
                // Long

                // Take profit?
                if (Point.close > takeProfit)
                {
                    posPlot = Log.WriteClosePosition(OpeningPoint.close, Point, longOrShort, posPlot, Point.TickNumber);
                    return -1; // SELL
                }
                // Stop loss?
                if (Point.close < stopLoss)
                {
                    posPlot = Log.WriteClosePosition(OpeningPoint.close, Point, longOrShort, posPlot, Point.TickNumber);
                    return -1; // SELL
                }
                
                if (Const.ClampTPSL)
                {
                    // Close in the takeprofit and stoploss values a little
                    // The longer we run the more we move these numbers closer to starting price
                    stopLoss -= (stopLoss - Point.close) * Const.SLClampValue;
                    takeProfit += (takeProfit - Point.close) * Const.TPClampValue;

                    // If the slow MA is going in the wrong direction then pull the stoploss in even more
                    if (Point.MA2 < Point.MA3 && false)
                    {
                        if (!pulledStopLoss)
                        {
                            posPlot.addText(Point.TickNumber, "Pulling stoploss due to MA");
                            pulledStopLoss = true;
                        }
                        stopLoss += (Point.MA3.Value - Point.MA2.Value) / 8;
                        takeProfit -= (Point.MA3.Value - Point.MA2.Value) / 8;

                        // If at a loss, set takeProfit to BreakEvenPoint.close
                        if (Point.close < LongBreakevenPoint)
                        {
                            if (!atLoss)
                            {
                                posPlot.addText(Point.TickNumber, "At a loss. Setting TakeProfit to breakeven");
                                atLoss = true;
                            }
                            takeProfit = LongBreakevenPoint;
                        }
                    }

                    /** We Don't have a chop indicator yet - TODO!!!
                    if (Point.Chop > 60)
                    {
                        stopLoss += (Point.Chop - 60) / 16;
                    }
                    **/
                }
            }
            else
            {
                // Short

                // Take profit?
                if (Point.close < takeProfit)
                {
                    posPlot = Log.WriteClosePosition(OpeningPoint.close, Point, longOrShort, posPlot, Point.TickNumber);
                    return -1; // SELL
                }
                // Stop loss?
                if (Point.close > stopLoss)
                {
                    posPlot = Log.WriteClosePosition(OpeningPoint.close, Point, longOrShort, posPlot, Point.TickNumber);
                    return -1; // SELL
                }

                if (Const.ClampTPSL)
                {
                    // Close in the takeprofit and stoploss values a little
                    // The longer we run the more we move these numbers closer to starting Point.close
                    stopLoss -= (stopLoss - Point.close) * Const.SLClampValue;
                    takeProfit += (takeProfit - Point.close) * Const.TPClampValue;

                    // If the slow MA is going in the wrong direction then pull the stoploss in even more
                    if (Point.MA2 >Point.MA3 && false)
                    {
                        if (!pulledStopLoss)
                        {
                            posPlot.addText(Point.TickNumber, "Pulling stoploss due to MA");
                            pulledStopLoss = true;
                        }
                        stopLoss -= (Point.MA2.Value - Point.MA3.Value) / 16;
                        takeProfit += (Point.MA2.Value - Point.MA3.Value) / 16;

                        // If at a loss, set takeProfit to BreakEvenPoint.close
                        if (Point.close > ShortBreakevenPoint)
                        {
                            if (!atLoss)
                            {
                                posPlot.addText(Point.TickNumber, "At a loss. Setting TakeProfit to breakeven");
                                atLoss = true;
                            }
                            takeProfit = ShortBreakevenPoint;
                        }
                    }
                }
            }



            // No signal to end position
            return 0;
        }

        public async Task PushPlotValues(DataPoint Point)
        {
            Dictionary<string, double[]> plotValues = new Dictionary<string, double[]>();
            plotValues.Add("Values_graph",
                new[]{ Point.TickNumber,
                    OpeningPoint.close,
                    takeProfit,
                    stopLoss,
                    Point.close
                });

            if (Const.log)
                await posPlot.PushValues(plotValues);
        }

        Plotter SetupPlot()
        {
            List<PlotterValues> plotterSetup = new List<PlotterValues>();
            PlotterValues Values = new PlotterValues();
            Values.title = "Values";
            Values.jsName = "drawValues";
            Values.htmlName = "Values_graph";
            Values.columnNames = new string[] { "Timestamp", "Values", "TP", "SL", "StartValues" };
            plotterSetup.Add(Values);
            string LongOrShort = longOrShort ? "_LONG" : "_SHORT";
            return new Plotter(plotterSetup, "Pos_" + id + LongOrShort);
        }

        public void FinishPlot()
        {
            posPlot.BuildSite();
        }

        public bool WinOrLoss()
        {
            if (posPlot.fileName.Contains("WIN"))
                return true;
            if (posPlot.fileName.Contains("LOSS"))
                return false;
            throw new Exception("WinLoss signal attempted when no signal given yet.");
        }
    }
}
