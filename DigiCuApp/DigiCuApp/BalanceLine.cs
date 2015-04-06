using NBitcoin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NBitcoin.BouncyCastle.Security;
using NBitcoin.OpenAsset;
using NBitcoin.Protocol;
using RapidBase.Client;


namespace DigicuApp
{
    public class BalanceLine
    {

        public String UnitName { get; set; }

        public uint256 TransactionId;
        public DateTime TransactionDateTime { get; set; }
        public String TransactionDateTimeString { get { return TransactionDateTime.ToString("yyyy-MM-dd HH:mm:ss"); } }
        public BitcoinColoredAddress Other { get; set; }
        public string OtherString { get { return Other.ToWif(); } }

        public Money ColoredAmount { get; set; }
        public Money ColoredBalance { get; set; }

        public string ColoredAmountString { get { return Format(ColoredAmount); } }
        public string ColoredBalanceString { get { return Format(ColoredBalance); } }

        public override String ToString()
        {
            return TransactionDateTimeString + " " + ColoredAmountString
                + (ColoredAmount < 0 ? " sent to " : " received from ") + Other
                + " new balance: " + ColoredBalanceString;
        }

        private String Format(Money m)
        {
            return Math.Round(m.ToUnit(MoneyUnit.Bit), 2).ToString() + " " + UnitName;
        }

    }
}
