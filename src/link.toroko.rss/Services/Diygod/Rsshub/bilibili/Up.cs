using link.toroko.rsshub.Services.diygod;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace link.toroko.rsshub.Services.Diygod.Rsshub
{
    public class Up
    {
        public static async Task GetUpAsync(long user_id, Action<Rss> action)
        {
            RestClient client = new RestClient("https://rsshub.app");
            RestRequest request = new RestRequest($"/bilibili/user/video/{user_id}", Method.GET);
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

        public static bool GetUp(long user_id, Action<Rss> action)
        {
            Task t = GetUpAsync(123,((r)=> { }));

            RestClient client = new RestClient("https://rsshub.app");
            RestRequest request = new RestRequest($"/bilibili/user/video/{user_id}", Method.GET);
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
                    catch { tcs.SetResult(null); }
                }
                else
                {
                    tcs.SetResult(null);
                }
            });
            SpinWait.SpinUntil(() => tcs.Task.IsCompleted, 10000);
            if (!tcs.Task.IsCompleted) { return false; }
            if (tcs.Task.Result == null) { return false; }
            action(tcs.Task.Result);
            return true;
        }
    }
}
