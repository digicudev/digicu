using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.BarCodes;
using Xamarin.Forms;
using NBitcoin;

namespace DigicuApp.Pages
{
    class SendPage : ContentPage
    {
        private DigicuAppWallet _wallet;

        public SendPage(DigicuAppWallet wallet)
        {
            _wallet = wallet;
            this.Title = "Send";
            StackLayout objStackLayout = new StackLayout();

            var sendLabel = new Label
            {
                XAlign = TextAlignment.Center,
                Text = AppResources.SendPageEnterRecepientLabel,
                FontSize = 15
            };
            objStackLayout.Children.Add(sendLabel);

            var recipientAddressEntry = new Entry {Placeholder = String.Empty};
            objStackLayout.Children.Add(recipientAddressEntry);

            var scanButton = new Button { Text = AppResources.SendPageScanButtonLabel };
            scanButton.Clicked += async (sender, args) =>
            {
                var result = await BarCodes.Instance.Read();
                if (!result.Success)
                {
                    await this.DisplayAlert("Failed", "Failed to read barcode", "OK");
                    recipientAddressEntry.Text = String.Empty;
                }
                else
                {
//                    var msg = String.Format("Barcode: {0} - {1}", result.Format, result.Code);
                    recipientAddressEntry.Text = result.Code;
//                    await this.DisplayAlert("Success", msg, "OK");
                }
            };
            objStackLayout.Children.Add(scanButton);

            var amountLabel = new Label
            {
                XAlign = TextAlignment.Center,
                Text = AppResources.SendPageEnterAmountLabel,
                FontSize = 15
            };
            objStackLayout.Children.Add(amountLabel);

            var amountEntry = new Entry { Placeholder = "0" };
            objStackLayout.Children.Add(amountEntry);

            var sendButton = new Button { Text = "Send" };
            sendButton.Clicked += async (sender, args) =>
            {
                if (_wallet.IsValidAddress(recipientAddressEntry.Text))
                {
                    double d = Double.Parse(amountEntry.Text);
                    Money amount = new Money(new Decimal(d), MoneyUnit.Bit);
                    var resultMsg = String.Format("Sent {0} to {1}",
                    amountEntry.Text, recipientAddressEntry.Text);
                    try
                    {
                        _wallet.SendColoredCoin(recipientAddressEntry.Text, amount, _wallet.AssetIdString);
                    }
                    catch (Exception e)
                    {
                        resultMsg = e.Message;
                    }
                    await this.DisplayAlert("Send Report", resultMsg, "OK");
                } else
                {
                    var failMsg = String.Format("Address {0} is not valid!",recipientAddressEntry.Text);
                    await this.DisplayAlert("Failed", failMsg, "OK");
                }

                await this.Navigation.PopModalAsync();
            };
            objStackLayout.Children.Add(sendButton);

            Button cancelButton = new Button();
            cancelButton.Text = "CANCEL";
            objStackLayout.Children.Add(cancelButton);
            cancelButton.Clicked += ((o2, e2) =>
            {
                this.Navigation.PopModalAsync();
            });


            this.Content = objStackLayout;
        }

        private async Task<StackLayout> CreateSendPage()
        {
            StackLayout objStackLayout = new StackLayout();

            Button cmdButton_CloseModalPage = new Button();
            cmdButton_CloseModalPage.Text = "Close";
            objStackLayout.Children.Add(cmdButton_CloseModalPage);
            cmdButton_CloseModalPage.Clicked += ((o2, e2) =>
            {
                this.Navigation.PopModalAsync();
            });

            var scanButton = new Button { Text = "Scan QR-Code" };
            objStackLayout.Children.Add(cmdButton_CloseModalPage);
            scanButton.Clicked += async (sender, args) =>
            {
                var result = await BarCodes.Instance.Read();
                if (!result.Success)
                    await this.DisplayAlert("Failed", "Failed to get barcode", "OK");
                else
                {
                    var msg = String.Format("Barcode: {0} - {1}", result.Format, result.Code);
                    await this.DisplayAlert("Success", msg, "OK");
                }
            };

            //
            return objStackLayout;
        }

    }




}
