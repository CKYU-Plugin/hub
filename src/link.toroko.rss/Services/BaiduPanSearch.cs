using link.toroko.rsshub.Services.PanBaidu;
using Robot.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace link.toroko.rsshub.Services
{
    public class BaiduPanSearch : IServices
    {
        public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> args = null)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;
            object pages = "1";
            object keyword = msgContent;

            if (args != null)
            {
                args.TryGetValue("pages", out pages);
                args.TryGetValue("Keyword", out keyword);
            }
            int ipages = 1;
            Int32.TryParse(pages.ToString(), out ipages);
            if (ipages < 1) { ipages = 1; }
            NccklDetail data = link.toroko.rsshub.Services.PanBaidu.Ncckl.GetIndex(keyword.ToString(), ipages);
            if (data == null)
            {
                content += "沒找到任何结果";
            }
            else
            {
                long pageCount = data.body.total > 0 ? (data.body.total / data.body.pageSize) : 1;
                content += $"{data.body.pageCurrent}/{pageCount}{Environment.NewLine}";
                int no = 1 + (data.body.pageCurrent -1 <0 ? 0 : data.body.pageCurrent - 1) * data.body.pageSize;
                foreach (var d in data.body.body)
                {
                    string pw = d.password != "" ? "密码" : "";
                    string title = HttpUtility.HtmlDecode(d.title).Replace(Environment.NewLine, "").Replace("<b>", "").Replace("</b>", "").Trim();
                    string _title = new Regex(@"(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?").
Replace(title, "***");
                    //    content += $"{HttpUtility.HtmlDecode(d.info)}{Environment.NewLine}";
                    content += $"[{no}]{_title}({HttpUtility.HtmlDecode(d.size.Replace("<strong>", "").Replace("</strong>", ""))}){Environment.NewLine}";
                    content += $"{d.url.Replace("https://","")}{pw}{d.password}{Environment.NewLine}";
                    no++;
                }
            }

            Program.requestNextList.AddOrUpdate(qq,
                new Program.RequestArgs { Service = new BaiduPanSearch(), Args = new Dictionary<string, object> { { "pages", (ipages + 1).ToString() },{ "Keyword", keyword } } },
                (key, oldValue) =>
                new Program.RequestArgs { Service = new BaiduPanSearch(), Args = new Dictionary<string, object> { { "pages", (ipages + 1).ToString() }, { "Keyword", keyword } } });

            Program.requestBeforeList.AddOrUpdate(qq,
                new Program.RequestArgs { Service = new BaiduPanSearch(), Args = new Dictionary<string, object> { { "pages", (ipages - 1).ToString() }, { "Keyword", keyword } } },
                (key, oldValue) =>
                new Program.RequestArgs { Service = new BaiduPanSearch(), Args = new Dictionary<string, object> { { "pages", (ipages - 1).ToString() }, { "Keyword", keyword } } });

            _content = content;
            return true;
        }

        public bool Reg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
