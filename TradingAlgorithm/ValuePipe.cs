using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TradingAlgorithm
{
    public class ValuePipe
    {
        public int MaxValues { get; private set; }
        public List<double> valuesArray = new List<double>();

        public ValuePipe(int maxValues)
        {
            this.MaxValues = maxValues;
        }

        public double? Push(double value)
        {
            valuesArray.Add(value);

            if (valuesArray.Count > MaxValues)
            {
                double returnValue = valuesArray[0];
                valuesArray.RemoveAt(0);
                return returnValue;
            }
            else
            {
                return null;
            }
        }

        public double MaxValue()
        {
            double max = 0;
            foreach (double value in valuesArray)
                if (value > max)
                    max = value;
            return max;
        }

        public double MinValue()
        {
            double min = valuesArray[0];
            foreach (double value in valuesArray)
                if (value < min)
                    min = value;
            return min;
        }

        // Gives a trend based on the current and previous raw direction values
        public string Trending()
        {
            if (valuesArray.Count < MaxValues)
            {
                return ""; // Empty string, should be picked up by whatever is calling it.
            }
            else
            {
                double lastVal = valuesArray[MaxValues - 1];
                double penultimateVal = valuesArray[MaxValues - 2];
                double penpenultimateVal = valuesArray[MaxValues - 3];
                double lastDir = lastVal - penultimateVal;
                double penultimateDir = penultimateVal - penpenultimateVal;
                string returner = lastDir > penultimateDir ? "rising" : "falling";
                return returner;
            }
        }

        // Gives a raw direction based on the last 2 points
        public double? Direction()
        {
            if (valuesArray.Count < MaxValues)
            {
                return null; // Empty string, should be picked up by whatever is calling it.
            }
            else
            {
                double lastVal = valuesArray[MaxValues - 1];
                double penultimateVal = valuesArray[MaxValues - 2];
                return lastVal - penultimateVal;
            }
        }

        // Returns true if the sign of the last and penultimate values are different
        // Designed to indicate when values cross the centreline
        public bool PassedZero()
        {
            if (valuesArray.Count < MaxValues)
            {
                return false; // Empty string, should be picked up by whatever is calling it.
            }
            else
            {
                double lastVal = valuesArray[MaxValues - 1];
                double penultimateVal = valuesArray[MaxValues - 2];
                if ((lastVal < 0 && penultimateVal > 0) | (lastVal > 0 && penultimateVal < 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // Gives the slope of a trend line plotted through all the points
        public double? TrendSlope()
        {
            if (valuesArray.Count < MaxValues)
            {
                return null; // Empty string, should be picked up by whatever is calling it.
            }
            else
            {
                // Calculate the slope of the trendline through all the points in this pipe

                double a = 0;
                double b1 = 0;
                double b2 = 0;
                double c = 0;
                double d = 0;

                for (int i = 0; i < valuesArray.Count; i++)
                {
                    a += (i + 1) * valuesArray[i];
                    b1 += (i + 1);
                    b2 += valuesArray[i];
                    c += (i + 1) * (i + 1);
                    d += (i + 1);
                }

                a = a * MaxValues;
                double b = b1 + b2;
                c = c * MaxValues;
                d = d * d;
                double m = (a - b) / (c - d);

                return m;
            }
        }
    }
}