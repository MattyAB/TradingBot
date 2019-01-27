using System;
using System.Collections.Generic;

namespace TradingAlgorithm
{
    public class TradingAlgorithm
    {
        private Indicators indicators;
        private PositionOpener opener;
        private List<Position> positions;

        public TradingAlgorithm(int startTimeStamp, List<PositionOpener.PositionDecision> decisions)
        {
            indicators = new Indicators(startTimeStamp, true);
            opener = new PositionOpener(decisions);
            positions = new List<Position>();
        }

        public List<PositionSignal> Tick(DataPoint Point)
        {
            // signal to be returned. +1 is buy, -1 is sell.
            List<PositionSignal> returns = new List<PositionSignal>();

            Point = indicators.Tick(Point);
            int choice = opener.Tick(Point);
            if (choice != 0)
            {
                // If choice bigger than 0 this is true, else false.
                bool longOrShort = (choice > 0);
                returns.Add(new PositionSignal
                    (longOrShort, Math.Abs(choice), Point, opener.NextPositionID));
            }
            
            // Now tick all positions
            for(int i = 0; i < positions.Count; i++)
            {
                bool signal = positions[i].Tick(Point);
                if (signal)
                {
                    returns.Add(new PositionSignal(positions[i]));
                    positions[i].FinishPlot();
                    positions.RemoveAt(i);
                }
            }

            return returns;
        }

        public void AddPosition(Position p)
        {
            positions.Add(p);
        }

        public void RemovePosition(int id)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                if(positions[i].id == id)
                    positions.RemoveAt(i);
            }
        }

        public void FinishUp()
        {
            indicators.FinishPlots();
        }
    }
}
