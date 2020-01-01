using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.Threading;
using System.IO;
using System.Xml.Serialization;

namespace link.toroko.rsshub.Services.diygod.rsshub.api
{
    public class Bangumi
    {
        public static void GetBangumiAsync(long season_id, Action<Rss> action)
        {
            RestClient client = new RestClient("https://rsshub.app");
            RestRequest request = new RestRequest($"/bilibili/bangumi/{season_id}", Method.GET);
            client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile/14A403 Safari/602.1";
            client.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //var data = JsonConvert.DeserializeObject<RssHub>(response.Content, new JsonSerializerSettings
                    //{
                    //    NullValueHandling = NullValueHandling.Ignore,
                    //    MissingMemberHandling = MissingMemberHandling.Ignore,
                    //});
                    Rss data = new Rss();
                    XmlSerializer serializer = new XmlSerializer(typeof(Rss));
                    using (TextReader reader = new StringReader(response.Content))
                    {
                        data = (Rss)serializer.Deserialize(reader);
                    }

                    action(data);
                }
            });
        }

        public static void GetBangumi(long season_id , Action<Rss> action)
        {
            RestClient client = new RestClient("https://rsshub.app");
            RestRequest request = new RestRequest($"/bilibili/bangumi/{season_id}", Method.GET);
            client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile/14A403 Safari/602.1";

            var tcs = new TaskCompletionSource<Rss>();
            var response = client.ExecuteAsync(request, (r) =>
            {
                if (r.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    try
                    {
                        //var data = JsonConvert.DeserializeObject<RssHub>(r.Content, new JsonSerializerSettings
                        //{
                        //    NullValueHandling = NullValueHandling.Ignore,
                        //    MissingMemberHandling = MissingMemberHandling.Ignore,
                        //});
                        Rss data = new Rss();
                        XmlSerializer serializer = new XmlSerializer(typeof(Rss));
                        using (TextReader reader = new StringReader(r.Content))
                        {
                            data = (Rss)serializer.Deserialize(reader);
                        }

                        tcs.SetResult(data);
                    }
                    catch { tcs.SetResult(new Rss()); }
                }
                else
                {
                    tcs.SetResult(new Rss());
                }
            });
            SpinWait.SpinUntil(() => tcs.Task.IsCompleted, 10000);
            action(tcs.Task.Result);
        }

    }
}
