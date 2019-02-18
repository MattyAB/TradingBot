using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using Binance.API.Csharp.Client;
using TradingAlgorithm;

namespace TradingBot
{
    class Realnet
    {
        public Realnet()
        {
            string[] keys = File.ReadAllLines(Const.apiKey);

            Console.WriteLine("Init realnet");
            ApiClient apiClient = new ApiClient(keys[0], keys[1]);
            BinanceClient binanceClient = new BinanceClient(apiClient);
        }

        public void Tick(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Tick!");
        }
    }
}
