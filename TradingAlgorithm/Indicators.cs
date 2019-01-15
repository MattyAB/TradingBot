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
            MA.columnNames = new string[] {"price", "MA1", "MA2", "MA3"};
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
            return Point;
        }
    }
}
