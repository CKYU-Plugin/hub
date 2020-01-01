using link.toroko.rsshub.Services;
using link.toroko.rsshub.Services.bilibili.task;
using link.toroko.rsshub.Services.bilibili.web_api;
using link.toroko.rsshub.startup;
using RestSharp;
using Robot.API;
using Robot.Code;
using Robot.Property;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.WinForms;

namespace link.toroko.rsshub.Application
{
    public class BangumiList
    {
        static object filelocker = new object();

        public static void Run(string robotQQ, Int32 msgType, Int32 msgSubType, string msgSrc, string targetActive, string targetPassive, string msgContent, int messageid)
        {
            string content = _Code.Code_At(msgSrc) + Environment.NewLine;

            Image img;
            ConcurrentDictionary<int, Season> forDraw = new ConcurrentDictionary<int, Season>();
            short items = 0;
            short clounm = 7;
            string _html = $@"
<style>
</style>
<table border=1 cellspacing=0 style=""background-color:#ffffff;"">
<thead><tr><th bgcolor=""#FF441D"" colspan=""{clounm}""><h1>新番列表</h1></th></tr></thead>
<tr>";
            if (msgContent=="#新番列表")
            {
                Bangumi.GetTimeline(new Action<Timeline>((rt) =>
                {

                    if (rt.code == 0)
                    {
                        //content += "讀取繪製中…";
                        //_API.SendMessage(msgSrc, msgType, content, targetActive, robotQQ, msgSubType);
                        content = _Code.Code_At(msgSrc) + Environment.NewLine;

                        foreach (var r in rt.result)
                        {
                            for (int i = 0; i < r.seasons.Count(); i++)
                            {
                                foreach (var s in r.seasons)
                                {
                                    Data.bilibili_bangumi.bilibili_bangumi_session.AddOrUpdate(s.season_id, s.title, (key, oldValue) => s.title);
                                    forDraw.AddOrUpdate(s.season_id, s, (key, oldValue) => s);
                                }
                            }
                        }
                        RestClient client = new RestClient();
                        RestRequest request = new RestRequest();

                        foreach (var r in forDraw.OrderBy(o => o.Key))
                        {
                            Uri uri = new Uri(r.Value.cover);
                            var filename = uri.Segments[uri.Segments.Length - 1];

                            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image\", RobotBase.PluginName, filename)))
                            {
                                client = new RestClient(r.Value.cover);
                                request = new RestRequest(Method.GET);
                                client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile Safari/602.1";

                                var tcs_0 = new TaskCompletionSource<IRestResponse>();
                                var response_0 = client.ExecuteAsync(request, (s) =>
                                {
                                    tcs_0.SetResult(s);
                                });

                                SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 3000);
                                if (tcs_0.Task.IsCompleted)
                                {
                                    lock (filelocker)
                                    {
                                        using (var fileStream = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image\", RobotBase.PluginName, filename), FileMode.OpenOrCreate))
                                        {
                                            fileStream.Write(tcs_0.Task.Result.RawBytes,0, tcs_0.Task.Result.RawBytes.Length);
                                        }
                                    }
                                }
                                tcs_0.Task.Dispose();
                            }

                            _html += $@"
<td width=250>
<table border=1 cellspacing=0 width=250 align=center style=""background-color:#ffffff;"">

<tr>
<td width=1 bgcolor=#B4FFB4>{r.Value.season_id}</td>
<td bgcolor=#FBFDFF>{r.Value.title}</td>
</tr>
<tr>
<td colspan=2 bgcolor=#C8FFFF ><span width=245 height=245><img width=245 height=245 src=""{r.Value.cover}""></img></span></td>
</tr>
</table></td>
";
                            //data:image/jpeg;base64,
                            items++;
                            if (items % clounm == 0)
                            {
                                _html += $@"</tr><tr>";
                            }
                            filename = null;
                        }
                        forDraw.Clear();
                        _html += @"</tr></table>";

                        string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image\", RobotBase.PluginName);
                        img = HtmlRender.RenderToImageGdiPlus(_html, 0,0, TextRenderingHint.AntiAlias, null, null, 
                        new EventHandler<TheArtOfDev.HtmlRenderer.Core.Entities.HtmlImageLoadEventArgs>((s,e)=>
                        {
                            if(Uri.IsWellFormedUriString(e.Src, UriKind.Absolute))
                            {
                                Uri uri = new Uri(e.Src);
                                var filename = uri.Segments[uri.Segments.Length - 1];
                                try
                                {
                                        Bitmap b = new Bitmap($"{_path}/{filename}");
                                        e.Callback(b);
                                }
                                catch(Exception ex) { Console.WriteLine(ex.ToString()); e.Callback(e.Src); }
                            }
                        }));

                        content += _API.Api_UploadPic(img.ConvertImage(ImageFormat.Jpeg),out string outpath,$"bangumiList_q{msgSrc}");
                        Task.Run(() =>
                        {
                            Thread.Sleep(1000);
                            new FileInfo(outpath).Delete();
                        });

                        _html = null;
                        img.Dispose();
                        GC.Collect();
                        _API.SendMessage(msgSrc, msgType, content, targetActive, robotQQ, msgSubType);
                        Program.requestNextList.AddOrUpdate(msgSrc, new Program.RequestArgs { Service = new BangumiInfo() },
                            (key, oldValue) => new Program.RequestArgs { Service = new BangumiInfo() });
                    }
                }));
                return;
            }

            if (msgContent=="新番列表")
            {
                Bangumi.GetTimeline(new Action<Timeline>((rt) =>
                {
                    content += $"新番列表";

                    if (rt.code == 0)
                    {
                        foreach (var r in rt.result)
                        {
                            foreach (var s in r.seasons)
                            {
                                Data.bilibili_bangumi.bilibili_bangumi_session.AddOrUpdate(s.season_id, s.title, (key, oldValue) => s.title);
                            }
                        }
                    }
                    foreach (var s in Data.bilibili_bangumi.bilibili_bangumi_session.OrderBy(o => o.Key))
                    {
                        content += $"{Environment.NewLine}{s.Key}    :   {s.Value}";
                    }
                    //    content += $"{Environment.NewLine}輸入'追番 番號'在發現更新時通知你";
                    _API.SendMessage(msgSrc, msgType, content, targetActive, robotQQ, msgSubType);
                    Program.requestNextList.AddOrUpdate(msgSrc, new Program.RequestArgs { Service = new BangumiInfo() },
                        (key, oldValue) => new Program.RequestArgs {  Service= new BangumiInfo() });
                }));
                return;
            }

        }
    }
}
