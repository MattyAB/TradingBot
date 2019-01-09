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
            Assert.Null(VP.Push(10));
            Assert.Null(VP.Push(15));
            Assert.Null(VP.Push(2));
            Assert.Null(VP.Push(13));

            double actual = VP.Push(4).Value;

            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(5, actual);
            }

            MaxValueTest();
            MinValueTest();
        }

        public void MaxValueTest()
        {
            Assert.Equal(15, VP.MaxValue());
        }

        public void MinValueTest()
        {
            Assert.Equal(2, VP.MinValue());
        }
    }
}