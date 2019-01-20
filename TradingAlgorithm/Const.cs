using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgorithm
{
    public class Const
    {
        public const string backtestPath = @"E:\Documents\Code\C#\TradingBot\BacktestData\dataBTCUSDT.json"; // Path for backtest data

        public const double Fee = 0.001; // Binance maker and taker fees are identical

        #region Indicators
        public static readonly int MA1PipeLength = 50; // Moving Average 1 pipe length
        public static readonly int MA2PipeLength = 100; // Moving Average 2 pipe length
        public static readonly int MA3PipeLength = 500; // Moving Average 3 pipe length

        public static readonly int RSIPeriod = 14;

        #endregion
    }
}
