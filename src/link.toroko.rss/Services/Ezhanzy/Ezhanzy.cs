using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Wpf.Data;
using HtmlAgilityPack;
using System.Net;
using System.Net.Http;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Web;

namespace link.toroko.rsshub.Services.Ezhanzy
{
    class Ezhanzy
    {
        public static string cookie = "";
        public static IList<RestResponseCookie> Cookies = null;
        public static string username = "";
        public static string password = "";
        public static bool success = false;
        public static DateTime createDateTime;

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void Login()
        {
            cookie = "";
            RestClient client = new RestClient("http://ezhanzy.com");
            RestRequest request = new RestRequest($"/forum.php?mod=rss", Method.GET);
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36";
            request.AddHeader("Referer", @"http://ezhanzy.com/");
            var tcs_0 = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, (r) =>
            {
                tcs_0.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 5000);
            if (tcs_0.Task.IsCompleted)
            {
                if (tcs_0.Task.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    RestClient client_login = new RestClient("http://www.ezhanzy.com");
                    RestRequest request_login = new RestRequest($"/member.php?mod=logging&action=login&loginsubmit=yes&infloat=yes&lssubmit=yes&inajax=1", Method.POST);
                    client_login.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36";
                    request_login.AddHeader("Referer", @"http://ezhanzy.com/");
                    request_login.AddParameter("fastloginfield", "username");
                    request_login.AddParameter("username", ViewModelData.g.Sucai_ezhanzy_username);
                    request_login.AddParameter("cookietime", "2592000");
                    request_login.AddParameter("password", ViewModelData.g.Sucai_ezhanzy_password);
                    request_login.AddParameter("quickforward", "yes");
                    request_login.AddParameter("handlekey", "ls");
                    tcs_0.Task.Result.Cookies.ToList().ForEach(f => request.AddCookie(f.Name, f.Value));

                    var tcs_login = new TaskCompletionSource<IRestResponse>();
                    client_login.ExecuteAsync(request_login, (r) =>
                    {
                        tcs_login.SetResult(r);
                    });
                    SpinWait.SpinUntil(() => tcs_login.Task.IsCompleted, 5000);
                    if (tcs_login.Task.IsCompleted)
                    {
                        if (tcs_login.Task.Result.Content.Contains("window.location.href"))
                        {
                            cookie = String.Join(";", tcs_login.Task.Result.Cookies.Select(s => $"{s.Name}={s.Value}").ToArray());
                            Cookies = tcs_login.Task.Result.Cookies;
                            //    cookie = cookie.Replace("_identity-frontend", "_csrf-frontend");
                        }
                    }
                }
            }
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static ConcurrentDictionary<int, EzhanzyData> Search(string qq, string Keyword, int Page, out int TotalPage, ConcurrentDictionary<int, EzhanzyData> dict)
        {
            if (Page < 1) { Page = 1; }
            TotalPage = 1;
            int beforePage = Page - 1;
            if (beforePage < 1) { beforePage = 1; }
            int nextPage = 1;
            string Location = "";
            int TotalPageTmp = 1;

            ConcurrentDictionary<int, EzhanzyData> dict_current = new ConcurrentDictionary<int, EzhanzyData>();
            RestClient client = new RestClient("http://www.ezhanzy.com");
            RestRequest request = new RestRequest($"search.php?mod=forum", Method.POST);
            var tcs = new TaskCompletionSource<IRestResponse>();
            client.Encoding = Encoding.GetEncoding("GB18030");

            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36";
            request.AddHeader("Referer", @"http://ezhanzy.com/");
            Cookies?.ToList().ForEach(f => request.AddCookie(f.Name, f.Value));
            request.AddParameter("searchsubmit", "yes");
            request.AddParameter("srchtxt", Keyword);
            //  request.AddParameter("formhash", "");
            client.FollowRedirects = false;

            int time = 0;
            while (String.IsNullOrEmpty(Location))
            {
                time++;
                client.ExecuteAsync(request, (r) =>
                {
                    tcs.SetResult(r);
                });
                SpinWait.SpinUntil(() => tcs.Task.IsCompleted, 5000);
                if (tcs.Task.IsCompleted)
                {
                    Location = tcs.Task.Result.Headers.FirstOrDefault(f => f.Name.ToLower() == "location")?.Value.ToString();
                }
                if (String.IsNullOrEmpty(Location)) { Thread.Sleep(50); }
                if (time > 5) { return null; }
            }
        
            if (!String.IsNullOrEmpty(Location))
            {
                RestClient client1 = new RestClient("http://www.ezhanzy.com");
                RestRequest request1 = new RestRequest($"{Location}&page={Page}", Method.GET);
                var tcs1 = new TaskCompletionSource<IRestResponse>();
                client1.ExecuteAsync(request1, (r) =>
                {
                    tcs1.SetResult(r);
                });
                SpinWait.SpinUntil(() => tcs1.Task.IsCompleted, 5000);
                if (tcs1.Task.IsCompleted)
                {
                    if (tcs1.Task.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string Content = RestSharp.Extensions.MonoHttp.HttpUtility.HtmlDecode(System.Text.Encoding.GetEncoding("gb2312").GetString(tcs1.Task.Result.RawBytes));

                        Regex regex_urlTitle = new Regex(@"<h3 class=.xs3.>\n<a href=.([0-9A-Za-z\:\;\/\.\?\=\&\%]{1,256}). target=._blank.[ ]{0,1}>(.*)<\/a>\n<\/h3>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        Regex regex_totalpage = new Regex(@"<span title=.共 [0-9]{1,3} 页.> \/ ([0-9]{1,3}) 页<\/span>");

                        MatchCollection mc_urlTitle = regex_urlTitle.Matches(Content);
                        Match m_totalpage = regex_totalpage.Match(Content);
                        if (m_totalpage.Success)
                        {
                            int.TryParse(m_totalpage.Groups[1].Value, out TotalPageTmp);
                            TotalPage = TotalPageTmp;
                        }

                        if (mc_urlTitle.Count > 0)
                        {
                            int index = ((Page - 1) * 30);

                            foreach (Match m in mc_urlTitle)
                            {
                                if (m.Success)
                                {
                                    index++;
                                    string title = m.Groups[2].Value.Replace("<strong><font color=\"#ff0000\">", "").Replace("</font></strong>", "").Replace("<highlight>", "").Replace("</highlight>", "");
                                    dict.TryAdd(index, new EzhanzyData() { Title = title, Url = m.Groups[1].Value });
                                    dict_current.TryAdd(index, new EzhanzyData() { Title = title, Url = m.Groups[1].Value });
                                }
                            }
                        }
                    }
                }
            }
            nextPage = Page + 1;
            if(nextPage> TotalPageTmp) { nextPage = TotalPageTmp; }

        //    if (TotalPageTmp > 1 || Page > 1)
        //    {
                Program.requestNextList.AddOrUpdate(qq,
                    new Program.RequestArgs { Service = new EzhanzySearch(), Args = new Dictionary<string, object> { { "TotalPage", TotalPageTmp }, { "Page", nextPage }, { "Keyword", Keyword }, { "Dict", dict } } },
                    (key, oldValue) =>
                    new Program.RequestArgs { Service = new EzhanzySearch(), Args = new Dictionary<string, object> { { "TotalPage", TotalPageTmp }, { "Page", nextPage }, { "Keyword", Keyword }, { "Dict", dict } } });

                Program.requestBeforeList.AddOrUpdate(qq,
                    new Program.RequestArgs { Service = new EzhanzySearch(), Args = new Dictionary<string, object> { { "TotalPage", TotalPageTmp }, { "Page", beforePage }, { "Keyword", Keyword }, { "Dict", dict } } },
                    (key, oldValue) =>
                    new Program.RequestArgs { Service = new EzhanzySearch(), Args = new Dictionary<string, object> { { "TotalPage", TotalPageTmp }, { "Page", beforePage }, { "Keyword", Keyword }, { "Dict", dict } } });
        //    }

            Program.selected.AddOrUpdate(qq,
new Program.RequestArgs { Service = new EzhanzySearch(), Args = new Dictionary<string, object> { { "GetData", true }, { "TotalPage", TotalPageTmp }, { "Page", beforePage }, { "Keyword", Keyword },{ "Dict", dict } } },
(key, oldValue) =>
new Program.RequestArgs { Service = new EzhanzySearch(), Args = new Dictionary<string, object> { { "GetData", true }, { "TotalPage", TotalPageTmp }, { "Page", beforePage }, { "Keyword", Keyword }, { "Dict", dict } } });

            return dict_current;
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static EzhanzyItem GetData(string url)
        {
            EzhanzyItem ei = new EzhanzyItem();
            Login();
            if (String.IsNullOrEmpty(cookie)) { ei.Message = "登入失敗"; };

            RestClient client = new RestClient("http://www.ezhanzy.com");
            RestRequest request = new RestRequest($"{url.Replace(client.BaseUrl.OriginalString,"")}", Method.GET);
            Cookies.ToList().ForEach(f => request.AddCookie(f.Name, f.Value));
            var tcs = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, (r) =>
            {
                tcs.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs.Task.IsCompleted, 5000);
            if (tcs.Task.IsCompleted)
            {
                if (tcs.Task.Result.StatusCode == System.Net.HttpStatusCode.OK) {
                    string Content = RestSharp.Extensions.MonoHttp.HttpUtility.HtmlDecode(System.Text.Encoding.GetEncoding("gb2312").GetString(tcs.Task.Result.RawBytes));

                    var doc = new HtmlDocument() { OptionReadEncoding = false };
                    doc.LoadHtml(Content);

                    HtmlNode node_normal = null;
                    HtmlNode node = null;
                    HtmlNode node2 = null;
                    HtmlNode node_old_pw = null;
                    HtmlNode node_new = null;
                    HtmlNode node_old = null;

                    try
                    {
                        node_normal = doc.DocumentNode;
                    }
                    catch (Exception ex) { Debug.WriteLine(ex); }
                    //try
                    //{
                    //    node = doc.DocumentNode.SelectNodes("//*[contains(@class,'sf_ri')]").ToList()?.FirstOrDefault();
                    //}
                    //catch(Exception ex) { Debug.WriteLine(ex); }
                    //try
                    //{
                    //    node2 = doc.DocumentNode.SelectNodes("//*[contains(@class,'t_fsz')]").ToList()?.FirstOrDefault();
                    //}
                    //catch (Exception ex) { Debug.WriteLine(ex); }   

                    //try
                    //{
                    //    node_old_pw = doc.DocumentNode.SelectNodes("//*[contains(@class,'sf_you_mi')]/legend").ToList()?.FirstOrDefault();
                    //}
                    //catch (Exception ex) { Debug.WriteLine(ex); }
                    //try
                    //{
                    //    node_new = doc.DocumentNode.SelectNodes("//*[contains(@class,'showhide')]").ToList()?.FirstOrDefault();
                    //}
                    //catch (Exception ex) { Debug.WriteLine(ex); }
                    //try
                    //{
                    //    node_old = doc.DocumentNode.SelectNodes("//*[contains(@class,'sf_xia')]").ToList()?.FirstOrDefault();
                    //}
                    //catch (Exception ex) { Debug.WriteLine(ex); }


                    //if(node_new != null)
                    //{
                    //    RegexData(node_new.InnerHtml, node_new.InnerText, ref ei);
                    //}else if (node != null)
                    //{
                    //    RegexData(node.InnerHtml,node.InnerText, ref ei);
                    //}else if(node2 != null)
                    //{
                    //    RegexData(node2.InnerHtml, node2.InnerText, ref ei);
                    //}

                    if (node_normal != null)
                    {
                        RegexData(node_normal.InnerHtml, node_normal.InnerText, ref ei);
                    }


                    //if (node_old != null)
                    //{
                    //    RegexData(node_old.InnerHtml, node_old.InnerText, ref ei);

                    //    if (node_old_pw != null)
                    //    {
                    //        string old_pw = node_old_pw.InnerText.Replace("解压/提取密码：", "");
                    //        if (!String.IsNullOrEmpty(old_pw))
                    //        {
                    //            if (!String.IsNullOrEmpty(ei.UnzipPassword))
                    //            {
                    //                if (ei.UnzipPassword != old_pw)
                    //                {
                    //                    ei.ReferenceData = ei.UnzipPassword;
                    //                }
                    //            }
                    //            ei.UnzipPassword = old_pw;
                    //        }
                    //    }
                    //}

                    if (!String.IsNullOrEmpty(ei.TxtFile))
                    {

                        Uri baseAddress = new Uri("http://www.ezhanzy.com");
                        CookieContainer cookieContainer = new CookieContainer();
                        Cookies.ToList().ForEach(f => cookieContainer.Add(new Cookie(f.Name, f.Value) { Domain = f.Domain }));
                        HttpResponseMessage response = null;

                        using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                        {
                            using (var client_0 = new HttpClient(handler) { BaseAddress = baseAddress })
                            {
                                client_0.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36");
                                client_0.DefaultRequestHeaders.Add("Referer", "http://www.ezhanzy.com/");
                                response = client_0.GetAsync($"{ei.TxtFile.Replace("baseAddress", "")}").Result;
                                if (response != null)
                                {
                                    response.Content.Headers.ContentType.CharSet = "gbk";
                                    string file_data = response.Content.ReadAsStringAsync().Result;
                                    RegexData(file_data, file_data, ref ei);
                                }
                            }
                        }
                    }
                }
                else
                {
                    ei.Message = "读取失敗";
                }
            }
            return ei;
        }

        private static void RegexData(string html,string text, ref EzhanzyItem ei)
        {
            Regex regex_txtfile = new Regex(@"<a href=.(.*). target=._blank. rel=.nofollow. style=.text-decoration:none..>");

            Regex regex_baidupan = new Regex(@"(http[s]{0,1}:\/\/pan.baidu.com\/s\/[A-Za-z0-9_-]{7,24})([  　:：]{0,2}.[\u3000\u3400-\u4DBF\u4E00-\u9FFF:： ]{0,3}[ 　:：]{0,2})?([A-Za-z0-9]{4})?", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex regex_baidupan_only = new Regex(@"(http[s]{0,1}:\/\/pan.baidu.com\/s\/[A-Za-z0-9_-]{7,24})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex regex_baidupan_only2 = new Regex(@"(http[s]{0,1}:\/\/pan.baidu.com\/share\/[A-Za-z0-9_\-\?=&]{7,35})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex regex_pwd = new Regex(@"[解]?[压]?密码[（）A-Za-z0-9/.-]*[：: ]{0,1}(\r\n|\n\r|\n|\r|)([A-Za-z0-9\.\-]{1,60})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex regex_ed2k = new Regex(@"(ed2k:..\|file\|(.*)\|([0-9]{1,12})\|([0-9a-fA-F]{32})\|(.*)\|\/)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Regex regex_yunpan = new Regex(@"http[s]{0,1}:\/\/yunpan.cn/[0-9A-Za-z_]{10,16}", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Regex regex_yunpanpw = new Regex(@"(http[s]{0,1}:\/\/yunpan.cn\/[A-Za-z0-9_-]{7,24}).*(密码|提取码).*([A-Za-z0-9]{4})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Regex regex_ct = new Regex(@"http[s]{0,1}:\/\/\w{1,20}.ctfile.com\/fs\/[0-9]{7}-[0-9]{9}", RegexOptions.IgnoreCase | RegexOptions.Multiline);


			if (String.IsNullOrEmpty(text.Replace("\n", "").Trim()))
            {
                text = html;
            }

            //try
            //{
            //    var doc_0 = new HtmlDocument() { OptionReadEncoding = false };
            //    doc_0.LoadHtml(html);
            //    HtmlNodeCollection hmc = doc_0.DocumentNode.SelectNodes("//a[@id[starts-with(.,\"ed2k_\")]]");
            //    foreach (HtmlNode node in hmc)
            //    {
            //        string ed2k = node.GetAttributeValue("href", "");
            //        if (ed2k != null)
            //        {
            //            ei.Ed2k.TryAdd(ed2k, node.InnerText);
            //        }
            //    }
            //}
            //catch (Exception ex) { Debug.WriteLine(ex); }


            if (html.TrimEnd('\n').Split('\n').Length == 1)
            {
                if (!string.IsNullOrEmpty(ei.UnzipPassword))
                {
                    string tmp = ei.UnzipPassword;
                    if (html != tmp)
                    {
                        if (ei.BaiduPan.Where(w => w.Value == tmp).Count() > 0)
                        {
                            ei.UnzipPassword = html.Trim('\n');
                        }
                        else
                        {
                            ei.ReferenceData += $"{Environment.NewLine}{html.Trim('\n')}";
                        }
                    }
                }
                else
                {
                    ei.UnzipPassword = html.Replace(Environment.NewLine,"");
                }
            }

            Match m = regex_txtfile.Match(html);
            if (m.Success)
            {
                ei.TxtFile = m.Groups[m.Groups.Count - 1].Value;
            }

            Match m_pw = regex_pwd.Match(text);
            if (m_pw.Success)
            {
                string tmp_pw = m_pw.Groups[m_pw.Groups.Count - 1].Value;

                if (!string.IsNullOrEmpty(ei.UnzipPassword))
                {
                    string tmp = ei.UnzipPassword;
                    if (tmp_pw != tmp)
                    {
                        if (ei.BaiduPan.Where(w => w.Value == tmp).Count() > 0)
                        {
                            ei.UnzipPassword = tmp_pw;
                        }
                        else
                        {
                            ei.ReferenceData += $"{Environment.NewLine}{tmp_pw}";
                        }
                    }
                }
                else
                {
                    ei.UnzipPassword = tmp_pw;
                }
            }

            MatchCollection mc_baidu = regex_baidupan.Matches(text);
            foreach (Match m_baidu in mc_baidu)
            {
                if (m_baidu.Success)
                {
                    if (m_baidu.Groups.Count > 3)
                    {
                        ei.BaiduPan.TryAdd(m_baidu.Groups[m_baidu.Groups.Count - 3].Value, m_baidu.Groups[m_baidu.Groups.Count - 1].Value);
                    }
                }
            }

            MatchCollection mc_baiduonly = regex_baidupan_only.Matches(text);
            foreach (Match m_baiduonly in mc_baiduonly)
            {
                if (m_baiduonly.Success)
                {
                    if (m_baiduonly.Groups.Count == 2)
                    {
                        ei.BaiduPan.TryAdd(m_baiduonly.Groups[m_baiduonly.Groups.Count - 1].Value,"");
                    }
                }
            }

            MatchCollection mc_baiduonly2 = regex_baidupan_only2.Matches(text);
            foreach (Match m_baiduonly2 in mc_baiduonly2)
            {
                if (m_baiduonly2.Success)
                {
                    if (m_baiduonly2.Groups.Count == 2)
                    {
                        ei.BaiduPan.TryAdd(m_baiduonly2.Groups[m_baiduonly2.Groups.Count - 1].Value,"");
                    }
                }
            }

            MatchCollection mc_ed2k = regex_ed2k.Matches(html);
            foreach (Match m_ed2k in mc_ed2k)
            {
                if (m_ed2k.Groups.Count == 6)
                {
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"AA{m_ed2k.Groups[1].Value}ZZ");
                    string thunder = $"thunder://{System.Convert.ToBase64String(plainTextBytes)}";
                    ei.Ed2k.TryAdd(thunder, HttpUtility.UrlDecode(m_ed2k.Groups[2].Value));
                }
            }

            if (ei.BaiduPan.Count == 0)
            {
                MatchCollection mc_baiduonly_html = regex_baidupan_only.Matches(html);
                foreach (Match m_baiduonly_html in mc_baiduonly_html)
                {
                    if (m_baiduonly_html.Success)
                    {
                        if (m_baiduonly_html.Groups.Count == 2)
                        {
                            ei.BaiduPan.TryAdd(m_baiduonly_html.Groups[m_baiduonly_html.Groups.Count - 1].Value, "");
                        }
                    }
                }

                MatchCollection mc_baiduonly_html2 = regex_baidupan_only2.Matches(html);
                foreach (Match m_baiduonly_html2 in mc_baiduonly_html2)
                {
                    if (m_baiduonly_html2.Success)
                    {
                        if (m_baiduonly_html2.Groups.Count == 2)
                        {
                            ei.BaiduPan.TryAdd(m_baiduonly_html2.Groups[m_baiduonly_html2.Groups.Count - 1].Value, "");
                        }
                    }
                }
            }


			MatchCollection mc_ct = regex_ct.Matches(text);
			foreach (Match m_ct in mc_ct)
			{
				if (m_ct.Success)
				{
					string cturl = m_ct.Groups[0].Value;
					if (String.IsNullOrEmpty(cturl))
					{
						if (!ei.CtPan.Contains(cturl))
						{
							ei.CtPan.Add(cturl);
						}
					}
				}
			}

			MatchCollection mc_yunpan = regex_yunpan.Matches(text);
			foreach (Match m_yp in mc_yunpan)
			{
				if (m_yp.Success)
				{
					string ypurl = m_yp.Groups[0].Value;
					if (!ei.CtPan.Contains(ypurl))
					{
						ei.YunPan.TryAdd(ypurl, "");
					}
				}
			}

			MatchCollection m_3_0 = regex_yunpan.Matches(text);
			if (m_3_0.Count > 0)
			{
				foreach (Match match in m_3_0)
				{
					GroupCollection g = match.Groups;
					if (g.Count == 4)
					{
						ei.YunPan.TryAdd(g[1].Value, g[3].Value);
					}
				}
			}

		}

    }
}
