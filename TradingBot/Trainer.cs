using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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

                double reference = 0;

                // For all of our array
                // I goes for one more - when zero it is doing the reference value test
                Parallel.For(0, valuesCount + 1, i =>
                {
                    if (i == 0)
                    {
                        Backtester firstBT = new Backtester(dl);
                        reference = firstBT.Backtest(1000, 1500);
                        Console.WriteLine("   Reference: " + reference);

                        if (reference > bestProfit)
                        {
                            bestProfit = reference;
                            BestValues = AlgorithmValues;
                            Console.WriteLine("New Best! " + bestProfit);
                            SaveValues();
                        }
                    }
                    else
                    {
                        i = i - 1; // Here i needs to be dropped back down to its expected, to fit the range of the array.

                        double[] intermediateValues = GetValues();
                        intermediateValues[i] = intermediateValues[i] * (1 + learnRate);
                        AssignValues(intermediateValues);

                        Backtester bt = new Backtester(dl);
                        scores[i] = bt.Backtest(1000, 1500);

                        Console.WriteLine("   Tested value " + i + " of " + intermediateValues[i] + ": " + scores[i]);

                        if (scores[i] > bestProfit)
                        {
                            bestProfit = scores[i];
                            BestValues = intermediateValues;
                            Console.WriteLine("New Best! " + bestProfit);
                            SaveValues();
                        }
                    }
                });

                if (reference == 0) 
                    throw new Exception("Reference value not assigned.");
                
                Console.WriteLine("   Adjusted new values: ");
                double[] fractions = new double[valuesCount];
                for (int i = 0; i < valuesCount; i++)
                {
                    fractions[i] = (scores[i] - reference) / reference;
                    AlgorithmValues[i] += fractions[i] * AlgorithmValues[i];
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
            Const.TPClampValue = values[3];
            Const.SLClampValue = values[4];
            Const.PortfolioStartRatio = values[5];
        }

        static double[] GetValues()
        {
            double[] values = new double[valuesCount];

            values[0] = Const.TPPercentage;
            values[1] = Const.SLPercentage;
            values[2] = Const.TradeValue;
            values[3] = Const.TPClampValue;
            values[4] = Const.SLClampValue;
            values[5] = Const.PortfolioStartRatio;

            return values;
        }

        public static void LoadValues()
        {
            string raw = File.ReadAllText(Const.trainingValuesPath);

            string[] strings = raw.Split(',');

            if (strings.Length != valuesCount)
                throw new Exception("Number of values incorrect.");

            double[] values = new double[valuesCount];

            for (int i = 0; i < strings.Length; i++)
            {
                values[i] = Convert.ToDouble(strings[i]);
            }

            AssignValues(values);
        }

        public static void SaveValues()
        {
            double[] values = GetValues();

            string output = "";

            for (int i = 0; i < values.Length; i++)
            {
                output += values[i].ToString();
                if (i != values.Length - 1)
                    output += ", ";
            }

            File.WriteAllText(Const.trainingValuesPath, output);
        }
    }
}
