using System;
using System.Collections.Generic;
using System.Text;

namespace TradingBot
{
    public class Wallet
    {
        private double BTCBalance;
        private double USDTBalance;

        public Wallet(double RawBTC, double RawUSDT) // Needs to also take a list of current positions
        {
            BTCBalance = RawBTC;
            USDTBalance = RawUSDT;
        }

        public double GetUSDTBalance()
        {
            return USDTBalance;
        }

        public double GetBTCBalance()
        {
            return BTCBalance;
        }
    }
}
