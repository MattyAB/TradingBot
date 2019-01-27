using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace TradingAlgorithm
{
    class Indicators
    {
        private Plotter plot;

        // MA
        private MovingAverage MA1;
        private MovingAverage MA2;
        private MovingAverage MA3;

        // RSI
        private ValuePipe RSIPipe;
        private double[] prevGainLoss = new double[] {0, 0};

        public Indicators(int startTimeStamp, bool interval1m)
        {
            List<PlotterValues> plotterSetup = new List<PlotterValues>();
            PlotterValues MA = new PlotterValues();
            MA.title = "Moving Averages";
            MA.jsName = "drawMA";
            MA.htmlName = "ma_graph";
            MA.columnNames = new string[] { "Timestamp", "Price", "MA1", "MA2", "MA3" };
            plotterSetup.Add(MA);
            PlotterValues RSI = new PlotterValues();
            RSI.title = "Relative Strength Index";
            RSI.jsName = "drawRSI";
            RSI.htmlName = "rsi_graph";
            RSI.columnNames = new string[] { "Timestamp", "RSI" };
            plotterSetup.Add(RSI);
            plot = new Plotter(plotterSetup, "IndicatorExport");

            if (interval1m)
            {
                DateTime temp = new DateTime(1970, 1, 1);
                Const.plotStart = temp.AddSeconds(startTimeStamp + (60 * Const.plotStartPoint));
                temp = new DateTime(1970, 1, 1);
                Const.plotFinish = temp.AddSeconds(startTimeStamp + (60 * Const.plotFinishPoint));
            }
            else
            {
                throw new Exception("Not 1m interval - not programmed to deal with this...");
            }

            MA1 = new MovingAverage(Const.MA1PipeLength);
            MA2 = new MovingAverage(Const.MA2PipeLength);
            MA3 = new MovingAverage(Const.MA3PipeLength);

            RSIPipe = new ValuePipe(Const.RSIPeriod);
        }

        public DataPoint Tick(DataPoint Point)
        {
            Point.MA1 = MA1.Push(Point.close);
            Point.MA2 = MA2.Push(Point.close);
            Point.MA3 = MA3.Push(Point.close);

            RSIPipe.Push(Point.close);
            Point.RSI = calculateRSI(Point);

            if (Const.plotStart < Point.openTime && Const.plotFinish > Point.openTime)
                Task.Run(() => PushPlotValues(Point));

            return Point;
        }

        public async Task PushPlotValues(DataPoint Point)
        {
            Dictionary<string, double[]> plotValues = new Dictionary<string, double[]>();
            plotValues.Add("ma_graph",
                new[]{ (Int32)(Point.openTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    Point.close,
                    Math.Round(DoubleConvert(Point.MA1), 3),
                    Math.Round(DoubleConvert(Point.MA2), 3),
                    Math.Round(DoubleConvert(Point.MA3), 3) });
            plotValues.Add("rsi_graph",
                new[]{ (Int32)(Point.openTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                    Point.RSI });

            await plot.PushValues(plotValues);
        }

        public int calculateRSI(DataPoint Point)
        {
            double RSI;

            // Not going to be able to calculate RSI
            if (RSIPipe.valuesArray.Count < Const.RSIPeriod)
                return 0;

            double[] gainLoss = RSIPipe.AveGainLoss();
            if (gainLoss[1] == 0)
                gainLoss[1] = 1;

                double smoothRS = (prevGainLoss[0] * (Const.RSIPeriod - 1) + gainLoss[0]) /
                                  (prevGainLoss[1] * (Const.RSIPeriod - 1) + gainLoss[1]);
                RSI = 100 - (100 / (1 + smoothRS));
                prevGainLoss = gainLoss;

            if (double.IsInfinity(RSI) | Math.Abs(RSI) > 2147483647)
                return 0;
            return Convert.ToInt32(RSI);
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
                Task.Run(() => plot.BuildSite());
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Not enough data points in " + e.StackTrace + " to draw plot.");
            }
        }
    }
}
