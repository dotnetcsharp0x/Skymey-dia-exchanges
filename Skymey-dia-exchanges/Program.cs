using Nancy.Json;
using RestSharp;
using Skymey_dia_exchanges.Models;
using System.Text;

namespace Skymey_dia_exchanges
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                var client = new RestClient("https://api.diadata.org/v1/exchanges");
                var request = new RestRequest("https://api.diadata.org/v1/exchanges", Method.Get);
                request.AddHeader("Content-Type", "application/json");
                var r = client.Execute(request).Content;
                var Content = new StringContent(r.ToString(), Encoding.UTF8, "application/json");
                JavaScriptSerializer js = new JavaScriptSerializer();
                Exchange[] ex = js.Deserialize<Exchange[]>(r);
                foreach (Exchange ex2 in ex)
                {
                    Console.WriteLine(ex2.Name + " " + ex2.Blockchain);
                }
                int hours = 24;
                Thread.Sleep(1000*60*60*hours);
            }
        }
    }
}
