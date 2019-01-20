using System;
using System.Collections.Generic;
using System.Text;

namespace TradingAlgorithm
{
    public class PositionOpener
    {
        // TODO: Maybe chage to double eventually to show confidence ?
        public delegate int PositionDecision(DataPoint Point);

        private List<PositionDecision> decisions;

        public PositionOpener(List<PositionDecision> decisions)
        {
            this.decisions = decisions;
        }

        public int Tick(DataPoint Point)
        {
            int decision = 0;
            
            // Iterates through the delegates and tries them all
            for (int i = 0; i < decisions.Count; i++)
            {
                int currentChoice = decisions[i](Point);
                // Returns the delegate number instead of just -1 or 1
                // So that the Plot of the position can see which delegate was used
                if (currentChoice > 0)
                    return i + 1;
                if (currentChoice < 0)
                    return -i - 1;
            }

            return 0;
        }
    }
}
