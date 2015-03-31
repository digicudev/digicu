using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using RapidBase.Client;

using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.OpenAsset;
using Xunit;
using NBitcoin.Protocol;
using Gma.QrCodeNet.Encoding;
using Acr.XamForms.UserDialogs;
using Acr.BarCodes;
using DigicuApp.Pages;
using System.Globalization;
using UsingResxLocalization;
using XLabs.Forms.Controls;
using System.Diagnostics;
using NBitcoin.BouncyCastle.Security;


namespace DigicuApp
{

    
    

    public class App : Application
    {
        public App()
        {
            
            if (Device.OS != TargetPlatform.WinPhone)
            {
                AppResources.Culture = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            }
            
            Key key = (Helpers.Settings.SecretSettings == String.Empty) 
                ? new Key()
                : Key.Parse(Helpers.Settings.SecretSettings, Network.Main);
            _client = new ClientFacade();
            wallet = new DigicuAppWallet(_client,key);

                _balancePage = new BalancePage(wallet);
                _transactionsPage = new TransactionsPage(wallet);
                _settingsPage = new SettingsPage(wallet) {};

            mainTab = new ExtendedTabbedPage()
            {
                Title = "digicu.org",
                Children = { _settingsPage,_balancePage,_transactionsPage},
                CurrentPage=_balancePage,
                SwipeEnabled=true
            };

            //mainTab.CurrentPageChanged += () => Debug.WriteLine("MainPage CurrentPageChanged to {0}", mainTab.CurrentPage.Title);

            MainPage = new NavigationPage(mainTab) { Title = "digicu.org" };

            askPassword = true;
            askUserForPassword();
        }

        private SettingsPage _settingsPage;
        private BalancePage _balancePage;
        private TransactionsPage _transactionsPage;
        private ExtendedTabbedPage mainTab;

        protected override void OnStart()
        {
            askPassword = true;
        }

        protected override void OnSleep()
        {
            askPassword = true;
        }

        protected override void OnResume()
        {
           
            askUserForPassword();
        }

        private bool askPassword;

        private void askUserForPassword()
        {
            if (!askPassword) return;
            if (Helpers.Settings.PinSettings != String.Empty)
            {
                var pinCheckPage = new PinCheckPage(Helpers.Settings.PinSettings);
                MainPage.Navigation.PushModalAsync(pinCheckPage);
            }
            askPassword = false;
        }

        private DigicuAppWallet wallet;
        private ClientFacade _client;


    }
}
