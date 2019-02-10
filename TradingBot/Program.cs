using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TradingAlgorithm;

namespace TradingBot
{
    class Program
    {
        static void Main(string[] args)
        {
            TestVars();

            Console.ReadLine();
        }

        static void Backtest()
        {
            // Load Data
            DataLoader dl = new DataLoader(Const.backtestPath);

            Backtester backtest = new Backtester(dl);
            backtest.Backtest(1000, 1500);
        }

        static void TestVars()
        {
            Const.log = false;

            // I variables
            double iStart = 0.085;
            double iEnd = 0.1;
            int Iend = 5;
            double iStep = (iEnd - iStart) / (Iend + 1);

            // J variables
            double jStart = 0.015;
            double jEnd = 0.03;
            int Jend = 5;
            double jStep = (iEnd - iStart) / (Iend + 1);

            double[][] result = new double[Iend + 1][];

            // Load Data
            DataLoader dl = new DataLoader(Const.backtestPath);

            Stopwatch s = new Stopwatch();
            s.Start();

            // Gather data - this takes a while
            for (int i = 0; i <= Iend; i++)
            {
                double I = iStart + (i * iStep);
                Const.TPPercentage = I;

                Console.WriteLine("i: " + I);
                result[i] = new double[Jend + 1];

                for (int j = 0; j <= Jend; j++)
                {
                    try
                    {
                        int soFar = ((i * (Jend + 1)) + j);
                        long seconds = s.ElapsedMilliseconds / 1000;
                        double secondsPer = seconds / soFar;

                        double secondsRemaining = secondsPer * (((Iend + 1) * (Jend + 1)) - soFar);
                        Console.Write(secondsRemaining / 60 + " remaining: ");
                    }
                    catch(DivideByZeroException e)
                    {
                        Console.WriteLine("Dividing by zero.");
                    }

                    double J = jStart + (j * jStep);
                    
                    Console.WriteLine(J);
                    Const.SLPercentage = J;

                    Backtester backtest = new Backtester(dl);
                    result[i][j] = backtest.Backtest(1000, 1500);

                    dl.ResetDL();
                }
            }

            // Finish by spitting the data into a csv.
            using (StreamWriter outfile = new StreamWriter(@"E:\Documents\Code\C#\TradingBot\Testing\TestTPSL.csv"))
            {
                for (int x = 0; x < result.Length; x++)
                {
                    string content = "";
                    for (int y = 0; y < result[x].Length; y++)
                    {
                        content += result[x][y].ToString() + ",";
                    }
                    //trying to write data to csv
                    outfile.WriteLine(content);
                }
            }
        }
    }
}
