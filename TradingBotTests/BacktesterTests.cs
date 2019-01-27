using System;
using TradingAlgorithm;
using Xunit;
using TradingBot;

namespace TradingBotTests
{
    public class BacktesterTests
    {
        [Fact]
        public void BacktesterWalletTest()
        {
            Const.Points = 1200;

            // This is not bitcoin data oopsieeee but it still works for our purposes
            Backtester bt = new TradingBot.Backtester(
                @"../../../../BacktestData/1mTestData.json", 1000, 1200);

            Assert.Equal(0.270, Math.Round(bt.CurrentPortfolio.BTCBalance, 3));
            Assert.Equal(1000, bt.CurrentPortfolio.USDTBalance);
        }
    }
}
