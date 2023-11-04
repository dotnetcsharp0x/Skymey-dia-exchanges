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
                var client = new RestClient("https://api.diadata.org/v1/exchanges");
                var request = new RestRequest("https://api.diadata.org/v1/exchanges", Method.Get);
                request.AddHeader("Content-Type", "application/json");
                var r = client.Execute(request).Content;
                List<Exchange> ex = new JavaScriptSerializer().Deserialize<List<Exchange>>(r);
                MongoContext con = new MongoContext("mongodb://127.0.0.1:27017");
                var col = await con.GetCollection("exchanges");

                List<InsertOneModel<BsonDocument>> listWrites = new List<InsertOneModel<BsonDocument>>();
                List<ReplaceOneModel<BsonDocument>> listWrites2 = new List<ReplaceOneModel<BsonDocument>>();
                foreach (var item in ex)
                {
                    var filter = new BsonDocument { { "Name", item.Name } };
                    List<BsonDocument> users = await col.Find(filter).ToListAsync();
                    if (users.Count == 0)
                    {
                        listWrites.Add(new InsertOneModel<BsonDocument>(item.ToBsonDocument()));
                    }
                    else
                    {
                        listWrites2.Add(new ReplaceOneModel<BsonDocument>(new BsonDocument(item.ToBsonDocument()), new BsonDocument(item.ToBsonDocument())));
                    }
                    
                }
                //collection2.BulkWriteAsync(ex);
                if (listWrites2.Count() > 0)
                {
                    await col.BulkWriteAsync(listWrites2);
                }
                if (listWrites.Count() > 0)
                {
                    await col.BulkWriteAsync(listWrites);
                }
                int hours = 24;                
                Thread.Sleep(1000*60*60*hours);
            }
        }
    }
}
