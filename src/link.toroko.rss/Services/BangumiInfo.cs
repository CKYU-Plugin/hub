using HtmlAgilityPack;
using link.toroko.rsshub.Services.bilibili.task;
using link.toroko.rsshub.Services.bilibili.web_api;
using link.toroko.rsshub.startup;
using RestSharp;
using Robot.API;
using Robot.Code;
using Robot.Property;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace link.toroko.rsshub.Services
{
    public class BangumiInfo : IServices
    {
        public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> Args = null)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;
            string session_string = new String(msgContent.Where(Char.IsDigit).ToArray());
            long sessionid = 0;
            Int64.TryParse(session_string, out sessionid);
            string session = msgContent.Trim();
            if (msgContent != session_string)
            {
                try
                {
                    var count = Data.bilibili_bangumi.bilibili_bangumi_session.Count();
                    var tmp = Data.bilibili_bangumi.bilibili_bangumi_session.Where(w => w.Value.ToUpper().Contains(session.ToUpper())).FirstOrDefault();
                    if (tmp.Key != 0)
                    {
                        sessionid = tmp.Key;
                    }
                }
                catch { }
            }
            if (sessionid < 1) { _content = "找不到该番剧"; return false; }
            var tcs_0 = new TaskCompletionSource<Services.diygod.Rss>();
            link.toroko.rsshub.Services.diygod.rsshub.api.Bangumi.GetBangumi(sessionid, new Action<Services.diygod.Rss>((rss) =>
            {
                tcs_0.SetResult(rss);
            }));

            SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 10000);
            if (!tcs_0.Task.IsCompleted) { _content = ""; return false; }
            if (tcs_0.Task.Result.Channel.Item.Count() == 0) { _content = ""; return false; }
            Services.diygod.Rss r = tcs_0.Task.Result;

            _content = $"《{r.Channel.Title}》{Environment.NewLine}{r.Channel.Description.Replace(" - Made with love by RSSHub(https://github.com/DIYgod/RSSHub)", "")}{Environment.NewLine}{r.Channel.Item.First().Title}{Environment.NewLine}{r.Channel.Item.First().Description.Substring(0, 24)}";
            if (ispro)
            {
                var doc_0 = new HtmlDocument() { OptionReadEncoding = false };
                var html = HttpUtility.HtmlDecode(r.Channel.Item.First().Description);
                doc_0.LoadHtml(html);
                var imageUrl = doc_0.DocumentNode.Element("img").GetAttributeValue("src", "");
                if (imageUrl != null)
                {
                    if (Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                    {
                        Uri uri = new Uri(imageUrl);
                        var filename = uri.Segments[uri.Segments.Length - 1];
                        RestClient client = new RestClient(uri);
                        RestRequest request = new RestRequest("", Method.GET);
                        client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile Safari/602.1";
                        //    request.AddHeader("Referer", $"https://github.com/DIYgod/RSSHub");
                        var tcs_1 = new TaskCompletionSource<IRestResponse>();
                        client.ExecuteAsync(request, (s) =>
                        {
                            tcs_1.SetResult(s);
                        });
                        SpinWait.SpinUntil(() => tcs_1.Task.IsCompleted, 3000);
                        if (tcs_1.Task.IsCompleted)
                        {
                            if (tcs_1.Task.Result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                using (var fileStream = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image\", RobotBase.PluginName, $"BangumiInfo_List_{filename}"), FileMode.OpenOrCreate))
                                {
                                    fileStream.Write(tcs_1.Task.Result.RawBytes, 0, tcs_1.Task.Result.RawBytes.Length);
                                }
                                Bitmap b = new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image\", RobotBase.PluginName, $"BangumiInfo_List_{filename}"));
                                _content += _API.Api_UploadPic(b.ConvertImage(ImageFormat.Jpeg), out string outpath, $"BangumiInfo_q{qq}");
                                Task.Run(() =>
                                {
                                    Thread.Sleep(1000);
                                    new FileInfo(outpath).Delete();
                                });
                            }
                        }
                    }
                }
            }
            _content += $"{Environment.NewLine}{r.Channel.Item.First().Link}";
            return true;
        }

        public bool Reg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null)
        {
            _content = "";
            return false;
        }

        public bool Start(string qq, string gdid, string msgContent, int messageid, out string _content)
        {
            throw new NotImplementedException();
        }

        public bool Status(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public bool Stop(string qq, string gdid, string msgContent, int messageid, out string _content)
        {
            throw new NotImplementedException();
        }

        public bool UnReg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null)
        {
            _content = "";
            return false;
        }
    }
}
