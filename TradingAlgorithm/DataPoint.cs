using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgorithm
{
    public class DataPoint
    {
        #region Candlestick

        public double open;
        public double high;
        public double low;
        public double close;
        public double volume;
        public double quoteVolume;
        public long tradeCount;
        public double buyBase;
        public double buyQuote;
        public DateTime openTime;
        public DateTime closeTime;

        #endregion

        #region Indicators

        public double? MA1;
        public double? MA2;
        public double? MA3;

        #endregion

        public string[] Export()
        {
            return new string[]
            {
                Convert.ToString(open),
                Convert.ToString(high),
                Convert.ToString(low),
                Convert.ToString(close),
                Convert.ToString(volume),
                Convert.ToString(tradeCount),
                Convert.ToString(openTime.ToFileTimeUtc()),
                Convert.ToString(MA1),
                Convert.ToString(MA2),
                Convert.ToString(MA3)
            };
        }
    }
}
