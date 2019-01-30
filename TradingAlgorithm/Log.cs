using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace TradingAlgorithm
{
    public class Log
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

        /// <summary>
        /// Displays all the finishing data in a pretty, readable way
        /// </summary>
        /// <param name="startValue">Initial portfolio value</param>
        /// <param name="endValueConst">Finishing portfolio value, calculated with initial price</param>
        /// <param name="endValueFluct">Finishing portfolio value, calculated with finishing price (subject to price fluctuations)</param>
        public static void FinishUp(double startValue, double endValueConst, double endValueFluct, int longPos, int shortPos)
        {
            Console.WriteLine("All done! Started with a portfolio value of " + startValue + "...");
            Console.WriteLine("Mesaured with the start price, we finish with a value of " + endValueConst + "! This is a percentage of " + Math.Round(((endValueConst - startValue) / startValue) * 100) + "%");
            Console.WriteLine("But if we measure with the end price, we finish with a value of " + endValueFluct + "! This is a percentage of " + Math.Round(((endValueFluct - startValue) / startValue) * 100) + "%");
            Console.WriteLine("Total of " + longPos + " long positions and " + shortPos + " short pos");
        }
    }
}
