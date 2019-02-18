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
        private TradingAlgorithm.TradingAlgorithm algorithm;
        private BinanceClient binanceClient;

        public Realnet()
        {
            string[] keys = File.ReadAllLines(Const.apiKey);

            Console.WriteLine("Init realnet");
            ApiClient apiClient = new ApiClient(keys[0], keys[1]);
            binanceClient = new BinanceClient(apiClient);

            List<PositionOpener.PositionDecision> decisions = new List<PositionOpener.PositionDecision>();
            decisions.Add(Backtester.RSIDecision);
            algorithm = new TradingAlgorithm.TradingAlgorithm(
                (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, 
                decisions);
        }

        public void Tick(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Tick!");
        }
    }
}
