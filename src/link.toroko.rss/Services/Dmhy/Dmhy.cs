using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace link.toroko.rsshub.Services.Dmhy
{
    public class Dmhy
    {
        public static bool GetList(string search, Action<rss> action)
        {
            var tcs_0 = new TaskCompletionSource<IRestResponse>();
            rss r = null;
            RestClient client = new RestClient("http://share.dmhy.org");
            RestRequest request = new RestRequest($"/topics/rss/rss.xml?keyword={HttpUtility.HtmlEncode(search)}", Method.GET);
            client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile Safari/602.1";
            request.AddHeader("Referer", $"http://share.dmhy.org");

            var response_0 = client.ExecuteAsync(request, (rp) =>
            {
                tcs_0.SetResult(rp);
            });
            SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 10000);
            if (!tcs_0.Task.IsCompleted) { return false; }
            if (tcs_0.Task.Result.StatusCode != System.Net.HttpStatusCode.OK) { return false; }
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(rss));
                using (StringReader reader = new StringReader(tcs_0.Task.Result.Content))
                {
                    r = (rss)(serializer.Deserialize(reader));
                }
            }catch{ }
            if (r == null) { return false; }
            action(r);
            return true;
        }
    }
}
