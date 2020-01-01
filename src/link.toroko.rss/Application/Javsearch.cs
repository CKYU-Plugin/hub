using Robot.API;
using Robot.Code;
using Robot.Property;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.WinForms;
using Wpf.Data;

namespace link.toroko.rsshub.Application
{
    public class Javsearch
    {
        public static void Run(string robotQQ, Int32 msgType, Int32 msgSubType, string msgSrc, string targetActive, string targetPassive, string msgContent, int messageid)
        {
            string content = _Code.Code_At(msgSrc) + Environment.NewLine;

            if (ViewModelData.g.BEnable_R18 && msgContent.Contains("/搜番號"))
            {
                if (!Services.Jav.Jav.GetIndex(msgContent.Replace("/搜番號", "").Trim(), (s, m, a) =>
                {
                    long count = s.Count;
                    //   content += $"共找到{m}個結果。{Environment.NewLine}";

                    for (int i = 0; i < (count > 6 ? 6 : count); i++)
                    {
                        content += $"{s.ElementAt(i).Key}\t{s.ElementAt(i).Value.Title}{Environment.NewLine}";
                    };

                }))
                {
                    content += "沒找到任何番號";
                }
                _API.SendMessage(msgSrc, msgType, content, targetActive, robotQQ, msgSubType);
                return;
            }

            if (ViewModelData.g.BEnable_R18 && msgContent.Contains("#搜番號"))
            {
                if (!Services.Jav.Jav.GetIndex(msgContent.Replace("#搜番號", "").Trim(), (s, m, a) =>
                {
                    long count = s.Count;
                    Image img;
                    string _html = @"
<style>.boxix {text-align: left;font-family: '喵想源黑小豬體';text-shadow: 1px 1px 2px black, 0 0 25px blue, 0 0 5px darkblue;}</style>
<table border=""1"" cellpadding=""3"" cellspacing=""0"" width=""600px"" style=""background-color:#aea;"">";

                    for (int i = 0; i < (count > 6 ? 6 : count); i++)
                    {
                        _html += $@"
<tr>
<td width=""30"" rowspan=""2""><img width=""40"" height=""40"" src=""{ s.ElementAt(i).Value.Image}""/></td>
<td width=100% bgcolor=""#fcf8e0"">{s.ElementAt(i).Value.No} ({s.ElementAt(i).Value.Date.ToString("yyyy-MM-dd")})</td>
</tr>
<tr>
<td width=""100%"" bgcolor=""#FFFFFF""><span><font size=""3"" class=""boxix"">{s.ElementAt(i).Value.Title}</font></span></td>
</tr>
";
                    }
                    _html += "</table>";
                    TextRenderingHint textRenderingHint = TextRenderingHint.AntiAlias;
                    img = HtmlRender.RenderToImageGdiPlus(_html, 0, 0, textRenderingHint, null, null, null);

                    content += _API.Api_UploadPic(img.ConvertImage( ImageFormat.Jpeg), out string outpath, $"jav_q{msgSrc}");
                    Task.Run(() =>
                    {
                        Thread.Sleep(1000);
                        new FileInfo(outpath).Delete();
                    });
                }))
                {
                    content += "沒找到任何番號";
                }
                _API.SendMessage(msgSrc, msgType, content, targetActive, robotQQ, msgSubType);
                return;
            }

            if (ViewModelData.g.BEnable_R18 && msgContent.Contains("#搜磁力"))
            {
                if (!Services.Jav.Jav.GetMarget(msgContent.Replace("#搜磁力", "").Trim(), (s) =>
                {
                    if (s.Count == 0)
                    {
                        content += "沒找到任何磁力";
                    }
                    else
                    {
                        Image img;
                        long count = s.Count;
                        string hash = Environment.NewLine;
                        string _html = $@"
<style>.boxix {{text - align: left;text-shadow: 1px 1px 2px black, 0 0 25px blue, 0 0 5px darkblue;}}</style>
<table border=""1"" cellpadding=""3"" cellspacing=""0"" width=""600px"" style=""background-color:#aea;"">
<thead><tr><th bgcolor=""#FF441D"" colspan=""3"">{s.First()?.No}</th></tr></thead>
<tbody>
";
                        for (int i = 0; i < (count > 6 ? 6 : count); i++)
                        {
                            _html += $@"
<tr>
<td bgcolor=""#FBFDFF"">No.{i + 1}</td>
<td bgcolor=""#95E8AE"" colspan=""2"" class=""boxix"">{s.ElementAt(i).Title}</td>
</tr>
<tr>
<td bgcolor=""#fdfe50"">{s.ElementAt(i).Size}</td>
<td bgcolor=""#f6ecff"">{s.ElementAt(i).Update.ToString("yyyy-MM-dd")}</td>
<td bgcolor=""#FFFEE5"">{s.ElementAt(i).Hash}</td>
</tr>
";
                            hash += $"No.{i + 1}\t{s.ElementAt(i).Hash}{Environment.NewLine}";
                        }

                        _html += "</tbody></table>";
                        TextRenderingHint textRenderingHint = TextRenderingHint.AntiAlias;
                        img = HtmlRender.RenderToImageGdiPlus(_html, 0, 0, textRenderingHint, null, null, null);
                        content += _API.Api_UploadPic(img.ConvertImage(ImageFormat.Jpeg), out string outpath, $"jav_q{msgSrc}");
                        Task.Run(() =>
                        {
                            Thread.Sleep(1000);
                            new FileInfo(outpath).Delete();
                        });
                        content += hash;
                    }
                }))
                {
                    content += "沒找到任何磁力";
                }
                _API.SendMessage(msgSrc, msgType, content, targetActive, robotQQ, msgSubType);
                return;
            }

            if (ViewModelData.g.BEnable_R18 && msgContent.Contains("/搜磁力"))
            {
                if (!Services.Jav.Jav.GetMarget(msgContent.Replace("/搜磁力", "").Trim(), (s) =>
                {
                    if (s.Count == 0)
                    {
                        content += "沒找到任何磁力";
                    }
                    else
                    {
                        long count = s.Count;
                        for (int i = 0; i < (count > 6 ? 6 : count); i++)
                        {
                            var a = new Regex(@"(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?").
                   Replace(s[i].Title, "***");
                            content += $"{a}{Environment.NewLine}{s[i].Update.ToShortDateString()}\t{s[i].Size}{Environment.NewLine}{s[i].Hash}{Environment.NewLine}";
                        }
                    }
                }))
                {
                    content += "沒找到任何磁力";
                }
                _API.SendMessage(msgSrc, msgType, content, targetActive, robotQQ, msgSubType);
                return;
            }
        }
    }
}
