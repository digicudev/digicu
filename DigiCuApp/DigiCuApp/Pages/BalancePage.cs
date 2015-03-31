using System;
using Xamarin.Forms;
using NBitcoin;

namespace DigicuApp.Pages
{
    class BalancePage : ContentPage
    {
        private Image _webImage;
        private Button _sendButton;
        private Button _moreTransactionsButton;
        public BalancePage(DigicuAppWallet wallet)
        {
            _wallet = wallet;
            
            createImage();
            this.Title = AppResources.balanceHeader;
            //this.BindingContext = this._wallet;

            _sendButton = new Button { Text = "Send" };
            _sendButton.Clicked += (async (o2, e2) =>
            {
                var sendPage = new SendPage(wallet);
                await Navigation.PushModalAsync(sendPage);
            });

            _moreTransactionsButton = new Button { Text = "exchange 1 "+_wallet.UnitName+" for transactions" };
            _moreTransactionsButton.Clicked += (async (o2, e2) =>
            {
                if(_wallet.ColoredAmount < 100) {
                    await this.DisplayAlert("Transaction Request", "Rejected. You do not have enough "+_wallet.UnitName+" available.", "OK");
                    return;
                }
                string resultMsg = "Request for transactions sent. They should be available in 5 to 30 minutes. Refresh to see them in balance.";
                try
                {
                    _wallet.SendColoredCoin(_wallet.TransactionExchangeAddress, new Money(new Decimal(1.0), MoneyUnit.Bit), _wallet.AssetIdString);
                }
                catch (Exception e)
                {
                    resultMsg = e.Message;
                }
                await this.DisplayAlert("Transaction Request", resultMsg, "OK");
            });


            CreateNewStackSafe();
        }

        private void CreateNewStackSafe()
        {
            try
            {
                CreateNewStack();
            }
            catch (Exception e)
            {
                _stack = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                };
                _stack.Children.Add(new Label
                {
                    XAlign = TextAlignment.Center,
                    YAlign = TextAlignment.Start,
                    Text = "please check your internet connection",
                    FontSize = 30
                });
                var retryButton = new Button() { Text = "Retry" };
                retryButton.Clicked += (async (o2, e2) =>
                {
                    CreateNewStackSafe(); 
                });
                _stack.Children.Add(retryButton);
                this.Content = _stack;
            }
        }

        public void CreateNewStack()
        {
            _stack = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
            };
            populateStack();
            this.Content = _stack;
        }

        private StackLayout _stack;

        private void populateStack()
        {
            _stack.Children.Clear();
            _stack.Children.Add(_webImage);
            var pubKeyLabel = new Entry
            {
                //XAlign = TextAlignment.Center,
                Text = _wallet.PubKeyColoredString,
                HorizontalOptions = LayoutOptions.Center
                
                //FontSize = 16
            };
            
            
            //pubKeyLabel.SetBinding(Label.TextProperty, "PubKeyColored");

            _stack.Children.Add(pubKeyLabel);
            _stack.Children.Add(new Label
            {
                XAlign = TextAlignment.Center,
                YAlign = TextAlignment.Start,
                Text = "balance: " + _wallet.ColoredAmountString + " " + _wallet.UnitName,
                FontSize = 20
            });

            //if (_wallet.UnconfirmedColoredAmount != NBitcoin.Money.Zero) 
            //    _stack.Children.Add(new Label
            //    {
            //        XAlign = TextAlignment.Center,
            //        YAlign = TextAlignment.Start,
            //        Text = "unconfirmed: " + _wallet.UnconfirmedAmountString + " " + _wallet.UnitName,
            //        FontSize = 20
            //    });

            if (_wallet.Transactions<100)
            _stack.Children.Add(new Label
            {
                XAlign = TextAlignment.Center,
                YAlign = TextAlignment.Start,
                Text = "transactions left: " + _wallet.Transactions,
                //+ (_wallet.UnconfirmedTransactions > 0 ? " (+" + _wallet.UnconfirmedTransactions + ")" : ""),
                FontSize = 20
            });

            _stack.Children.Add(_sendButton);

            if (_wallet.Transactions < 100)
                _stack.Children.Add(_moreTransactionsButton);

            var refreshButton = new Button() { Text = "Refresh" };
            refreshButton.Clicked += (async (o2, e2) =>
            {
                CreateNewStackSafe(); 
            });
            _stack.Children.Add(refreshButton);
            this.Content = _stack;
        }

        

        private void createImage()
        {
            _webImage = new Image {Aspect = Aspect.AspectFit};
            var request = "https://api.qrserver.com/v1/create-qr-code/?data=" + _wallet.PubKeyColoredString + "&amp;size=200x200";
            _webImage.Source = ImageSource.FromUri(new Uri(request));
            _webImage.HeightRequest = 200;
            _webImage.WidthRequest = 200;
        }

        private DigicuAppWallet _wallet;
        
    }
}
