using System;
using System.Collections.Generic;
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

            AccountInfo accountInfo = binanceClient.GetAccountInfo().Result;

            Wallet portfolio = GetWallet(accountInfo);

            List<PositionSignal> signals = algorithm.Tick(point, portfolio.GetTotalBalance(point.close));

            foreach (PositionSignal signal in signals)
            {
                bool buyOrSell; // True if buy, False if sell

                if (signal.longOrShort)
                    if (signal.add) // long Buy
                        buyOrSell = true;
                    else // long Sell
                        buyOrSell = false;
                else
                if (signal.add) // short Buy
                    buyOrSell = false;
                else // short Sell
                    buyOrSell = true;

                double TradeValue = portfolio.GetTotalBalance(point.close) * Const.TradeValue;

                // Check we have enough capital to carry out the change
                if (buyOrSell)
                {
                    // Buy
                    if (portfolio.USDTBalance < TradeValue)
                        break;
                }
                else
                {
                    // Sell
                    if (portfolio.BTCBalance < TradeValue / point.close)
                        break;
                }

                // MAKE A CHANGE!!!
            }

            TickNumber++;
        }

        public Wallet GetWallet(AccountInfo info)
        {
            double RawBTC = 0;
            double RawUSDT = 0;

            foreach (var i in info.Balances)
            {
                if (i.Asset == "USD")
                    RawUSDT = (double)i.Free;
                if (i.Asset == "BTC")
                    RawBTC = (double)i.Free;
            }

            return new Wallet(RawBTC, RawUSDT);
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
