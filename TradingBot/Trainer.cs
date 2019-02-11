using System;
using System.Collections.Generic;
using System.Text;
using TradingAlgorithm;

namespace TradingBot
{
    class Trainer
    {
        private const int valuesCount = 5;

        public static void GradientDescent(int count, double learnRate)
        {
            Const.log = false;

            // Load Data
            DataLoader dl = new DataLoader(Const.backtestPath);

            double[] AlgorithmValues = GetValues();

            for (int n = 0; n < count; n++)
            {

            }
        }

        static void AssignValues(double[] values)
        {
            if (values.Length != valuesCount)
                throw new Exception("Values of the wrong length passed in.");

            Const.RSILow = (int)values[0];
            Const.RSIHigh = (int)values[1];
            Const.TPPercentage = values[2];
            Const.SLPercentage = values[3];
            Const.RequiredPositionRestMins = (int)values[4];
        }

        static double[] GetValues()
        {
            double[] values = new double[valuesCount];

            values[0] = Const.RSILow;
            values[1] = Const.RSIHigh;
            values[2] = Const.TPPercentage;
            values[3] = Const.SLPercentage;
            values[4] = Const.RequiredPositionRestMins;

            return values;
        }
    }
}
