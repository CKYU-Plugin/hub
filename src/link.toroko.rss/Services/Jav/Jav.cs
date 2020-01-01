using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace link.toroko.rsshub.Services.Jav
{
    public class Jav
    {
        public static bool GetMarget(string no, Action<List<JavDetail>> action)
        {
            List<JavDetail> d = new List<JavDetail>();

            if (!no.Contains("-"))
            {
                string ban = new String(no.Where(Char.IsDigit).ToArray());
                string cha = new String(no.Where(Char.IsLetter).ToArray());
                no = $"{cha}-{ban}".ToUpper();
            }

            RestClient client = new RestClient("https://www.javbus.com/");
            RestRequest request = new RestRequest($"{no.ToUpper()}", Method.GET);
            client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile Safari/602.1";
            var tcs_0 = new TaskCompletionSource<IRestResponse>();
            var response_0 = client.ExecuteAsync(request, (r) =>
            {
                tcs_0.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 10000);
            if (!tcs_0.Task.IsCompleted) { return false; }
            if (tcs_0.Task.Result.StatusCode != System.Net.HttpStatusCode.OK) { return false; }
            var match = Regex.Match(tcs_0.Task.Result.Content, @"gid = ([0-9]{10,12});");
            if (!match.Success) { return false; }
            if (match.Groups.Count != 2) { return false; }
            if (!match.Groups[1].Success) { return false; }
            if (match.Groups[1].Length == 0) { return false; }

            var gid = match.Groups[1].Value;
            request = new RestRequest($"ajax/uncledatoolsbyajax.php?gid={gid}&lang=zh&uc=0", Method.GET);
            request.AddHeader("Referer", $"https://www.javbus.com/{no}");

            var tcs_1 = new TaskCompletionSource<IRestResponse>();
            var response_1 = client.ExecuteAsync(request, (r) =>
            {
                tcs_1.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs_1.Task.IsCompleted, 10000);
            if (!tcs_1.Task.IsCompleted) { return false; }
            var doc_0 = new HtmlDocument() { OptionReadEncoding = false };
            doc_0.LoadHtml(Encoding.UTF8.GetString(tcs_1.Task.Result.RawBytes));
            HtmlNodeCollection node_table = doc_0.DocumentNode.SelectNodes("//tr");
            if (node_table == null) { return false; }

            foreach (var cell in node_table)
            {
                JavDetail jd = new JavDetail();
                var match1 = Regex.Matches(cell.InnerHtml, @"urn:btih:([0-9A-F]{40})", RegexOptions.IgnoreCase);
                if (match1.Count == 0) { continue; }
                if (match1[0].Groups.Count < 2) { continue; }
                jd.Hash = match1[0].Groups[1].Value.Trim();
                jd.Marget = $"magnet:?xt=urn:{jd.Hash}";

                var match2 = Regex.Matches(cell.InnerHtml, @"(.*)<.a>", RegexOptions.IgnoreCase);
                if (match2.Count < 3) { continue; }
                if (match2[0].Groups.Count < 2) { continue; }
                jd.No = no;

                var doc_1 = new HtmlDocument() { OptionReadEncoding = false };
                doc_1.LoadHtml(match2[0].Groups[1].Value);

                jd.Title = doc_1.DocumentNode.InnerText.Trim();

                jd.Size = match2[1].Groups[1].Value.Trim();
                DateTime dt2 = DateTime.MinValue;
                DateTime.TryParse(match2[2].Groups[1].Value.Trim(), out dt2);
                jd.Update = dt2;
                d.Add(jd);
                
            }
            action(d);
            return true;
        }

        public static bool GetIndex(string search, Action<Dictionary<string, JavInfo>, long, long> action)
        {
            Dictionary<string, JavInfo> d = new Dictionary<string, JavInfo>();
            HtmlNode _tmp;
            HtmlNode _tmp_frame;
            HtmlNode _tmp_info;
            HtmlNode _tmp_tag;
            var tcs_0 = new TaskCompletionSource<IRestResponse>();
            var tcs_1 = new TaskCompletionSource<IRestResponse>();

            RestClient client = new RestClient("https://www.javbus.com/");
            RestRequest request = new RestRequest($"search/{search}", Method.GET);
            client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile Safari/602.1";
            request.AddCookie("existmag", "mag");
            request.AddHeader("Referer", $"https://www.javbus.com/");

            var response_0 = client.ExecuteAsync(request, (r) =>
            {
                tcs_0.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 10000);
            if (!tcs_0.Task.IsCompleted) { return false; }

            if (tcs_0.Task.Result.StatusCode != System.Net.HttpStatusCode.OK) { return false; }
            var doc_0 = new HtmlDocument() { OptionReadEncoding = false };
            doc_0.LoadHtml(Encoding.UTF8.GetString(tcs_0.Task.Result.RawBytes));
            HtmlNode node_MessageBox = doc_0.DocumentNode.SelectSingleNode("//*[contains(@class,'alert alert-success alert-common')]");
            if (node_MessageBox != null)
            {
                string _marget = new String(HttpUtility.HtmlDecode(node_MessageBox.SelectSingleNode("//*[@id='resultshowmag']").InnerText).Where(Char.IsDigit).ToArray());
                Int64.TryParse(_marget, out long long_marget);
                string _all = new String(HttpUtility.HtmlDecode(node_MessageBox.SelectSingleNode("//*[@id='resultshowall']").InnerText).Where(Char.IsDigit).ToArray());
                Int64.TryParse(_all, out long long_all);
                HtmlNodeCollection nodes = doc_0.DocumentNode.SelectNodes("//*[@id='waterfall']/div[1]/*[contains(@class,'item')]");

                request = new RestRequest($"uncensored/search/{search}", Method.GET);
                var response_1 = client.ExecuteAsync(request, (r) =>
                {
                    tcs_1.SetResult(r);
                });
                SpinWait.SpinUntil(() => tcs_1.Task.IsCompleted, 10000);
                if (!tcs_1.Task.IsCompleted) { return false; }

                if (tcs_1.Task.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var doc_1 = new HtmlDocument() { OptionReadEncoding = false };
                    doc_1.LoadHtml(Encoding.UTF8.GetString(tcs_1.Task.Result.RawBytes));
                    HtmlNodeCollection nodes_1 = doc_1.DocumentNode.SelectNodes("//*[@id='waterfall']/div[1]/*[contains(@class,'item')]");
                    nodes_1.ToList().ForEach(f => nodes.Add(f));
                }

                foreach (var f in nodes)
                {
                    _tmp = null;
                    _tmp_frame = null;
                    _tmp = f.Element("a");
                    if (_tmp == null) { continue; }
                    var _href = _tmp.GetAttributeValue("href", "");
                    var doc_tmp = new HtmlDocument() { OptionReadEncoding = false };

                    doc_tmp.LoadHtml(_tmp.InnerHtml);
                    _tmp_frame = doc_tmp.DocumentNode.SelectSingleNode("//*[contains(@class,'photo-frame')]");
                    _tmp_info = doc_tmp.DocumentNode.SelectSingleNode("//*[contains(@class,'photo-info')]");
                    _tmp_tag = doc_tmp.DocumentNode.SelectSingleNode("//*[contains(@class,'photo-info')]/*[contains(@class,'item-tag')]");

                    if (_tmp_frame == null) { continue; }
                    if (_tmp_info == null) { continue; }

                    var name = _tmp_frame.Element("img").GetAttributeValue("title", "");
                    var src = _tmp_frame.Element("img").GetAttributeValue("src", "");

                    var tmp = _tmp_info.Element("span").ChildNodes;
                    if (_tmp_info.Element("span").ChildNodes.Where(w => w.Name == "date").Count() != 2) { continue; }
                    var no = _tmp_info.Element("span").ChildNodes.Where(w => w.Name == "date").FirstOrDefault().InnerText;
                    var date = _tmp_info.Element("span").ChildNodes.Where(w => w.Name == "date").LastOrDefault().InnerText;

                    DateTime dt = DateTime.MinValue;
                    DateTime.TryParse(date, out dt);
                    JavInfo jc = new JavInfo()
                    {
                        No = no,
                        Date = dt,
                        Url = _href,
                        Image = src,
                        Title = name
                    };
                    d.Add(no, jc);
                }
                action(d, long_marget, long_all);
                return true;
            }
            return false;
        }

    }
}
