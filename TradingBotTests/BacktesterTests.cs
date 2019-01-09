using System;
using Xunit;
using TradingBot;

namespace TradingBotTests
{
    public class BacktesterTests
    {
        [Fact]
        public void BacktesterWalletTest()
        {
            // This is not bitcoin data oopsieeee but it still works for our purposes
            Backtester bt = new TradingBot.Backtester(
                @"../../../../BacktestData/1mTestData.json");

            Assert.Equal(0.270, Math.Round(bt.CurrentPortfolio.GetBTCBalance(), 3));
            Assert.Equal(1000, bt.CurrentPortfolio.GetUSDTBalance());
        }
    }
}
