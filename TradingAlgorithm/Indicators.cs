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
        public List<DataPoint> points = new List<DataPoint>();

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
            PlotterValues Portfolio = new PlotterValues();
            Portfolio.title = "Portfolio Value";
            Portfolio.jsName = "drawPort";
            Portfolio.htmlName = "port_graph";
            Portfolio.columnNames = new string[] { "Timestamp", "Value" };
            plotterSetup.Add(Portfolio);
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

        public DataPoint Tick(DataPoint Point, double portfolioValue)
        {
            Point.MA1 = MA1.Push(Point.close);
            Point.MA2 = MA2.Push(Point.close);
            Point.MA3 = MA3.Push(Point.close);

            RSIPipe.Push(Point.close);
            Point.RSI = calculateRSI(Point);

            if (Const.plotStart < Point.openTime && Const.plotFinish > Point.openTime)
                PushPlotValues(Point, portfolioValue);

            points.Add(Point);

            return Point;
        }

        public async Task PushPlotValues(DataPoint Point, double portfolioValue)
        {
            Dictionary<string, double[]> plotValues = new Dictionary<string, double[]>();
            plotValues.Add("ma_graph",
                new[]{ Point.TickNumber,
                    Point.close,
                    Math.Round(DoubleConvert(Point.MA1), 3),
                    Math.Round(DoubleConvert(Point.MA2), 3),
                    Math.Round(DoubleConvert(Point.MA3), 3) });
            plotValues.Add("port_graph",
                new[]{ Point.TickNumber,
                    portfolioValue });
            plotValues.Add("rsi_graph",
                new[]{ Point.TickNumber,
                    Point.RSI });

            if (Const.log)
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
                plot.BuildSite();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Not enough data points in " + e.StackTrace + " to draw plot.");
            }
        }
    }
}
