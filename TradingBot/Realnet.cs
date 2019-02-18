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

            var serverTime = binanceClient.GetServerTime().Result;

            List<PositionOpener.PositionDecision> decisions = new List<PositionOpener.PositionDecision>();
            decisions.Add(Backtester.RSIDecision);
            algorithm = new TradingAlgorithm.TradingAlgorithm(
                Convert.ToInt32(serverTime.ServerTime / 1000), 
                decisions);
        }

        public void Tick(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Tick!");
        }
    }
}
