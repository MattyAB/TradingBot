using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgorithm
{
    public class PositionSignal
    {
        // If false this is a removal signal
        public bool add = true;
        public Position pos;
        public int id;
        public bool longOrShort;

        // Signal inherets Position
        public PositionSignal(bool longOrShort, int opener, DataPoint OpeningPoint, int id)
        {
            pos = new Position(longOrShort, opener, OpeningPoint, id);
            this.id = id;
            this.longOrShort = longOrShort;
        }

        public PositionSignal(Position p)
        {
            pos = p;
            add = false;
            id = p.id;
            longOrShort = p.longOrShort;
        }
    }
}
