using System;
using Xunit;
using TradingBot;

namespace TradingBotTests
{
    public class DataLoaderTests
    {
        [Fact]
        public void DataLoaderTest()
        {
            DataLoader dl = new TradingBot.DataLoader(
                @"E:\Documents\Code\C#\TradingBot\Binance_ETHBTC_30m_1512086400000-1514764800000.json");
            
            Assert.Equal(0.04368400, dl.dataPoints[0].open);
            Assert.Equal(0.04375100, dl.dataPoints[0].high);
            Assert.Equal(0.04334200, dl.dataPoints[0].low);
            Assert.Equal(0.04366500, dl.dataPoints[0].close);
            Assert.Equal(2081.85600000, dl.dataPoints[0].volume);
            Assert.Equal(90.79655078, dl.dataPoints[0].quoteVolume);
            Assert.Equal(3904, dl.dataPoints[0].tradeCount);
            Assert.Equal(976.19100000, dl.dataPoints[0].buyBase);
            Assert.Equal(42.59074736, dl.dataPoints[0].buyQuote);
            Assert.Equal(new DateTime(
                2017, 12, 1, 0, 0, 0), dl.dataPoints[0].openTime);
            DateTime Expected = new DateTime(2017, 12, 1, 0, 29, 59);
            Assert.Equal(Expected.AddMilliseconds(999), dl.dataPoints[0].closeTime);
        }
    }
}
