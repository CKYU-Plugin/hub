using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Robot.Property;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Wpf.Data;
using Wpf.Models;
using System.Net.Http;
using SharpCompress.Archives;

namespace link.toroko.rsshub.Services.Sucai
{
    class Xssvip
    {
        public static string cookie = "";
        public static IList<RestResponseCookie> Cookies = null;
        public static string username = "";
        public static string password = "";
        public static bool success = false;
        public static DateTime createDateTime;

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static bool Login()
        {
            Regex regex_csrf_token = new Regex(@"<meta name=.csrf-token. content=.([0-9A-Za-z=_].*).>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex regex_csrf_param = new Regex(@"<meta name=.csrf-param. content=.([0-9A-Za-z=_].*).>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            IList<RestResponseCookie> Cookies_loginbefore = null;
            string csrf_param = "";
            string csrf_token = "";

            RestClient client_0 = new RestClient("http://xiaobai.xssvip.cn");
            RestRequest request_0 = new RestRequest("/login", Method.GET);
            client_0.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
            var tcs_0 = new TaskCompletionSource<IRestResponse>();
            client_0.ExecuteAsync(request_0, (r) =>
            {
                tcs_0.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 20000);
            if (tcs_0.Task.IsCompleted)
            {
                if (tcs_0.Task.Result.StatusCode == HttpStatusCode.OK)
                {
                    Cookies_loginbefore = tcs_0.Task.Result.Cookies;
                    cookie = String.Join(";", tcs_0.Task.Result.Cookies.Select(s => $"{s.Name}={s.Value}").ToArray()) + ";";
                    Match m_csrf_param = regex_csrf_param.Match(tcs_0.Task.Result.Content);
                    Match m_csrf_token = regex_csrf_token.Match(tcs_0.Task.Result.Content);
                    if (m_csrf_param.Success && m_csrf_token.Success && m_csrf_param.Groups.Count == 2 && m_csrf_token.Groups.Count == 2)
                    {
                        csrf_param = m_csrf_param.Groups[1].Value;
                        csrf_token = m_csrf_token.Groups[1].Value;
                    }
                    else { return false; }
                }
                else { return false; }
            }
            else { return false; }

            RestClient client = new RestClient("http://xiaobai.xssvip.cn");
            RestRequest request = new RestRequest("/login", Method.POST);
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
            request.AddHeader("Referer", @"http://xiaobai.xssvip.cn/");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded;");
            request.AddParameter("application/x-www-form-urlencoded", $"{csrf_param}={csrf_token}&LoginForm%5Busername%5D={ViewModelData.g.Sucai_xssvip_username}&LoginForm%5Bpassword%5D={ViewModelData.g.Sucai_xssvip_password}", ParameterType.RequestBody);
            Cookies_loginbefore.ToList().ForEach(f => request.AddCookie(f.Name, f.Value));
            var tcs = new TaskCompletionSource<IRestResponse>();
            client.FollowRedirects = false;
            client.ExecuteAsync(request, (r) =>
            {
                tcs.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs.Task.IsCompleted, 20000);
            if (tcs.Task.IsCompleted)
            {
                if (tcs.Task.Result.StatusCode == HttpStatusCode.Found)
                {
                    cookie += String.Join(";", tcs.Task.Result.Cookies.Select(s => $"{s.Name}={s.Value}").ToArray());
                    Cookies = tcs.Task.Result.Cookies;
                    Cookies_loginbefore.ToList().ForEach(f => Cookies.Add(new RestResponseCookie() { Name = f.Name, Value = f.Value }));
                    createDateTime = DateTime.Now;
                    username = ViewModelData.g.Sucai_xssvip_username;
                    password = ViewModelData.g.Sucai_xssvip_password;
                    return true;
                }
            }
            return false;
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static bool GetOnePassTokenAndStatus(ref string _onepassToken , ref string _content)
        {
            Regex regex_csrf_token = new Regex(@"<meta name=.csrf-token. content=.([0-9A-Za-z=_].*).>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex regex_csrf_param = new Regex(@"<meta name=.csrf-param. content=.([0-9A-Za-z=_].*).>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            RestClient client_1 = new RestClient("http://xiaobai.xssvip.cn");
            RestRequest request_1 = new RestRequest(Method.GET);
            client_1.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
            Cookies.ToList().ForEach(f => request_1.AddCookie(f.Name, f.Value));
            var tcs_1 = new TaskCompletionSource<IRestResponse>();
            client_1.ExecuteAsync(request_1, (r) =>
            {
                tcs_1.SetResult(r);
            });
            SpinWait.SpinUntil(() => tcs_1.Task.IsCompleted, 20000);
            if (tcs_1.Task.IsCompleted)
            {
                Match m_csrf_param = regex_csrf_param.Match(tcs_1.Task.Result.Content);
                Match m_csrf_token = regex_csrf_token.Match(tcs_1.Task.Result.Content);
                if (m_csrf_param.Success && m_csrf_token.Success && m_csrf_param.Groups.Count == 2 && m_csrf_token.Groups.Count == 2)
                {
                    _onepassToken = m_csrf_token.Groups[1].Value;
                    _content = tcs_1.Task.Result.Content;
                    return true;
                }
            }
            return false;
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static Dictionary<string,string> GetStatus()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string _onepassToken = "";
            string _content = "";
            if (!Login()) { return null; }
            if (!GetOnePassTokenAndStatus(ref _onepassToken, ref _content)) { return null; }
            HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(_content);
            List<string> tmp = htmlDoc.DocumentNode.SelectNodes("/html/body/div[1]/ul/li")?.Select(s => s.InnerText)?.ToList();
            foreach(var t in tmp)
            {
                if (t.Contains("："))
                {
                    string[] s = t.Split('：');
                    dic.Add(s[0], s[1]);
                }
            }
            return dic;
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static XssvipItem GetData(string url)
        {
            XssvipItem xi = new XssvipItem();
            string _onepassToken="";
            string _content = "";
            string data0 = "";
            int code = 0;
            string msg = "";
            Dictionary<string,string> links = new Dictionary<string, string>();
            Uri baseAddress = new Uri("http://xiaobai.xssvip.cn");
            CookieContainer cookieContainer = new CookieContainer();
            HttpResponseMessage response = null;

            try
            {
                if (!Login()) { return new XssvipItem() { Message = "登入失败" }; }
                if (!GetOnePassTokenAndStatus(ref _onepassToken, ref _content)) { return new XssvipItem() { Message = "登入失败2" }; }
                Cookies.ToList().ForEach(f => cookieContainer.Add(baseAddress, new Cookie(f.Name, f.Value)));

                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                {
                    using (var client_0 = new HttpClient(handler) { BaseAddress = baseAddress })
                    {
                        client_0.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");
                        client_0.DefaultRequestHeaders.Add("Referer", "http://xiaobai.xssvip.cn/");
                        client_0.DefaultRequestHeaders.Add("X-CSRF-TOKEN", _onepassToken);
                        var content = new FormUrlEncodedContent(new[]
                        {
                        new KeyValuePair<string, string>("url", url),
                        });
                        response = client_0.PostAsync("/url", content).Result;
                    }
                }

                if (response != null)
                {
                    data0 = response.Content.ReadAsStringAsync().Result;
                    xi.Message = "解码失败";
                    try
                    {
                        var d = JsonConvert.DeserializeObject<XssvipDetail>(data0, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                        });
                        code = d.code;
                        msg = d.msg;
                        if (d.link != null)
                        {
                            links.Add(d.link, "素材下载");
                        }
                    }
                    catch
                    {
                        var d = JsonConvert.DeserializeObject<XssvipDetail2>(data0, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                        });
                        code = d.code;
                        d.link?.ToList().ForEach(f =>
                        {
                            links.Add(f.link, f.label);
                        });
                    }

                    if (code == 1)
                    {
                        foreach(var fr in links)
                        {
                            //baseAddress = new Uri($"http://xiaobai.xssvip.cn{fr.Key}");

                            //using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                            //{
                            //    using (var client_0 = new HttpClient(handler) { BaseAddress = baseAddress })
                            //    {
                            //        client_0.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");
                            //        client_0.DefaultRequestHeaders.Add("Referer", "http://xiaobai.xssvip.cn/");
                            //        client_0.DefaultRequestHeaders.Add("X-CSRF-TOKEN", _onepassToken);
                            //        response = client_0.GetAsync("/url").Result;
                            //    }
                            //}

                            RestClient client2 = new RestClient($"http://xiaobai.xssvip.cn{fr.Key}");
                            RestRequest request2 = new RestRequest("", Method.GET);
                            client2.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
                            request2.AddHeader("Referer", @"http://xiaobai.xssvip.cn/");
                            request2.AddHeader("X-CSRF-TOKEN", _onepassToken);
                            Cookies.ToList().ForEach(f => request2.AddCookie(f.Name, f.Value));
                            client2.FollowRedirects = false;

                            var tcs_1 = new TaskCompletionSource<IRestResponse>();
                            client2.ExecuteAsync(request2, (r) =>
                            {
                                tcs_1.SetResult(r);
                            });
                            SpinWait.SpinUntil(() => tcs_1.Task.IsCompleted, 20000);

                            if (tcs_1.Task.IsCompleted)
                            {
                                if(tcs_1.Task.Result.StatusCode == HttpStatusCode.Found)
                                {
                                    var location = tcs_1.Task.Result.Headers.Where(w => w.Name == "Location").FirstOrDefault();
                                    string slocation = location?.Value.ToString();
                                    if (slocation != "")
                                    {
                                        xi.DownloadUrls.Add(slocation, fr.Value);
                                        continue;
                                    }
                                    else
                                    {
                                        xi.Message += $"{Environment.NewLine}素材下载提取失败(Location)" ;
                                        continue;
                                    }
                                }
                                if (tcs_1.Task.Result.StatusCode == HttpStatusCode.OK)
                                {
                                    Regex regex_link = new Regex(@"var url=.(.*)\"";", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                    Match m_link = regex_link.Match(tcs_1.Task.Result.Content);
                                    if (m_link.Success & m_link.Groups.Count == 2)
                                    {
                                        xi.DownloadUrls.Add(m_link.Groups[1].Value, fr.Value);
                                    }
                                    else
                                    {
                                        xi.Message += $"{Environment.NewLine}素材下载提取失败(var)";
                                        continue;
                                    }
                                }
                                else
                                {
                                    xi.Message += $"{Environment.NewLine}找不到素材下载方法";
                                    continue;
                                }
                            }
                            else
                            {
                                xi.Message += $"{Environment.NewLine}素材下载提取失败";
                                continue;
                            }
                        };
                    }
                    else
                    {
                        if (msg != string.Empty)
                        {
                            xi.Message += $"{Environment.NewLine}{msg}";
                        }
                        else
                        {
                            xi.Message += $"{Environment.NewLine}找不到素材";
                        }
                    }
                }
                else
                {
                    xi.Message += $"{Environment.NewLine}连线失败";
                }
            }
            catch (Exception ex) { return new XssvipItem() { Message = $"{ex.ToString()}" }; }
            return xi;
        }
    }
}
