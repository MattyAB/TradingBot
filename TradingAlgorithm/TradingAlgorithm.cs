using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace TradingAlgorithm
{
    public class TradingAlgorithm
    {
        private Indicators indicators;
        public PositionOpener opener;
        private List<Position> positions;
        private List<int> longPosCount = new List<int>();
        private List<int> shortPosCount = new List<int>();

        private int readyForPosition = 0; // 1 or -1 when position signal given, then waits for inversion.

        public TradingAlgorithm(int startTimeStamp, List<PositionOpener.PositionDecision> decisions)
        {
            // Clear exports directory
            DirectoryInfo di = new DirectoryInfo(Const.exportPath);
            if(Const.log)
                foreach (FileInfo file in di.GetFiles())
                    file.Delete();

            indicators = new Indicators(startTimeStamp, true);
            opener = new PositionOpener(decisions);
            positions = new List<Position>();
        }

        public List<PositionSignal> Tick(DataPoint Point, double portfolioValue)
        {
            // signal to be returned. +1 is buy, -1 is sell.
            List<PositionSignal> returns = new List<PositionSignal>();

            Point = indicators.Tick(Point, portfolioValue);
            int choice = opener.Tick(Point);
            if (choice != 0)
            {
                readyForPosition = choice;
            }

            // Inverts the correct way
            //if (Point.inversion != 0 && Point.inversion == readyForPosition)
            if (readyForPosition != 0)
            {
                    // If choice bigger than 0 this is true, else false.
                    bool longOrShort = (choice > 0);
                returns.Add(new PositionSignal
                    (longOrShort, Math.Abs(choice), Point, opener.NextPositionID));

                readyForPosition = 0;
            }
            
            // Now tick all positions
            for(int i = 0; i < positions.Count; i++)
            {
                int signal = positions[i].Tick(Point);
                if (signal == -1) // SELL
                {
                    returns.Add(new PositionSignal(positions[i], positions[i].WinOrLoss()));
                    if(Const.log)
                        positions[i].FinishPlot();
                    positions.RemoveAt(i);
                }
            }

            int longs = 0;
            int shorts = 0;
            foreach (Position p in positions)
            {
                if (p.longOrShort)
                    longs++;
                else
                    shorts++;
            }

            longPosCount.Add(longs);
            shortPosCount.Add(shorts);

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

        public AlgorithmData FinishUp()
        {
            // Indicator Plots
            indicators.FinishPlots();

            // Data creation for final plot.
            AlgorithmData data = new AlgorithmData();
            data.points = indicators.points;
            data.longPos = longPosCount.Select<int, double>(i => i).ToList(); // Convert List<int> to List<double>
            data.shortPos = shortPosCount.Select<int, double>(i => i).ToList(); // Convert List<int> to List<double>
            return data;
        }

        public void TruncatePositions()
        {
            positions.Clear();
        }
    }

    public struct AlgorithmData
    {
        public List<DataPoint> points;
        public List<double> longPos;
        public List<double> shortPos;
    }
}
