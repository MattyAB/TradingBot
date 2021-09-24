using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;

namespace TradingAlgorithm
{
    public class Log
    {
        // TODO : building the log to log to console and a text file
        // TODO : predefined log methods that have values passed in

        public static Plotter logText(int TickNo, string text, Plotter plot)
        {
            plot.addText(TickNo, text);
            Console.WriteLine(TickNo + ": Position " + plot.fileName + " : " + text);
            //File.AppendAllText(Const.logPath, TickNo + ": Position " + plot.fileName + " : " + text + Environment.NewLine);
            return plot;
        }

        public static void logText(int TickNo, string text)
        {
            Console.WriteLine(TickNo + " : " + text);
            File.AppendAllText(Const.logPath, TickNo + " : " + text + Environment.NewLine);
        }

        public static Plotter WriteClosePosition(double startPrice, DataPoint endPoint, bool longOrShort, Plotter plot, int TickNo)
        {
            double diff = (endPoint.close - startPrice);

            if (!longOrShort) // Reverse polarity for a short position
                diff = -diff;

            double percentage = Math.Round((diff / startPrice) * 100);

            string text;
            text = "Closed position at price " + endPoint.close + ": net gain of $" + Math.Round(diff, 2) +
                ", a percentage increase of " + percentage + "%";

            if (percentage > 0)
                plot.fileName += "_WIN";
            else
                plot.fileName += "_LOSS";

            plot = logText(TickNo, text, plot);
            return plot;
        }

        /// <summary>
        /// Displays all the finishing data in a pretty, readable way
        /// </summary>
        /// <param name="startValue">Initial portfolio value</param>
        /// <param name="endValueConst">Finishing portfolio value, calculated with initial price</param>
        /// <param name="endValueFluct">Finishing portfolio value, calculated with finishing price (subject to price fluctuations)</param>
        public static void FinishUp(double startValue, double endValueConst, double endValueFluct, int longWin, int longLoss, int shortWin, int shortLoss, double days)
        {
            /**
            Console.WriteLine("All done! Started with a portfolio value of " + startValue + "...");
            Console.WriteLine("Mesaured with the start price, we finish with a value of " + endValueConst + "! This is a percentage of " + Math.Round(((endValueConst - startValue) / startValue) * 100) + "%");
            Console.WriteLine("But if we measure with the end price, we finish with a value of " + endValueFluct + "! This is a percentage of " + Math.Round(((endValueFluct - startValue) / startValue) * 100) + "%");
            Console.WriteLine("Total of " + longPos + " long positions and " + shortPos + " short pos");
            **/
            if (Const.log)
            {
                Console.WriteLine();
                Console.WriteLine(new String('-', 5) + " VALUE " + new String('-', 5));
                Console.WriteLine("Start value : " + startValue);
                Console.WriteLine(new String('-', 20));
                Console.WriteLine("End value : " + endValueConst + "(" + Math.Round(((endValueConst - startValue) / startValue) * 100) + "%)  from start BTC value - AKA " + Math.Round((((endValueConst - startValue) / startValue) * 100 / days), 2) + "% per day");
                Console.WriteLine(new String('-', 20));
                Console.WriteLine("End value : " + endValueFluct + "(" + Math.Round(((endValueFluct - startValue) / startValue) * 100) + "%)  from final BTC value - AKA " + Math.Round((((endValueFluct - startValue) / startValue) * 100 / days), 2) + "% per day");
                Console.WriteLine();
                Console.WriteLine(new String('-', 5) + " POSITIONS " + new String('-', 5));
                Console.WriteLine("      WIN   LOSS");
                Console.WriteLine("LONG  " + longWin + "   " + longLoss + "    " + (longWin + longLoss));
                Console.WriteLine("SHORT " + shortWin + "   " + shortLoss + "    " + (shortWin + shortLoss));
                Console.WriteLine("      " + (longWin + shortWin) + "  " + (longLoss + shortLoss) + "    " + (shortWin + shortLoss + longWin + longLoss));
            }

        }
    }
}
