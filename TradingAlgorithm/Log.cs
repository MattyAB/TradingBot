using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgorithm
{
    class Log
    {
        // TODO : building the log to log to console and a text file
        // TODO : predefined log methods that have values passed in

        public static Plotter logText(string text, Plotter plot)
        {
            plot.addText(text);
            return plot;
        }

        public static Plotter WriteClosePosition(double startPrice, DataPoint endPoint, bool longOrShort, Plotter plot)
        {
            double diff = (endPoint.close - startPrice);

            string text;
            if (longOrShort)
                text = "Closed position at price " + endPoint.close + ": net gain of $" + Math.Round(diff, 2) +
                       ", a percentage increase of " + Math.Round((diff / startPrice) * 100) + "%";
            else
                text = "Closed position at price " + endPoint.close + ": net gain of $" + -Math.Round(diff, 2) +
                       ", a percentage increase of " + Math.Round((-diff / startPrice) * 100) + "%";
            plot = logText(text, plot);
            return plot;
        }
    }
}
