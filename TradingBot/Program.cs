using System;

namespace TradingBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Backtester backtest = new Backtester(@"E:\Documents\Code\C#\TradingBot\data.json");

            Console.WriteLine("Hello World!");
        }
    }
}
