using System;
using System.Collections.Generic;

namespace TradingAlgorithm
{
    public class MovingAverage : ValuePipe
    {
        private double total = 0;
        private double? ma;

        public MovingAverage(int maxValue) : base(maxValue)
        {

        }

        public new double? Push(double value)
        {
            total += value; // Add value to the total
            double? result = base.Push(value); // Add value to the pipe

            if (result != null)
            {
                double resultNn = result.Value; // Result not null
                total -= resultNn;
                ma = total / MaxValues;
            }
            else
            {
                // Returns null when the pipe is not full
                ma = null;
            }

            return ma;
        }

        public string Trending()
        {
            if (valuesArray.Count < MaxValues)
            {
                return ""; // Empty string, should be picked up by whatever is calling it.
            }
            else
            {
                double lastVal = valuesArray[valuesArray.Count - 1];
                double penultimateVal = valuesArray[valuesArray.Count - 2];
                double penpenultimateVal = valuesArray[valuesArray.Count - 3];
                double lastDir = lastVal - penultimateVal;
                double penultimateDir = penultimateVal - penpenultimateVal;
                return lastDir > penultimateDir ? "rising" : "falling";
            }
        }

        public double GetMa()
        {
            return ma.Value;
        }

        public double? GetMaNullable()
        {
            return ma;
        }

        public double Total()
        {
            return total;
        }

        public double StandardDeviation()
        {
            double sumOfSquares = 0;
            double numValues = 0;
            foreach (double v in valuesArray)
            {
                double divergence = v - ma.Value;
                divergence = divergence * divergence;
                sumOfSquares += divergence;
                numValues++;
            }
            sumOfSquares = sumOfSquares / numValues;
            return Math.Sqrt(sumOfSquares);
        }
    }
}