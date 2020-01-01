using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.Ft12
{
    class Ft12
    {
        public static string GetShort(string url)
        {
            //https://www.ft12.com/do.php?m=index&a=urlCreate
            RestClient client = new RestClient($"https://create.ft12.com/create.php?m=index&a=urlCreate");
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded;");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            request.AddParameter("url", url);
            request.AddParameter("type", "u6");
            var tcs_1 = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, (r) =>
            {
                tcs_1.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs_1.Task.IsCompleted, 20000);
            if (tcs_1.Task.IsCompleted)
            {
                try
                {
                    var d = JsonConvert.DeserializeObject<Ft12Item>(tcs_1.Task.Result.Content, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                    });
                    return $"{d.list}";
                }
                catch { return url; }
            }
            return url;
        }
    }
}
