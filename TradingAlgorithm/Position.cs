using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TradingAlgorithm
{
    class Position
    {
        private bool longOrShort;
        private int openerUsed;
        private DataPoint OpeningPoint;


        public double takeProfit;
        public double stopLoss;

        public Position(bool longOrShort, int opener, DataPoint OpeningPoint)
        {
            this.longOrShort = longOrShort;
            openerUsed = opener;
            this.OpeningPoint = OpeningPoint;

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
    }
}
