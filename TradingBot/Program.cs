using System;

namespace TradingBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Backtester backtest = new Backtester(Const.backtestPath);

            Console.WriteLine("Hello World!");
        }
    }
}
