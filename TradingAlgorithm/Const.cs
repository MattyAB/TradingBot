using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgorithm
{
    public class Const
    {
        public const string apiKey = @"..\..\..\..\API.txt";
        public const string logPath = @"..\..\..\..\TradingBot\TradingBot\log.txt";
        public const string backtestPath = @"..\..\..\..\BacktestData\2019x6month.json"; // Path for backtest data
        public const string exportPath = @"../../../../Exports/"; // Export path
        public const string trainingValuesPath = @"..\..\..\..\TradingBot\Testing\values.csv";
        public static int Points = 700000; // How many data points to test on
        public static bool log = true;

        public static double TradeValue = 0.4; // Trades made are $100
        public static double Fee = 0.001; // Binance maker and taker fees are identical
        public const int PortfolioStartValue = 2000; // Start value portfolio is this much USD plus this much in BTC
        public static double PortfolioStartRatio = 0.952895; // Ratio of BTC to USD in the start portfolio.
        
        public static int plotStartPoint = 1000;
        public static int plotFinishPoint = 10000;
        public static DateTime plotStart;
        public static DateTime plotFinish;

        #region Position

        public static int RequiredPositionRestMins = 30; // Minimum minutes between position opening
        public static double TPPercentage = 0.1512368; // Fractional start value for take profit
        public static double SLPercentage = 0.0386197; // Fractional start value for stop loss
        public static bool ClampTPSL = true; // False if we want to keep the TPSL values constant.
        public static double TPClampValue = 0.0002029419; // Percentage to clamp per tick.
        public static double SLClampValue = 0.0001916039; // Percentage to clamp per tick.
        public static double LongBreakevenMultiplier = 0.29759; // Starting value - no idea how good this will be
        public static double ShortBreakevenMultiplier = 0.3307884; // Starting value - no idea how good this will be

        public static double longMACrossThreshold = -1;
        public static double shortMACrossThreshold = 1;

        #endregion

        #region Indicators

        public static readonly int MA1PipeLength = 50; // Moving Average 1 pipe length
        public static readonly int MA2PipeLength = 100; // Moving Average 2 pipe length
        public static readonly int MA3PipeLength = 500; // Moving Average 3 pipe length

        public static int RSIPeriod = 14;
        public static int RSILow = 4;
        public static int RSIHigh = 86;

        #endregion
    }
}
