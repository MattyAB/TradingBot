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
        public bool win;

        // Creation signal
        public PositionSignal(bool longOrShort, int opener, DataPoint OpeningPoint, int id)
        {
            pos = new Position(longOrShort, opener, OpeningPoint, id);
            this.id = id;
            this.longOrShort = longOrShort;
        }

        // Removal signal
        public PositionSignal(Position p, bool win)
        {
            pos = p;
            add = false;
            id = p.id;
            longOrShort = p.longOrShort;
            this.win = win;
        }
    }
}
