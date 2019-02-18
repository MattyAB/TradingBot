using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace TradingBot
{
    class Realnet
    {
        public Realnet()
        {
            Console.WriteLine("Init realnet");
        }

        public void Tick(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Tick!");
        }
    }
}
