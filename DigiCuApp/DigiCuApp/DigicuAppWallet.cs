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

    public class DigicuAppWallet :  BasicWallet, INotifyPropertyChanged
    {

        public static string ASSET_ID_TTOK_STRING = "ANLocLjURwjUHvs7FRGFeCe3qQfjPfVQY3"; // test tokens for developers

        public string TransactionExchangeAddress { get {return _transactionExchangeAddress;} }
        

        public string UnitName { get; set; }
       
        public List<BalanceLine> BalanceLineList
        {
            get
            {
                BalanceLineList balance = new BalanceLineList(_client, UnitName, PubKeyColored);
                return balance.GetBalanceLineList();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public override String SecretString
        {
            get
            {
                return Helpers.Settings.SecretSettings;
            }
            set
            {
                if (value.Length < 3) return;
                var newSecret = value=="new" ? new Key().GetBitcoinSecret(Network.Main).ToWif() : value;
                try
                {
                    var newKey = Key.Parse(newSecret);
                }
                catch (Exception e)
                {
                    return; 
                }

                Helpers.Settings.SecretSettings = newSecret;
                _key = Key.Parse(newSecret, Network.Main);
                OnPropertyChanged();
            }
        }

        
        public String Pin
        {
            get { return Helpers.Settings.PinSettings; } 
            set
            {
                Helpers.Settings.PinSettings = value;
                _pin = value; 
                OnPropertyChanged(); 
            } 
        }
        
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

       
        public DigicuAppWallet(ClientFacade client, Key key) : base(client, key)
        {
            UnitName = UNIT_NAME_TTOK;
            _transactionExchangeAddress = "akKKZLkLNq3HTijz31HarEtNwNn8VfrHCJi";
            SecretString = key.GetBitcoinSecret(Network.Main).PrivateKey.GetWif(Network.Main).ToString();
            
        }

        public string AssetIdString {
            get { return ASSET_ID_TTOK_STRING; }
        }

        /// /////////////////////////////////////////////////////////////////////////////////////////////////

        
        private static string UNIT_NAME_TTOK = "ttok";

        
        //private static string UNIT_NAME = "d-EUR";


        private String _pin;
        private string _transactionExchangeAddress;

        

        
    }
}
