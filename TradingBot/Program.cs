using System;
using TradingAlgorithm;

namespace TradingBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Backtester backtest = new Backtester(Const.backtestPath, 1000, 1500);

            Console.WriteLine("Hello World!");

            Console.ReadLine();
        }
    }
}
