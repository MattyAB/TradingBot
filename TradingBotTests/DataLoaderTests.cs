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
                @"../../../../Binance_ETHBTC_30m_1512086400000-1514764800000.json");

            DataPoint point = dl.GetNextPoint();
            
            Assert.Equal(0.04368400, point.open);
            Assert.Equal(0.04375100, point.high);
            Assert.Equal(0.04334200, point.low);
            Assert.Equal(0.04366500, point.close);
            Assert.Equal(2081.85600000, point.volume);
            Assert.Equal(90.79655078, point.quoteVolume);
            Assert.Equal(3904, point.tradeCount);
            Assert.Equal(976.19100000, point.buyBase);
            Assert.Equal(42.59074736, point.buyQuote);
            Assert.Equal(new DateTime(
                2017, 12, 1, 0, 0, 0), point.openTime);
            DateTime Expected = new DateTime(2017, 12, 1, 0, 29, 59);
            Assert.Equal(Expected.AddMilliseconds(999), point.closeTime);
        }
    }
}
