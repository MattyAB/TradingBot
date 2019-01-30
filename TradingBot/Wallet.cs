using System;
using System.Collections.Generic;
using System.Text;

namespace TradingBot
{
    public class Wallet
    {
        public double BTCBalance;
        public double USDTBalance;

        public Wallet(double RawBTC, double RawUSDT) // Needs to also take a list of current positions
        {
            BTCBalance = RawBTC;
            USDTBalance = RawUSDT;
        }

        public double GetTotalBalance(double Price)
        {
            return USDTBalance + Price * BTCBalance;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
