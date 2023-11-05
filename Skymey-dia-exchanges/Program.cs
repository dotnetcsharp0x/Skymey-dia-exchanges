using DnsClient.Protocol;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Nancy.Json;
using RestSharp;
using Skymey_dia_exchanges.Data;
using Skymey_dia_exchanges.Models;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Skymey_dia_exchanges
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            while(true)
            {
                #region DIA
                var client = new RestClient("https://api.diadata.org/v1/exchanges");
                var request = new RestRequest("https://api.diadata.org/v1/exchanges", Method.Get);
                request.AddHeader("Content-Type", "application/json");
                var r = client.Execute(request).Content;
                List<Exchanges> ex = new JavaScriptSerializer().Deserialize<List<Exchanges>>(r);
                #endregion

                MongoClient _mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
                ApplicationContext db = ApplicationContext.Create(_mongoClient.GetDatabase("skymey"));
                foreach (var item in ex)
                {
                    item._id = ObjectId.GenerateNewId();
                    Console.WriteLine(item.Name);
                    var exchange = (from i in db.Exchanges select i).FirstOrDefault();
                    if(exchange == null)
                    {
                        await db.Exchanges.AddAsync(item);
                    }
                    else
                    {
                        exchange.Trades = item.Trades;
                        exchange.Volume24h = item.Volume24h;
                        exchange.Pairs = item.Pairs;
                        db.Exchanges.Update(item);
                    }
                }
                await db.SaveChangesAsync();
                int hours = 24;                
                Thread.Sleep(1000*60*60*hours);
            }
        }
    }
}
