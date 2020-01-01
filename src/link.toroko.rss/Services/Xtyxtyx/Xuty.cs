using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wpf.Data;

namespace link.toroko.rsshub.Services.Xtyxtyx
{
    class Xuty
    {
        public static string GetShort(string url)
        {
            RestClient client = new RestClient($"https://xuty.tk/submit");
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded;");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            request.AddParameter("destination", url);
            request.AddParameter("email", ViewModelData.g.Xuty_email);
            request.AddParameter("tag", "sucai_download_api");
            var tcs_1 = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, (r) =>
            {
                tcs_1.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs_1.Task.IsCompleted, 20000);
            if (tcs_1.Task.IsCompleted)
            {
                HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(tcs_1.Task.Result.Content);
                List<string> tmp = htmlDoc.DocumentNode.SelectNodes("/html/body/section/div/div/div/a")?.Select(s => s.InnerText)?.ToList();
                return tmp?.LastOrDefault();
            }
            return url;
        }
    }
}
