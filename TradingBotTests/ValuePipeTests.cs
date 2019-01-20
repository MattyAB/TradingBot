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
            Assert.Null(VP.Direction());
            Assert.Null(VP.Push(10));
            DirectionUp();
            Assert.Null(VP.Push(15));
            DirectionUp();
            Assert.Null(VP.Push(2));
            DirectionDown();
            Assert.Null(VP.Push(13));
            DirectionUp();

            double actual = VP.Push(4).Value;

            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(5, actual);
            }

            MaxValueTest();
            MinValueTest();
            Differences();
        }

        public void MaxValueTest()
        {
            Assert.Equal(15, VP.MaxValue());
        }

        public void MinValueTest()
        {
            Assert.Equal(2, VP.MinValue());
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