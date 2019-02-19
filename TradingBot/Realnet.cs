using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Enums;
using Binance.API.Csharp.Client.Models.Market;
using TradingAlgorithm;

namespace TradingBot
{
    class Realnet
    {
        private TradingAlgorithm.TradingAlgorithm algorithm;
        private BinanceClient binanceClient;
        private int TickNumber = 0;

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

            File.Delete(Const.logPath); // Empties or creates our log.

            // First ticks

            var candlestick = binanceClient.GetCandleSticks("btcusdt", TimeInterval.Minutes_1).Result;

            foreach (Candlestick c in candlestick)
            {
                DataPoint point = FromCandlestick(c, TickNumber);

                algorithm.Tick(point, 0);

                TickNumber++;
            }

            algorithm.TruncatePositions();
        }

        public void Tick(object source, ElapsedEventArgs e)
        {
            var candlestick = binanceClient.GetCandleSticks("btcusdt", TimeInterval.Minutes_1, limit: 1).Result;

            if(candlestick.Count() != 1) throw new Exception("More candlesticks than expected?");

            DataPoint point = FromCandlestick(candlestick.Last(), TickNumber);

            Log.logText(TickNumber, "Got tick of closing price " + point.close);

            TickNumber++;
        }

        private DataPoint FromCandlestick(Candlestick c, int tickNo)
        {
            DataPoint point = new DataPoint();
            
            point.open = (double)c.Open;
            point.close = (double)c.Close;
            point.high = (double)c.High;
            point.low = (double)c.Low;
            point.volume = (double)c.Volume;
            point.openTime = new DateTime(1970, 1, 1);
            point.openTime.AddMilliseconds(c.OpenTime);
            point.closeTime = new DateTime(1970, 1, 1);
            point.openTime.AddMilliseconds(c.CloseTime); ;
            point.TickNumber = tickNo;

            return point;
        }
    }
}
