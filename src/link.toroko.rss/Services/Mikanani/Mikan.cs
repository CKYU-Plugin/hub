using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace link.toroko.rsshub.Services.Mikanani
{
    public class Mikan
    {
        public static Dictionary<string, string> GetBangumiId(string search)
        {
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            var tcs_0 = new TaskCompletionSource<IRestResponse>();
            rss r = null;
            RestClient client = new RestClient("https://mikanani.me/");
            RestRequest request = new RestRequest($"/Home/Search?searchstr={HttpUtility.HtmlEncode(search)}", Method.GET);
            client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile Safari/602.1";
            request.AddHeader("Referer", $"http://mikanani.me");
            var response_0 = client.ExecuteAsync(request, (rp) =>
            {
                tcs_0.SetResult(rp);
            });
            SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 10000);
            if (!tcs_0.Task.IsCompleted) { return tmp; }
            if (tcs_0.Task.Result.StatusCode != System.Net.HttpStatusCode.OK) { return tmp; }
            var doc_0 = new HtmlDocument() { OptionReadEncoding = false };
            doc_0.LoadHtml(Encoding.UTF8.GetString(tcs_0.Task.Result.RawBytes));
            HtmlNode node_list = doc_0.DocumentNode.SelectSingleNode("//*[contains(@class,'list-inline an-ul')]");
            if (node_list != null)
            {
                foreach(var nodes in node_list.ChildNodes)
                {
                    if (nodes.Name == "li")
                    {
                        try
                        {
                            string url = nodes.Element("a").GetAttributeValue("href", "").Split('/').LastOrDefault();
                            string title = String.Join("", nodes.Element("a").InnerText.Split(' ').ToList().Skip(1));
                            tmp.Add(url, title);
                        }
                        catch { }
                    }
                }
            }
            return tmp;
        }

        public static bool GetList(string search, Action<rss> action)
        {
            var tcs_0 = new TaskCompletionSource<IRestResponse>();
            rss r = null;
            RestClient client = new RestClient("https://mikanani.me/");
            RestRequest request = new RestRequest($"/RSS/Search?searchstr={HttpUtility.HtmlEncode(search)}", Method.GET);
            client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile Safari/602.1";
            request.AddHeader("Referer", $"http://mikanani.me");

            var response_0 = client.ExecuteAsync(request, (rp) =>
            {
                tcs_0.SetResult(rp);
            });
            SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 10000);
            if (!tcs_0.Task.IsCompleted) { return false; }
            if (tcs_0.Task.Result.StatusCode != System.Net.HttpStatusCode.OK) { return false; }
            try
            {
                //DataContractSerializer serializer = new DataContractSerializer(typeof(rss));
                //using (MemoryStream reader = new MemoryStream(tcs_0.Task.Result.RawBytes))
                //{
                //    r = (rss)(serializer.ReadObject(reader));
                //}
                XmlSerializer serializer = new XmlSerializer(typeof(rss));
                using (StringReader reader = new StringReader(tcs_0.Task.Result.Content.Substring(39)))
                {
                    r = (rss)(serializer.Deserialize(reader));
                }
            }
            catch(Exception ex) { Console.WriteLine(ex.ToString()); }
            if (r == null) { return false; }
            action(r);
            return true;
        }
    }
}
