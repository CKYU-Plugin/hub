using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using link.toroko.rsshub.Services.bilibili.web_api;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace link.toroko.rsshub.Services.bilibili.task
{
    public static class Bangumi
    {
        public static void GetTimeline(Action<Timeline> action)
        {
            RestClient client = new RestClient("https://bangumi.bilibili.com/");
            RestRequest request = new RestRequest("/web_api/timeline_global", Method.GET);
            client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile/14A403 Safari/602.1";

            client.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = JsonConvert.DeserializeObject<Timeline>(response.Content, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        
                    });

                    action(data);
                }
            });


            JObject.Parse("abc", new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Error = (sender, error) =>
                {

                }
            });
        }

        private static object locker = new object();
        private static History h = null;

        public static void Get(Action<History> action, long page = 1)
        {
            lock (locker)
            {
                h = null;
                Get2(action, page);
            }
        }

        private static void Get2(Action<History> action, long page = 1)
        {
            RestClient client = new RestClient("https://bangumi.bilibili.com/");
            RestRequest request = new RestRequest($"/media/web_api/search/result?season_version=-1&area=-1&is_finish=0&copyright=-1&season_status=-1&season_month=-1&pub_date=-1&page={page}&season_type=1&pagesize=30", Method.GET);
            long _page = page;

            var tcs_0 = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, (r) =>
            {
                tcs_0.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 5000);
            if (!tcs_0.Task.IsCompleted) { return; }

            if (tcs_0.Task.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = JsonConvert.DeserializeObject<History>(tcs_0.Task.Result.Content, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                });

                if (h == null)
                {
                    h = data;
                }
                else
                {
                    if (data.code == 0)
                    {
                        data.result.data.AddRange(h.result.data);
                        h = data;
                    }
                }

                if (data.result.page.num * data.result.page.size < data.result.page.total)
                {
                    _page += 1;
                    Get2(action, _page);
                }
                else
                {
                    action(h);

                }
            }
            else
            {
                action(h);
            }
        }
    }
}
