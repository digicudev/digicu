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
using Xunit;

namespace DigicuApp
{
    public class BasicWallet
    {

        public virtual String SecretString
        {
            get
            {
                return _key.GetBitcoinSecret(Network.Main).ToWif();
            }
            set
            {
                _key = Key.Parse(value, Network.Main);
            }
        }

        public String PubKeyString
        {
            get
            {
                return PubKey.ToWif();
                //                return _key.GetWif(Network.Main).GetAddress().ToWif();
            }
        }

        public BitcoinAddress PubKey
        {
            get
            {
                return _key.PubKey.GetAddress(Network.Main);
                //                return _key.GetWif(Network.Main).GetAddress().ToWif();
            }
        }

        public String PubKeyColoredString
        {
            get
            {
                return PubKeyColored.ToWif();
            }
        }

        public BitcoinColoredAddress PubKeyColored
        {
            get
            {
                return _key.GetWif(Network.Main).GetAddress().ToColoredAddress();
            }
        }


        public long GetQuantity()
        {
            //            var colored = new BitcoinColoredAddress(PubKeyColoredString);
            //            var balance = _client.GetBalance(colored).Result;
            var balanceSummary = _client.GetBalanceSummary(PubKeyColored).Result;
            if (balanceSummary.Spendable.Assets.Length == 0) return 0;
            return balanceSummary.Spendable.Assets[0].Quantity;
        }

        public string ColoredAmountString { get { return Math.Round(ColoredAmount.ToUnit(MoneyUnit.Bit), 2).ToString(); } }
        public string UnconfirmedAmountString { get { return Math.Round(UnconfirmedColoredAmount.ToUnit(MoneyUnit.Bit), 2).ToString(); } }

        public Money ColoredAmount
        {
            get
            {
                var balanceSummary = _client.GetBalanceSummary(PubKeyColored).Result;
                if (balanceSummary.Spendable.Assets.Length == 0) return 0;
                return balanceSummary.Spendable.Assets[0].Quantity;
            }
        }

        public string BitcoinAmountString
        {
            get
            {
                return Math.Round(BitcoinAmount.ToUnit(MoneyUnit.BTC), 4).ToString();
            }
        }

        public Money BitcoinAmount
        {
            get
            {
                var balanceSummary = _client.GetBalanceSummary(PubKeyColored).Result;
                if (balanceSummary.Spendable.Amount.Satoshi < 10000) return Money.Zero;
                return balanceSummary.Spendable.Amount;
            }
        }

        public long Transactions
        {
            get
            {
                long satoshis = BitcoinAmount.Satoshi;
                long tx = satoshis / 10000;
                return Math.Max(0, tx - 1);
            }
        }
        public long UnconfirmedTransactions
        {
            get
            {
                long satoshis = UnconfirmedBitcoinAmount.Satoshi;
                long tx = satoshis / 10000;
                return Math.Max(0, tx);
            }
        }

        public string UnconfirmedColoredAmountString
        {
            get
            {
                return Math.Round(UnconfirmedColoredAmount.ToUnit(MoneyUnit.Bit), 2).ToString();
            }
        }

        public Money UnconfirmedColoredAmount
        {
            get
            {
                var balanceSummary = _client.GetBalanceSummary(PubKeyColored).Result;
                if (balanceSummary.UnConfirmed.Assets.Length == 0) return 0;
                return balanceSummary.UnConfirmed.Assets[0].Quantity;
            }
        }

        public string UnconfirmedBitcoinAmountString
        {
            get
            {
                return Math.Round(UnconfirmedBitcoinAmount.ToUnit(MoneyUnit.BTC), 4).ToString();
            }
        }

        public Money UnconfirmedBitcoinAmount
        {
            get
            {
                var balanceSummary = _client.GetBalanceSummary(PubKeyColored).Result;
                return balanceSummary.UnConfirmed.Amount;
            }
        }

        public bool IsValidAddress(string recipientAddressString)
        {
            try
            {
                var coloredReceiver = new BitcoinColoredAddress(recipientAddressString, Network.Main);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public void SendColoredCoin(string coloredReceiverString, Money amount, string assetIdString)
        {
            var coloredReceiver = new BitcoinColoredAddress(coloredReceiverString, Network.Main);
            var txBuilder = createTransactionBuilder(coloredReceiver, amount, assetIdString);
            var tx = txBuilder.BuildTransaction(true);
            _client.Send(tx);
        }


        public BasicWallet(ClientFacade client, Key key)
        {
            _client = client;
            _key = key;
        }



        /// ////////////////////////////////////////////////////////////////////////////////////////////////////

        private TransactionBuilder createTransactionBuilder(BitcoinColoredAddress receiverAddress, Money amount, string assetIdString)
        {
            var colored = PubKeyColored;
            var balance = _client.GetBalance(PubKeyColored, true).Result;
            var assetId = new BitcoinAssetId(assetIdString, Network.Main);
            ulong amountSatoshi = (ulong)amount.Satoshi;
            var asset = new Asset(assetId, amountSatoshi);

            // all coins are spent and change returned. if we require confirmations user needs to wait
            var normalCoins = balance.Operations
                //.Where(op => op.Confirmations >= 1)
                .SelectMany(o => o.ReceivedCoins)
                .OfType<Coin>()
                .ToList();

            var coloredCoins = balance.Operations
                //.Where(op => op.Confirmations >= 1)
                .SelectMany(o => o.ReceivedCoins)
                .OfType<ColoredCoin>()
                .Where(c => c.AssetId.GetWif(Network.Main).ToString() == assetIdString)
                .ToList();

            //var fee = new Money(10600);

            var txBuilder = new TransactionBuilder();
            txBuilder.AddCoins(normalCoins.ToArray())
                .AddCoins(coloredCoins.ToArray())
                //.SendFees(fee)
                .AddKeys(_key)
                .SendAsset(receiverAddress, asset)
                .SetChange(colored);

            var estimatedFees = txBuilder.EstimateFees(txBuilder.BuildTransaction(false));
            txBuilder.SendFees(estimatedFees);

            return txBuilder;
        }

        private void publishTransaction(Transaction tx)
        {
            //            var node = NBitcoin.Protocol.Node.Connect(Network.Main,
            //                Utils.ParseIpEndpoint("23.97.197.36", Network.Main.DefaultPort));
            //                node.VersionHandshake();
            //                node.SendMessage(new InvPayload(InventoryType.MSG_TX, tx.GetHash()));
            //                node.SendMessage(new TxPayload(tx));
            //                System.Threading.Thread.Sleep(500);
        }

        protected Key _key;
        protected ClientFacade _client;

        public string GetBalanceString()
        {
            return "spendableCC=" + ColoredAmountString + ",unconfirmedCC=" + UnconfirmedColoredAmountString+",spendableBTC=" + BitcoinAmountString+",addressCC="+PubKeyColoredString+",addressBTC="+PubKeyString;
        }
    }
}
