using MongoDB.Bson;
using MongoDB.Driver;
using Nancy.Json;
using RestSharp;
using Skymey_dia_exchanges.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Skymey_main_lib;

namespace Skymey_dia_exchanges.Actions.Exchanges
{
    public class GetExchanges : ServiceBase
    {
        private RestClient _client;
        private RestRequest _request;
        private MongoClient _mongoClient;
        private ApplicationContext _db;
        public GetExchanges()
        {
            _client = new RestClient("https://api.diadata.org/v1/exchanges");
            _request = new RestRequest("https://api.diadata.org/v1/exchanges", Method.Get);
            _mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
            _db = ApplicationContext.Create(_mongoClient.GetDatabase("skymey"));
        }

        public void GetExchangesFromDia() {
            try
            {
                #region DIA
                _request.AddHeader("Content-Type", "application/json");
                var r = _client.Execute(_request).Content;
                HashSet<Skymey_main_lib.Models.Exchanges> ex = new JavaScriptSerializer().Deserialize<List<Skymey_main_lib.Models.Exchanges>>(r).ToHashSet();
                #endregion
                foreach (var item in ex)
                {
                    Console.WriteLine(item.Name);
                    var exchange = (from i in _db.Exchanges where i.Name == item.Name select i).FirstOrDefault();

                    if (exchange == null)
                    {
                        item._id = ObjectId.GenerateNewId();
                        item.Update = DateTime.UtcNow;
                        _db.Exchanges.Add(item);
                    }
                    else
                    {
                        exchange.Trades = item.Trades;
                        exchange.Volume24h = item.Volume24h;
                        exchange.Pairs = item.Pairs;
                        exchange.Update = DateTime.UtcNow;
                        _db.Exchanges.Update(exchange);
                    }

                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }

        public void Dispose()
        {
        }
        ~GetExchanges()
        {

        }
    }
}
