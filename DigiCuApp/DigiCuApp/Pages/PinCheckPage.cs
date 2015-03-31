using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DigicuApp.Pages
{

    interface IPasswordProtected
    {
        void OnPassedPasswordCheck();
        void OnFailedPasswordCheck();
    }
    class PinCheckPage : ContentPage
    {

        public string EnteredPin { get; set; }

        private IPasswordProtected callback;
        private string password;

//        public PinCheckPage(IPasswordProtected _callback, string _password)
        public PinCheckPage(string _password)
        {
//            callback = _callback;
            password = _password;
            this.Title = "PIN check";
            EnteredPin = String.Empty;
            StackLayout objStackLayout = new StackLayout() { VerticalOptions = LayoutOptions.Start };

            var pinCheckLabel = new Label
            {
                XAlign = TextAlignment.Center,
                Text = AppResources.PinCheckPageEnterPinLabel,
                FontSize = 30
            };
            objStackLayout.Children.Add(pinCheckLabel);

            var pinEntry = new Entry {Placeholder = String.Empty};
            objStackLayout.Children.Add(pinEntry);


            var okButton = new Button { Text = "OK" };
            okButton.Clicked += async (sender, args) =>
            {
                EnteredPin = pinEntry.Text;
//                var msg = String.Format("Sending {0} to {1}", 
//                    amountEntry.Text, recipientAddressEntry.Text);
//                await this.DisplayAlert("Success", msg, "OK");
                bool pinCorrect = EnteredPin == password;
                if (pinCorrect)
                {
                    await this.Navigation.PopModalAsync();
//                    callback.OnPassedPasswordCheck();
                }
                else
                {
                    await this.DisplayAlert("Failed", "Wrong password. Try again. Good luck!", "OK");
//                    callback.OnFailedPasswordCheck();
                }
            };
            objStackLayout.Children.Add(okButton);

            this.Content = objStackLayout;
            
        }


    }
}
