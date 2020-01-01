using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.T_im
{
    class T_im
    {
        public static string GetShort(string url)
        {
            RestClient client = new RestClient($"http://t.im/?url={url}&keyword=&title=&expiration=365");
            RestRequest request = new RestRequest(Method.GET);
        //    request.AddHeader("Content-Type", "application/x-www-form-urlencoded;");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            var tcs_1 = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, (r) =>
            {
                tcs_1.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs_1.Task.IsCompleted, 20000);
            if (tcs_1.Task.IsCompleted)
            {
                Regex r = new Regex("<span id=.shortUrl. class=.clipText label label-success.>(.*)<.span>");
               Match m = r.Match(tcs_1.Task.Result.Content);
                if (m.Success&& m.Groups.Count==2)
                {
                    return m.Groups[1].Value;
                }
            }
            return url;
        }
    }
}
