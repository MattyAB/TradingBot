using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgorithm
{
    class Indicators
    {
        private Plotter plot;

        private MovingAverage MA1;
        private MovingAverage MA2;
        private MovingAverage MA3;

        public Indicators()
        {
            List<PlotterValues> plotterSetup = new List<PlotterValues>();
            PlotterValues MA = new PlotterValues();
            MA.title = "Moving Averages";
            MA.jsName = "drawMA";
            MA.htmlName = "ma_graph";
            MA.columnNames = new string[] {"Timestamp", "Price", "MA1", "MA2", "MA3"};
            plotterSetup.Add(MA);
            plot = new Plotter(plotterSetup);

            MA1 = new MovingAverage(Const.MA1PipeLength);
            MA2 = new MovingAverage(Const.MA2PipeLength);
            MA3 = new MovingAverage(Const.MA3PipeLength);
        }

        public DataPoint Tick(DataPoint Point)
        {
            Point.MA1 = MA1.Push(Point.close);
            Point.MA2 = MA2.Push(Point.close);
            Point.MA3 = MA3.Push(Point.close);

            Dictionary<string, double[]> plotValues = new Dictionary<string, double[]>();
            plotValues.Add("ma_graph", 
                new []{ (Int32)(Point.openTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    Point.close,
                    Math.Round(DoubleConvert(Point.MA1), 3),
                    Math.Round(DoubleConvert(Point.MA2), 3),
                    Math.Round(DoubleConvert(Point.MA3), 3) });

            plot.PushValues(plotValues);

            return Point;
        }

        public double DoubleConvert(double? value)
        {
            if (value == null)
                return 0;
            else
                return value.Value;
        }

        public void FinishPlots()
        {
            try
            {
                plot.BuildSite();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Not enough data points in " + e.StackTrace + " to draw plot.");
            }
        }
    }
}
