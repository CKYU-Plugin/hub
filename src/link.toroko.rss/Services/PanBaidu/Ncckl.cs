using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.PanBaidu
{
    public class Ncckl
    {
        public static NccklDetail GetIndex(string keyword, int page)
        {
            NccklDetail data = null;
            NccklDetail outData = new NccklDetail();
            RestClient client = new RestClient("http://so.ncckl.cn/");
            RestRequest request = new RestRequest($"/soYunPan/Search", Method.POST);
            client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile Safari/602.1";
            request.AddHeader("Referer", @"http://so.ncckl.cn/");
            request.AddParameter("keyWord", keyword);
            request.AddParameter("pages", page.ToString());
            request.AddQueryParameter("pages", page.ToString());
            request.AddHeader("Content-Type", "multipart/form-data");
            var tcs_0 = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, (r) =>
            {
                tcs_0.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 5000);
            if (!tcs_0.Task.IsCompleted) { return null; }
         //   if (tcs_0.Task.Result.StatusCode != 0) { return null; }

            try
            {
                data = JsonConvert.DeserializeObject<NccklDetail>(tcs_0.Task.Result.Content, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                });
            }
            catch { return null; }

            if (data == null) { return null; }
            if (data.code != 0) { return null; }
            if (data.body.code != 0) { return null; }

            outData.code = data.code;
            outData.body = new Body();
            outData.body.body = new List<Body1>();
            outData.body.code = data.body.code;
            outData.body.pageCurrent = data.body.pageCurrent;
            outData.body.pageSize = data.body.pageSize;
            outData.body.total = data.body.total;

            for (int i = 0; i < data.body.body.Count(); i++)
            {
                var d = data.body.body[i];
             //   if (!Uri.TryCreate(d.url, UriKind.Absolute, out Uri uri)) { continue; }

                client = new RestClient($"http://so.ncckl.cn/api/url?url={d.url}");
                request = new RestRequest( Method.GET);
                client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile Safari/602.1";
                request.AddHeader("Referer", @"http://so.ncckl.cn/");
                var tcs_1 = new TaskCompletionSource<IRestResponse>();
                client.ExecuteAsync(request, (q) =>
                {
                    tcs_1.SetResult(q);
                });
                SpinWait.SpinUntil(() => tcs_1.Task.IsCompleted, 5000);
                if (!tcs_1.Task.IsCompleted) { continue; }
                if (tcs_1.Task.Result.StatusCode != System.Net.HttpStatusCode.OK) { continue; }
                Regex r = new Regex(@"(https.*)\""");
                Match m = r.Match(tcs_1.Task.Result.Content);
                if (!m.Success) { continue; }
                if (m.Groups.Count == 0) { continue; }
                d.url = m.Groups[1].Value;
                outData.body.body.Add(d);
            }

            return outData;
        }
    }
}
