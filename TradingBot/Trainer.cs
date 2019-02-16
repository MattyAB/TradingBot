﻿using System;
using System.Collections.Generic;
using System.Text;
using TradingAlgorithm;

namespace TradingBot
{
    class Trainer
    {
        private const int valuesCount = 6;

        public static void GradientDescent(int count, double learnRate)
        {
            Const.log = false;

            // Load Data
            DataLoader dl = new DataLoader(Const.backtestPath);

            double[] AlgorithmValues = GetValues();
            double[] BestValues = GetValues();
            double bestProfit = -10000;

            // Training iterations
            for (int n = 0; n < count; n++)
            {
                Console.WriteLine("Iteration " + n);
                double[] scores = new double[valuesCount];

                Backtester firstBT = new Backtester(dl);
                double reference = firstBT.Backtest(1000, 1500);
                Console.WriteLine("   Reference: " + reference);

                if (reference > bestProfit)
                {
                    bestProfit = reference;
                    BestValues = AlgorithmValues;
                    Console.WriteLine("New Best! " + bestProfit);
                }

                dl.ResetDL();

                // For all of our array
                for (int i = 0; i < valuesCount; i++)
                {
                    double[] intermediateValues = GetValues();
                    intermediateValues[i] = intermediateValues[i] * (1 + learnRate);
                    AssignValues(intermediateValues);

                    Console.Write("   Testing value " + i + " of " + intermediateValues[i] + ": ");

                    Backtester bt = new Backtester(dl);
                    scores[i] = bt.Backtest(1000, 1500);

                    Console.WriteLine(scores[i]);

                    if (scores[i] > bestProfit)
                    {
                        bestProfit = scores[i];
                        BestValues = intermediateValues;
                        Console.WriteLine("New Best! " + bestProfit);
                    }

                    dl.ResetDL();
                }
                
                Console.WriteLine("   Adjusted new values: ");
                double[] fractions = new double[valuesCount];
                for (int i = 0; i < valuesCount; i++)
                {
                    fractions[i] = (scores[i] - reference) / reference;
                    AlgorithmValues[i] -= fractions[i] * AlgorithmValues[i];
                    Console.WriteLine("      " + AlgorithmValues[i]);
                }

                AssignValues(AlgorithmValues);
            }

            AssignValues(BestValues);
            Console.WriteLine("Assigning and testing best values of: ");
            for (int i = 0; i < valuesCount; i++)
            {
                Console.WriteLine("   " + BestValues[i]);
            }
        }

        static void AssignValues(double[] values)
        {
            if (values.Length != valuesCount)
                throw new Exception("Values of the wrong length passed in.");
            
            Const.TPPercentage = values[0];
            Const.SLPercentage = values[1];
            Const.TradeValue = values[2];
            Const.TPSLClampValue = values[3];
            Const.LongBreakevenMultiplier = values[4];
            Const.ShortBreakevenMultiplier = values[5];
        }

        static double[] GetValues()
        {
            double[] values = new double[valuesCount];

            values[0] = Const.TPPercentage;
            values[1] = Const.SLPercentage;
            values[2] = Const.TradeValue;
            values[3] = Const.TPSLClampValue;
            values[4] = Const.LongBreakevenMultiplier;
            values[5] = Const.ShortBreakevenMultiplier;

            return values;
        }
    }
}
