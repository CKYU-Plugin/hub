using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.javbus
{
    public class Javbus
    {
        public static bool GetIndex(string search, Action<Dictionary<string,string>> action)
        {
            RestClient client = new RestClient("https://www.javbus.com/");
            RestRequest request = new RestRequest($"search/{search}", Method.GET);
            client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile Safari/602.1";
            request.AddCookie("existmag", "mag");
            var response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK) { return false; }
            var doc = new HtmlDocument();
            doc.Load(response.Content);
            HtmlNode node = doc.DocumentNode.SelectSingleNode("//*[@id='waterfall']/div[1]");
            



        }
    }
}
