using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace DigicuApp
{
    class BalanceLineList
    {

        public List<BalanceLine> GetBalanceLineList()
        {
                fillTxDic();
                _balanceLineList = _txd.Values.ToList();
                return _balanceLineList;
        }


        public BalanceLineList(ClientFacade client, string unitName, BitcoinColoredAddress pubKeyColored)
        {
            _txd = new Dictionary<uint256, BalanceLine>();
            _balanceLineList = new List<BalanceLine>();
            _client = client;
            _unitName = unitName;
            _pubKeyColored = pubKeyColored;
        }

        private BitcoinColoredAddress _pubKeyColored;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void fillTxDic()
        {
            var balance = _client.GetBalance(_pubKeyColored, false).Result;
            //Money total = new Money(0);
            foreach (var op in balance.Operations)
            {
                var t = _client.GetTransaction(op.TransactionId).Result;
                if (t.Block == null) continue;

                Money txCoins = new Money(0);
                var balanceLine = GetBalanceLine(op.TransactionId);
                foreach (var i in op.ReceivedCoins)
                {
                    if (!(i.GetType() == typeof(ColoredCoin))) continue;
                    var cc = (ColoredCoin)i;
                    var dest = cc.ScriptPubKey.GetDestinationAddress(Network.Main).ToColoredAddress();
                    txCoins += i.Amount;

                    foreach (var input in t.Transaction.Inputs)
                    {

                        var other = input.ScriptSig.GetSigner().GetAddress(Network.Main).ToColoredAddress();
                        if (other != _pubKeyColored)
                        {
                            balanceLine.Other = other;
                            balanceLine.TransactionDateTime = t.Block.BlockHeader.BlockTime.DateTime;
                        }
                    }
                }

                foreach (var s in op.SpentCoins)
                {
                    if (!(s.GetType() == typeof(ColoredCoin))) continue;
                    var cc = (ColoredCoin)s;
                    txCoins -= s.Amount;
                    //var t = _client.GetTransaction(op.TransactionId).Result;
                    foreach (var o in t.Transaction.Outputs)
                    {
                        var x = o.ScriptPubKey.GetDestinationAddress(Network.Main);
                        if (x == null) continue;
                        var other = x.ToColoredAddress();
                        if (other != _pubKeyColored)
                        {
                            balanceLine.Other = other;
                            balanceLine.TransactionDateTime = t.Block.BlockHeader.BlockTime.DateTime;
                        }
                    }
                    var dest = cc.ScriptPubKey.PaymentScript.GetDestinationAddress(Network.Main).ToColoredAddress();
                }
                //total += txCoins;
                balanceLine.ColoredAmount = txCoins;
                if (txCoins.Equals(Money.Zero)) _txd.Remove(balanceLine.TransactionId);
                CalculateBalanceLines();
            }
        }

        private void CalculateBalanceLines()
        {
            var list = _txd.Values.ToList();
            list.Reverse();
            Money total = new Money(0);
            foreach (var line in list)
            {
                total += line.ColoredAmount;
                line.ColoredBalance = total;
            }
        }

        private BalanceLine GetBalanceLine(uint256 transactionId)
        {

            if (_txd.ContainsKey(transactionId)) return _txd[transactionId];
            BalanceLine balanceLine = new BalanceLine() { TransactionId = transactionId };
            _txd[transactionId] = balanceLine;
            balanceLine.UnitName = _unitName;
            return balanceLine;
        }

        private List<BalanceLine> _balanceLineList;

        private Dictionary<uint256, BalanceLine> _txd;

        private ClientFacade _client;

        private string _unitName;

    }
}
