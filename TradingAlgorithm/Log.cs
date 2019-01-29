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
    }
}
