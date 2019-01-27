using TradingAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TradingBotTests
{
    public class ValuePipeTests
    {
        ValuePipe VP = new ValuePipe(5);

        [Fact]
        public void ValuePipeTest()
        {
            Assert.Null(VP.Push(5));
            Assert.True(!VP.PassedZero());
            Assert.Null(VP.Direction());
            Assert.Null(VP.TrendSlope());
            TrendingEmpty();

            Assert.Null(VP.Push(10));
            Assert.True(!VP.PassedZero());
            DirectionUp();
            TrendingEmpty();

            Assert.Null(VP.Push(15));
            Assert.True(!VP.PassedZero());
            DirectionUp();
            TrendingEmpty();

            Assert.Null(VP.Push(2));
            Assert.True(!VP.PassedZero());
            DirectionDown();
            TrendingEmpty();

            Assert.Null(VP.Push(13));
            Assert.True(!VP.PassedZero());
            DirectionUp();
            TrendingUp();

            double actual = VP.Push(4).Value;
            TrendingDown();
            Assert.Equal(5, actual);

            MaxValueTest();
            MinValueTest();
            Differences();

            VP.Push(-2);
            Assert.True(VP.PassedZero());
            TrendingUp();

            VP.Push(3);
            Assert.True(VP.PassedZero());
            TrendingUp();

            Assert.Equal(4, VP.TrendSlope());
        }

        public void MaxValueTest()
        {
            Assert.Equal(15, VP.MaxValue());
        }

        public void MinValueTest()
        {
            Assert.Equal(2, VP.MinValue());
        }

        public void TrendingEmpty()
        {
            Assert.Equal("", VP.Trending());
        }

        public void TrendingUp()
        {
            Assert.Equal("rising", VP.Trending());
        }

        public void TrendingDown()
        {
            Assert.Equal("falling", VP.Trending());
        }

        public void DirectionUp()
        {
            Assert.True(VP.Direction() > 0);
        }

        public void DirectionDown()
        {
            Assert.True(VP.Direction() < 0);
        }

        public void Differences()
        {
            Assert.Equal(new double[] {5, -13, 11, -9}, VP.Differences());
        }
    }
}