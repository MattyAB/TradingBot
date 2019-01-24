using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TradingAlgorithm
{
    class Position
    {
        private int openerUsed;
        private DataPoint OpeningPoint;

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
        }

        // True when signalling to end position
        public bool Tick(DataPoint Point)
        {
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
    }
}
