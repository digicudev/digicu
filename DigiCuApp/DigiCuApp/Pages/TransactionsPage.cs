using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DigicuApp.Pages
{
    class TransactionsPage : ContentPage
    {
        public TransactionsPage(DigicuAppWallet _wallet)
        {
            this._wallet = _wallet;

            this.Title = "Transactions";

            stack = new StackLayout()
            {
                Padding = new Thickness(20),
            };

            AddShowTransactionButton();
            this.Content = new ScrollView()
            {
                Content=stack
            };

        }

        private void AddShowTransactionButton()
        {
            var showTransactionsButton = new Button() { Text = "show transactions" };
            showTransactionsButton.Clicked += (async (o2, e2) =>
            {
                stack.Children.Clear();
                stack.Children.Add(showTransactionsButton);
                stack.Children.Add(new Label { Text = "transactions of " + _wallet.PubKeyColoredString });
                FillStack();
            });

            stack.Children.Clear();
            stack.Children.Add(showTransactionsButton);
        }


        protected override void OnAppearing()
        {
            

//            var showTransactionsButton = new Button() { Text = "show transactions" };
//            showTransactionsButton.Clicked += (async (o2, e2) =>
//            {
//                stack.Children.Clear();
//                stack.Children.Add(showTransactionsButton);
//                stack.Children.Add(new Label { Text = "transactions of " + _wallet.PubKeyColoredString });
//                FillStack();
//            });
//
//            stack.Children.Clear();
//            stack.Children.Add(showTransactionsButton);
//            stack.Children.Add(new Label { Text = "transactions of " + _wallet.PubKeyColoredString });

            base.OnAppearing();
        }

        private void FillStack()
        {
            var list = _wallet.BalanceLineList;
            if (list.Count == 0) stack.Children.Add(new Label { Text = "-no transactions found-" });
            foreach (var line in list)
            {
                stack.Children.Add(new Label { Text = line.ToString() });
            }
        }

        private StackLayout stack;
        private DigicuAppWallet _wallet;

    }
}
