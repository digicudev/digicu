using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DigicuApp.Pages
{
    class SettingsPage : ContentPage
    {

        

        private EntryCell _secretCell;
                public SettingsPage(DigicuAppWallet wallet)
                {
                    this._wallet = wallet;
                    this.Title = AppResources.settingsHeader;
                    this.BindingContext = this._wallet;
                    var pinCell = new EntryCell { Label = "PIN", Placeholder = String.Empty };
                    _secretCell = new EntryCell { Label = "Secret", Placeholder = String.Empty };

                    this.Content = new TableView
                    {
                        Intent = TableIntent.Settings,
                        Root = new TableRoot
                        {
                            new TableSection("Wallet")
                            {
                                _secretCell
        
                            },
        
                            new TableSection("PIN")
                            {
                                pinCell,
                            }
                        }
                    };
                    pinCell.SetBinding(EntryCell.TextProperty, "Pin");
                    _secretCell.SetBinding(EntryCell.TextProperty, "SecretString");
        
                }


        protected override void OnAppearing()
        {
            _secretCell.Text = _wallet.SecretString;
        }


        private DigicuAppWallet _wallet;
        
    }
}





