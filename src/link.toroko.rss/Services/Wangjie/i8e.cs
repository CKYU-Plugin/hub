using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.Wangjie
{
    class I8e
    {
        public static string GetShort(string url)
        {
            RestClient client = new RestClient($"https://i8e.net/index.php");
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded;");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            request.AddParameter("target", url);
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
                    var d = JsonConvert.DeserializeObject<I8eItem>(tcs_1.Task.Result.Content, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                    });
                    return $"https://i8e.net/{d.s}";
                }
                catch { return url; }
            }
            return url;
        }
    }
}
