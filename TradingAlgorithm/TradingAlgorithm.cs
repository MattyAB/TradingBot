using System;
using System.Collections.Generic;

namespace TradingAlgorithm
{
    public class TradingAlgorithm
    {
        private Indicators indicators;
        private List<string[]> exports;
        private PositionOpener opener;
        private List<Position> positions;

        public TradingAlgorithm(int startTimeStamp, List<PositionOpener.PositionDecision> decisions)
        {
            indicators = new Indicators(startTimeStamp, true);
            exports = new List<string[]>();
            opener = new PositionOpener(decisions);
            positions = new List<Position>();
        }

        public int Tick(DataPoint Point)
        {
            // signal to be returned. +1 is buy, -1 is sell.
            int returnSignal = 0;

            Point = indicators.Tick(Point);
            exports.Add(Point.Export());
            int choice = opener.Tick(Point);
            if (choice != 0)
            {
                // If choice bigger than 0 this is true, else false.
                bool longOrShort = (choice > 0);
                positions.Add(OpenPosition(longOrShort, Math.Abs(choice), Point));
                if (longOrShort)
                    returnSignal++;
                else
                    returnSignal--;
            }

            // Prepare a list of position IDs to end - unused at the moment
            List<int> EndIDs = new List<int>();
            // Now tick all positions
            for(int i = 0; i < positions.Count; i++)
            {
                bool signal = positions[i].Tick(Point);
                if (signal)
                {
                    EndIDs.Add(positions[i].id);
                    if (positions[i].longOrShort)
                        returnSignal--;
                    else
                        returnSignal++;
                    positions[i].FinishPlot();
                    positions.RemoveAt(i);
                }
            }

            return returnSignal;
        }

        private Position OpenPosition(bool longOrShort, int opener, DataPoint Point)
        {
            // Will also include TP and SL prices
            return new Position(longOrShort, opener, Point, this.opener.NextPositionID);
        }

        public List<string[]> Export()
        {
            return exports;
        }

        public void FinishUp()
        {
            indicators.FinishPlots();
        }
    }
}
