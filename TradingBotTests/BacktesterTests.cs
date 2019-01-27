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
            Backtester bt = new TradingBot.Backtester(@"../../../../BacktestData/1mTestData.json");

            Assert.Equal(27.0, Math.Round(bt.CurrentPortfolio.BTCBalance));
            Assert.Equal(100000, bt.CurrentPortfolio.USDTBalance);

            bt.Backtest(1000, 1200);
        }
    }
}
