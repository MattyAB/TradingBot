using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgorithm
{
    class Indicators
    {
        private ValuePipe MA1;
        private ValuePipe MA2;
        private ValuePipe MA3;

        public Indicators()
        {
            MA1 = new ValuePipe(Const.MA1PipeLength);
            MA2 = new ValuePipe(Const.MA2PipeLength);
            MA3 = new ValuePipe(Const.MA3PipeLength);
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
