using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TradingAlgorithm
{
    public class Position
    {
        private int openerUsed;
        private DataPoint OpeningPoint;
        private Plotter posPlot;
        private double startPrice;
        
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
            startPrice = OpeningPoint.close;

            if (longOrShort)
            {
                // Long
                // Aim to make 5%
                takeProfit = OpeningPoint.close * 1.05;
                // Cut losses at 2%
                stopLoss = OpeningPoint.close * 0.98;
            }
            else
            {
                // Short
                // Aim to make 5%
                takeProfit = OpeningPoint.close * 0.95;
                // Cut losses at 2%
                stopLoss = OpeningPoint.close * 1.02;
            }

            posPlot = SetupPlot();
            PushPlotValues(OpeningPoint);
        }

        // True when signalling to end position
        public bool Tick(DataPoint Point)
        {
            PushPlotValues(Point);

            if (longOrShort)
            {
                // Long

                // Take profit?
                if (Point.close > takeProfit)
                {
                    return true;
                }
                // Stop loss?
                if (Point.close < stopLoss)
                {
                    return true;
                }
            }
            else
            {
                // Short

                // Take profit?
                if (Point.close < takeProfit)
                {
                    return true;
                }
                // Stop loss?
                if (Point.close > stopLoss)
                {
                    return true;
                }
            }

            // No signal to end position
            return false;
        }

        public void PushPlotValues(DataPoint Point)
        {
            Dictionary<string, double[]> plotValues = new Dictionary<string, double[]>();
            plotValues.Add("price_graph",
                new[]{ (Int32)(Point.openTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    Point.close,
                    takeProfit,
                    stopLoss,
                    startPrice
                });

            posPlot.PushValues(plotValues);
        }

        Plotter SetupPlot()
        {
            List<PlotterValues> plotterSetup = new List<PlotterValues>();
            PlotterValues Price = new PlotterValues();
            Price.title = "Price";
            Price.jsName = "drawPrice";
            Price.htmlName = "price_graph";
            Price.columnNames = new string[] { "Timestamp", "Price", "TP", "SL", "StartPrice" };
            plotterSetup.Add(Price);
            return new Plotter(plotterSetup, "Pos_" + id);
        }

        public void FinishPlot()
        {
            posPlot.BuildSite();
        }
    }
}
