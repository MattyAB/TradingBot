using System;
using System.IO;
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
            
            // Load Data
            DataLoader dl = new DataLoader(Const.backtestPath);

            // Gather data - this takes a while.
            for (double i = 0.005; i < 0.1; i += 0.005)
            {
                Console.WriteLine(i);
                result[(int)(i * 200) - 1] = new double[20];
                for (double j = 0.005; j < 0.1; j += 0.005)
                {
                    Const.TPPercentage = i;
                    Const.SLPercentage = j;
                    Backtester backtest = new Backtester(dl);
                    result[(int)(i *200) - 1][(int)(j*200) - 1] = backtest.Backtest(1000, 1500);

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
