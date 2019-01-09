﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgorithm
{
    public class DataPoint
    {
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
    }
}