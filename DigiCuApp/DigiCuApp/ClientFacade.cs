using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using RapidBase.Client;
using RapidBase.Models;

namespace DigicuApp
{
    public class ClientFacade
    {
        public Task<BalanceSummary> GetBalanceSummary(BitcoinColoredAddress adr)
        {
            return _client.GetBalanceSummary(adr);
        }

        public Task<BalanceModel> GetBalance(BitcoinColoredAddress adr , bool unspentOnly)
        {
            return _client.GetBalance(adr, unspentOnly);
        }

        public Task<GetTransactionResponse> GetTransaction(uint256 id)
        {
            return _client.GetTransaction(id);
        }

        public ClientFacade()
        {
            _client = CreateClient();
        }

        public void Send(Transaction t)
        {
            _client.Broadcast(t).Wait();
        }



        /// ////////////////////////////////////////////////////////////////////////////

        private RapidBaseClient CreateClient()
        {
            var client = new RapidBaseClient(new Uri("http://rapidbase-test.azurewebsites.net/"), Network.Main);
            client.Colored = true;
            return client;
        }


        private RapidBaseClient _client;
    }
}
