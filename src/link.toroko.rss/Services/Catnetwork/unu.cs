using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.Catnetwork
{
    class Unu
    {
        public static string GetShort(string url)
        {
            RestClient client = new RestClient($"https://u.nu/api.php?action=shorturl&format=simple&url={RestSharp.Extensions.MonoHttp.HttpUtility.HtmlEncode(url)}");
            RestRequest request = new RestRequest(Method.GET);
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            var tcs_1 = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, (r) =>
            {
                tcs_1.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs_1.Task.IsCompleted, 20000);
            if (tcs_1.Task.IsCompleted)
            {
                return tcs_1.Task.Result.Content;
            }
            return url;
        }
    }
}
