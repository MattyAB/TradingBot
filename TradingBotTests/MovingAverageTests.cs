using TradingAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TradingBotTests
{
    public class MovingAverageTests
    {
        MovingAverage MA = new MovingAverage(5);

        [Fact]
        public void MovingAverageTest()
        {
            Assert.Null(MA.Push(5));
            Assert.Null(MA.Push(10));
            Assert.Null(MA.Push(15));
            Assert.Null(MA.Push(2));
            TrendingTest(""); // Not enough values yet
            Assert.Null(MA.Push(13));

            Assert.Equal(9, MA.Push(5));
            Assert.Equal(45, MA.Total());

            TrendingTest("falling");

            Assert.Equal(10, MA.Push(15));
            Assert.Equal(50, MA.Total());

            TrendingTest("rising");

            Assert.Equal(10, MA.GetMa());
            Assert.Equal(10, MA.GetMaNullable().Value);
        }

        public void TrendingTest(string expected)
        {
            Assert.Equal(expected, MA.Trending());
        }
    }
}