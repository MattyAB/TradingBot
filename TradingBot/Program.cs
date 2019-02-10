using System;
using System.IO;
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

            double[][] result = new double[20][];

            // Gather data - this takes a while.
            Parallel.For(0, 20, i =>
            {
                Console.WriteLine(i);

                // Load Data
                DataLoader dl = new DataLoader(Const.backtestPath);

                Console.WriteLine("L" + i);
                result[i] = new double[20];
                for (int j = 0; j < 20; j++)
                {
                    Const.TPPercentage = (double)(i + 1) / 200;
                    Const.SLPercentage = (double)(j + 1) / 200;
                    Backtester backtest = new Backtester(dl);
                    result[i][j] = backtest.Backtest(1000, 1500);

                    dl.ResetDL();
                }
            });

            Console.WriteLine("Done!");

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
