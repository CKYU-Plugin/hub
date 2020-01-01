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
using System.Diagnostics;

namespace link.toroko.rsshub.Services.rrsc
{
	class Meinigui
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
			RestClient client = new RestClient("http://www.meinigui.com/");
			RestRequest request = new RestRequest($"/site/login.html", Method.POST);
			client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
			request.AddHeader("Referer", @"http://www.meinigui.com/");
			request.AddParameter("username", ViewModelData.g.Sucai_rrsc_username);
			request.AddParameter("password", ViewModelData.g.Sucai_rrsc_password);
			var tcs_0 = new TaskCompletionSource<IRestResponse>();
			client.ExecuteAsync(request, (r) =>
			{
				tcs_0.SetResult(r);
			});
			SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 5000);
			if (tcs_0.Task.IsCompleted)
			{
				string pan = tcs_0.Task.Result.Content;
				if (pan.Contains("{") & pan.Contains("}"))
				{
					int first = 0;
					int last = pan.Length;
					first = pan.IndexOf("{");
					last = pan.LastIndexOf("}");
					last++;
					pan = pan.Substring(first, last - first);
					try
					{
						var d = JObject.Parse(pan);
						int i = d["status"].Value<int>();
						if (i == 1)
						{
							success = true;

							cookie = String.Join(";", tcs_0.Task.Result.Cookies.Select(s => $"{s.Name}={s.Value}").ToArray());
							Cookies = tcs_0.Task.Result.Cookies;
							cookie = cookie.Replace("_identity-frontend", "_csrf-frontend");
							createDateTime = DateTime.Now;
							username = ViewModelData.g.Sucai_rrsc_username;
							password = ViewModelData.g.Sucai_rrsc_password;
						}
						else { success = false; }
					}
					catch { }
				}
			}
		}

		[HandleProcessCorruptedStateExceptions, SecurityCritical]
		public static MeiniguiItem GetFile(MeiniguiItem mi)
		{
			RestClient client = null;
			RestRequest request = null;
			client = new RestClient($"{mi.DownloadUrl}");
			client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
			request = new RestRequest(Method.GET);
			request.AddHeader("Referer", @"http://www.meinigui.com/");
			Cookies?.Where(w => w.Name == "PHPSESSID").ToList().ForEach(f => request.AddCookie(f.Name, f.Value));

			var tcs_1 = new TaskCompletionSource<IRestResponse>();
			client.ExecuteAsync(request, (rp) =>
			{
				tcs_1.SetResult(rp);
			});

			SpinWait.SpinUntil(() => tcs_1.Task.IsCompleted, 30000);
			if (tcs_1.Task.IsCompleted)
			{
				string savePath = Path.Combine(RobotBase.currentfloder, RobotBase.path_rrsc, mi.FileName);
				string extension = Path.GetExtension(savePath);
				string filename = Path.GetFileNameWithoutExtension(savePath);
				string tempFileName = Path.GetFileName(savePath);
				int _count = 0;
				while (File.Exists(savePath))
				{
					tempFileName = $"{filename}({_count++}){extension}";
					savePath = Path.Combine(RobotBase.currentfloder, RobotBase.path_rrsc, tempFileName);
				}
				File.WriteAllBytes(savePath, tcs_1.Task.Result.RawBytes);
				mi.FileName = tempFileName;
				return mi;
			}
			else
			{
				return null;
			}
		}

		[HandleProcessCorruptedStateExceptions, SecurityCritical]
		public static MeiniguiItem GetData2(string url)
		{
			MeiniguiItem mi = new MeiniguiItem();
			Regex regex_0 = new Regex(@"file.html\?id=[A-Za-z0-9+/]*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Regex regex_pwd = new Regex(@"[解]?[压]?密码[为]?[（）A-Za-z0-9/.-]*[：: ]{0,1}(\r\n|\n\r|\n|\r|)([A-Za-z0-9\.\-]{1,60})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Regex regex_isChinese = new Regex("[\u3000\u3400-\u4DBF\u4E00-\u9FFF].*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Regex regex_baidupan = new Regex(@"(http[s]{0,1}:\/\/pan.baidu.com\/s\/[A-Za-z0-9_-]{7,24})([  　:：]{0,2}.[\u3000\u3400-\u4DBF\u4E00-\u9FFF:： ]{0,3}[ 　:：]{0,2})?([A-Za-z0-9]{4})?", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Regex regex_baidupan_only = new Regex(@"(http[s]{0,1}:\/\/pan.baidu.com\/s\/[A-Za-z0-9_-]{7,24})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Regex regex_baidupan_only2 = new Regex(@"(http[s]{0,1}:\/\/pan.baidu.com\/share\/[A-Za-z0-9_\-\?=&]{7,35})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Regex regex_ctpan = new Regex(@"(http[s]{0,1}:\/\/renrensucai.ctfile.com\/fs\/[A-Za-z0-9\-]{1,20})");
			Regex regex_Thunder = new Regex(@"(Thunder:\/\/[a-zA-Z0-9+\/]+={0,2})");
			Regex regex_pwd2 = new Regex(@"<div [A-Za-z0-9""=]{5,20}padding[A-Za-z0-9""=;:]{5,20}>([A-Za-z0-9-=_]{1,20})<\/div>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			Regex regex_yunpan = new Regex(@"(http[s]{0,1}:\/\/yunpan.cn\/[A-Za-z0-9_-]{7,24}).*(密码|提取码).*([A-Za-z0-9]{4})", RegexOptions.IgnoreCase | RegexOptions.Multiline);

			RestClient client = new RestClient(RestSharp.Extensions.MonoHttp.HttpUtility.HtmlDecode(url));
			RestRequest request = new RestRequest(Method.POST);
			client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
			request.AddHeader("Referer", @"https://www.rr-sc.com/");
			var tcs_0 = new TaskCompletionSource<IRestResponse>();
			client.ExecuteAsync(request, (r) =>
			{
				tcs_0.SetResult(r);
			});
			SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 20000);
			if (tcs_0.Task.IsCompleted)
			{
				string s = "";
				string tmp = RestSharp.Extensions.MonoHttp.HttpUtility.HtmlDecode(System.Text.Encoding.GetEncoding("gb2312").GetString(tcs_0.Task.Result.RawBytes));
				var doc_0 = new HtmlDocument() { OptionReadEncoding = false };
				doc_0.LoadHtml(tmp);

				try
				{
					HtmlNodeCollection hmc = doc_0.DocumentNode.SelectNodes("//a[@id[starts-with(.,\"ed2k_\")]]");
					foreach (HtmlNode node in hmc)
					{
						string ed2k = node.GetAttributeValue("href", "");
						if (ed2k != null)
						{
							mi.Ed2k.TryAdd(ed2k, node.InnerText);
						}
					}
				}
				catch (Exception ex) { Debug.WriteLine(ex); }

				HtmlNode node_body = doc_0.DocumentNode.SelectNodes("//*[contains(@class,'t_f')]").FirstOrDefault();
				if (node_body != null)
				{
					s = node_body.InnerText;
				}
				Match m_2 = regex_pwd.Match(s);
				if (m_2.Success)
				{
					if (m_2.Groups.Count > 1)
					{
						mi.UnzipPassword = m_2.Groups[2].Value;
					}
				}

				s = Regex.Replace(s, "http", $"{Environment.NewLine}http", RegexOptions.IgnoreCase);

				MatchCollection m_3 = regex_baidupan.Matches(s.Replace("http", $"{Environment.NewLine}http").Replace("HTTP", $"{Environment.NewLine}HTTP"));
				if (m_3.Count > 0)
				{
					foreach (Match match in m_3)
					{
						GroupCollection g = match.Groups;
						if (g.Count == 4)
						{
							mi.BaiduPan.AddOrUpdate(g[1].Value, g[3].Value, (key, oldValue) => g[3].Value);
						}
					}
				}

				MatchCollection m_3_0 = regex_yunpan.Matches(s);
				if (m_3_0.Count > 0)
				{
					foreach (Match match in m_3_0)
					{
						GroupCollection g = match.Groups;
						if (g.Count == 4)
						{
							mi.YunPan.TryAdd(g[1].Value, g[3].Value);
						}
					}
				}

				MatchCollection m_3_1 = regex_baidupan_only.Matches(s);
				if (m_3_1.Count > 0)
				{
					foreach (Match match in m_3_1)
					{
						GroupCollection g = match.Groups;
						if (g.Count == 2)
						{
							mi.BaiduPan.TryAdd(g[1].Value, "");
						}
					}
				}

				MatchCollection m_3_2 = regex_baidupan_only2.Matches(s);
				if (m_3_2.Count > 0)
				{
					foreach (Match match in m_3_2)
					{
						GroupCollection g = match.Groups;
						if (g.Count == 2)
						{
							mi.BaiduPan.TryAdd(g[1].Value, "");
						}
					}
				}

				MatchCollection m_4 = regex_ctpan.Matches(s);
				if (m_4.Count > 0)
				{
					foreach (Match match in m_4)
					{
						GroupCollection g = match.Groups;
						if (g.Count == 2)
						{
							if (mi.CtPan.FindAll(f => f == g[1].Value).Count == 0)
							{
								mi.CtPan.Add(g[1].Value);
							}
						}
					}
				}

				MatchCollection m_5 = regex_Thunder.Matches(s);
				if (m_5.Count > 0)
				{
					foreach (Match match in m_5)
					{
						GroupCollection g = match.Groups;
						if (g.Count == 2)
						{
							if (mi.Thunder.FindAll(f => f == g[1].Value).Count == 0)
							{
								mi.Thunder.Add(g[1].Value);
							}
						}
					}
				}

				if (mi.UnzipPassword == "")
				{
					MatchCollection m_6 = regex_pwd2.Matches(s);
					foreach (Match m_6_9 in m_6)
					{
						if (m_6_9.Success)
						{
							if (m_6_9.Groups.Count > 1)
							{
								mi.UnzipPassword = m_6_9.Groups[1].Value;
							}
						}
					}
				}


			}
			return mi;
		}


		[HandleProcessCorruptedStateExceptions, SecurityCritical]
		public static MeiniguiItem GetData(string url)
		{
			try
			{
				Login();
				if (username != ViewModelData.g.Sucai_rrsc_username | password != ViewModelData.g.Sucai_rrsc_password) { Login(); }
				if (DateTime.Now.Subtract(createDateTime).Ticks > TimeSpan.FromDays(1).Ticks) { Login(); }
				if (!success) { Login(); }
				if (cookie == "") { return new MeiniguiItem() { Message = "登入失败" }; }
				if (Cookies.Count == 0) { return new MeiniguiItem() { Message = "登入失败" }; }
				if (Cookies.Where(w => String.IsNullOrEmpty(w.Value)).Any()) { return new MeiniguiItem() { Message = "登入失败(Cookies值为空)" }; }
				MeiniguiItem mi = null;
				RestClient client = new RestClient("http://www.meinigui.com/");
				RestRequest request = new RestRequest($"/site/down.html", Method.POST);
				client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
				client.Encoding = Encoding.GetEncoding("gb2312");
				request.AddHeader("Referer", @"http://www.meinigui.com/");
				//request.AddHeader("Cookie", cookie);
				request.AddHeader("X-Requested-With", "XMLHttpRequest");
				request.AddHeader("Accept-Encoding", "gzip, deflate");
				request.AddHeader("DNT", "1");
				request.AddHeader("Content-Type", "application/x-www-form-urlencoded;");
				request.AddParameter("url", url);
				//request.AddHeader("X-CSRF-Token", Cookies.Where(w=>w.Name.Contains("_identity-frontend")).FirstOrDefault()?.Value);
				//Cookies.ToList().ForEach(f => request.AddCookie(f.Name.Replace("_identity-frontend", "_csrf-frontend"), f.Value));
				Cookies?.Where(w => w.Name == "PHPSESSID" && !String.IsNullOrEmpty(w.Value)).ToList().ForEach(f => request.AddCookie(f.Name, f.Value));
				//var tcs_0 = new TaskCompletionSource<IRestResponse>();
				//client.ExecuteAsync(request, (r) =>
				//{
				//    tcs_0.SetResult(r);
				//});
				//SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 20000);

				Uri baseAddress = new Uri("http://www.meinigui.com");
				CookieContainer cookieContainer = new CookieContainer();
				HttpResponseMessage response = null;

				using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
				{
					using (var client_0 = new HttpClient(handler) { BaseAddress = baseAddress })
					{
						client_0.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");
						client_0.DefaultRequestHeaders.Add("Referer", "http://www.meinigui.com/");
						if (Cookies != null)
						{
							cookieContainer.Add(baseAddress, new Cookie("PHPSESSID", Cookies.Where(w => w.Name == "PHPSESSID" && !String.IsNullOrEmpty(w.Value)).FirstOrDefault()?.Value));
						}
						var content = new FormUrlEncodedContent(new[]
						{
						new KeyValuePair<string, string>("url", url),
						});
						response = client_0.PostAsync("/site/down.html", content).Result;
					}
				}

				//if (tcs_0.Task.IsCompleted)
				if (response != null)
				{
					response.Content.Headers.ContentType.CharSet = "gbk";
					string pan = response.Content.ReadAsStringAsync().Result;
					string forRegex = "";
					if (pan.Contains("{") & pan.Contains("}"))
					{
						int first = 0;
						int last = pan.Length;
						first = pan.IndexOf("{");
						last = pan.LastIndexOf("}");
						last++;
						if (last > first)
						{
							pan = pan.Substring(first, last - first);
						}
						try
						{
							mi = new MeiniguiItem();
							mi.Message = "解码失败";
							var d = JsonConvert.DeserializeObject<MeiniguiDetail>(pan, new JsonSerializerSettings
							{
								NullValueHandling = NullValueHandling.Ignore,
								MissingMemberHandling = MissingMemberHandling.Ignore,
							});
							string s = RestSharp.Extensions.MonoHttp.HttpUtility.HtmlDecode(d.msg);
							mi.Message = s;

							//  var d = JObject.Parse(pan);
							//  string s = HttpUtility.HtmlDecode(d["msg"].Value<string>());
							forRegex = s;
							//     Regex regex_0 = new Regex("id=(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
							Regex regex_0 = new Regex(@"file.html\?id=[A-Za-z0-9+/]*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
							Regex regex_pwd = new Regex(@"[解]?[压]?密码[为]?[（）A-Za-z0-9/.-]*[：: ]{0,1}(\r\n|\n\r|\n|\r|)([A-Za-z0-9\.\-]{1,60})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
							Regex regex_isChinese = new Regex("[\u3000\u3400-\u4DBF\u4E00-\u9FFF].*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
							//    Regex regex_baidupan = new Regex(@"(http[s]{0,1}:\/\/pan.baidu.com\/s\/[A-Za-z0-9_-]{7,24}).*(密码|提取码).*([A-Za-z0-9]{4})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
							Regex regex_baidupan = new Regex(@"(http[s]{0,1}:\/\/pan.baidu.com\/s\/[A-Za-z0-9_-]{7,24})([  　:：]{0,2}.[\u3000\u3400-\u4DBF\u4E00-\u9FFF:： ]{0,3}[ 　:：]{0,2})?([A-Za-z0-9]{4})?", RegexOptions.IgnoreCase | RegexOptions.Multiline);
							Regex regex_baidupan_only = new Regex(@"(http[s]{0,1}:\/\/pan.baidu.com\/s\/[A-Za-z0-9_-]{7,24})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
							Regex regex_baidupan_only2 = new Regex(@"(http[s]{0,1}:\/\/pan.baidu.com\/share\/[A-Za-z0-9_\-\?=&]{7,35})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
							Regex regex_ctpan = new Regex(@"(http[s]{0,1}:\/\/renrensucai.ctfile.com\/fs\/[A-Za-z0-9\-]{1,20})");
							Regex regex_filename = new Regex(@"<a href=""\/site\/file.html[ A-Za-z0-9=""/.?({':,})_]*>(.*)<\/a>|<a href=.\/site\/file.html.id=[A-Za-z0-9=&]*.*onmouseover.*_blank.>(.*)<\/a>|<em>文件名称:<\/em>(.*)<\/span>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
							Regex regex_pwd2 = new Regex(@"<div [A-Za-z0-9""=]{5,20}padding[A-Za-z0-9""=;:]{5,20}>([A-Za-z0-9-=_]{1,20})<\/div>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
							Regex regex_yunpan = new Regex(@"(http[s]{0,1}:\/\/yunpan.cn\/[A-Za-z0-9_-]{7,24}).*(密码|提取码).*([A-Za-z0-9]{4})", RegexOptions.IgnoreCase | RegexOptions.Multiline);
							Regex regex_fileurl = new Regex(@"<a href=.\/site\/(file.html\?id=[0-9A-Za-z]*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);


							Match m_0 = regex_0.Match(s);
							var doc_0 = new HtmlDocument() { OptionReadEncoding = false };
							doc_0.LoadHtml(s);

							try
							{
								HtmlNodeCollection hmc = doc_0.DocumentNode.SelectNodes("//a[@id[starts-with(.,\"ed2k_\")]]");
								foreach (HtmlNode node in hmc)
								{
									string ed2k = node.GetAttributeValue("href", "");
									if (ed2k != null)
									{
										mi.Ed2k.TryAdd(ed2k, node.InnerText);
									}
								}
							}
							catch (Exception ex) { Debug.WriteLine(ex); }

							Match m_1 = regex_pwd.Match(doc_0.DocumentNode.InnerText);
							forRegex += doc_0.DocumentNode.InnerText;
							MatchCollection m_fm = regex_filename.Matches(s);

							if (m_1.Success)
							{
								if (m_1.Groups.Count > 1)
								{
									mi.UnzipPassword = m_1.Groups[2].Value;
								}
							}

							if (m_0.Success && m_0.Groups.Count == 1)
							{

								using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
								{
									using (var client_0 = new HttpClient(handler) { BaseAddress = baseAddress })
									{
										client_0.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");
										client_0.DefaultRequestHeaders.Add("Referer", "http://www.meinigui.com/");
										cookieContainer.Add(baseAddress, new Cookie("PHPSESSID", Cookies.Where(w => w.Name == "PHPSESSID" && !String.IsNullOrEmpty(w.Value)).FirstOrDefault()?.Value));

										response = client_0.GetAsync($"site/{m_0.Groups[0].Value}").Result;
									}
								}
								int step = -1;

								foreach (Match m_f in m_fm)
								{
									step++;
									string url_file = m_0.Groups[0].Value;
									string fileName = "";

									if (m_f.Success && m_f.Groups.Count == 4)
									{
										fileName = m_f.Groups[1].Value.Trim() != "" ? m_f.Groups[1].Value.Trim() :
														  m_f.Groups[2].Value.Trim() != "" ? m_f.Groups[2].Value.Trim() :
														  m_f.Groups[3].Value.Trim();
									}

									Match m_01 = regex_0.Match(m_f.Groups[0].Value);
									if (m_01.Groups[0].Value != "")
									{
										url_file = m_01.Groups[0].Value;
									}
									else
									{
										MatchCollection mm = regex_fileurl.Matches(s);
										url_file = mm.Count >= step ? mm[step].Groups[1].Value : "";
									}

									if (m_f.Success && m_f.Groups.Count == 4 && (m_f.Groups[1].Value.Trim() != "" | m_f.Groups[2].Value.Trim() != "" | m_f.Groups[3].Value.Trim() != "")
									&& (!m_f.Groups[1].Value.Trim().Contains(".txt") && !m_f.Groups[2].Value.Trim().Contains(".txt") && !m_f.Groups[3].Value.Trim().Contains(".txt")))//IsFile
									{
										mi.IsFile = true;
										mi.NeedToDownload = true;
										mi.DownloadUrl = $"http://www.meinigui.com/site/{url_file}";
										mi.FileName = fileName;
										if (!mi.FileName.Contains(".")) { mi.FileName += ".txt"; }
										List<string> tmplist = new List<string>() { ".zip", ".rar", ".tar", ".7z" };
										if (response.Content.Headers.ContentLength < 2048 && tmplist.Contains(Path.GetExtension(mi.FileName).ToLower())) //< 2KB
										{
											byte[] content = response.Content.ReadAsByteArrayAsync().Result;
											using (MemoryStream stream = new MemoryStream(content))
											{
												using (MemoryStream stream2 = new MemoryStream())
												{
													try
													{
														var archive = ArchiveFactory.Open(stream);
														if (archive.IsComplete)
														{
															string stmp = "";
															foreach (var entry in archive.Entries)
															{
																if (!entry.IsDirectory)
																{
																	if (Path.GetExtension(entry.Key) == ".txt")
																	{
																		entry.WriteTo(stream2);
																		stmp = Encoding.GetEncoding("gb2312").GetString(stream2.ToArray());
																		stmp += Encoding.ASCII.GetString(stream2.ToArray());
																		stmp += Encoding.UTF8.GetString(stream2.ToArray());

																		forRegex += Environment.NewLine + stmp;

																		if (Regex.Matches(stmp, Regex.Escape(Environment.NewLine)).Count < 3)
																		{
																			if ((stmp.Contains("rr-sc.com-") && stmp.Split('\n').Length < 2) || (m_1.Groups[0].Value == "解压密码.txt"))
																			{
																				mi.UnzipPassword += $"{Environment.NewLine}{stmp}";
																			}
																			else
																			{
																				mi.ReferenceNote = stmp;
																			}
																		}

																		MatchCollection m_2 = regex_pwd.Matches(stmp);
																		foreach (Match m_2_f in m_2)
																		{
																			if (m_2_f.Success)
																			{
																				if (m_2_f.Groups.Count > 1)
																				{
																					mi.UnzipPassword = m_2_f.Groups[2].Value;
																					mi.NeedToDownload = false;
																				}
																			}
																		}
																	}
																}
															}
														}
													}
													catch { }
												}
											}
										}
									}
									else
									{
										client = new RestClient($"http://www.meinigui.com/site/{url_file}");
										client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
										request = new RestRequest(Method.GET);
										request.AddHeader("Referer", @"http://www.meinigui.com/");
										//request.AddHeader("Cookie", cookie);
										Cookies?.Where(w => w.Name == "PHPSESSID").ToList().ForEach(f => request.AddCookie(f.Name, f.Value));
										var tcs_1 = new TaskCompletionSource<IRestResponse>();
										client.ExecuteAsync(request, (rp) =>
										{
											tcs_1.SetResult(rp);
										});

										SpinWait.SpinUntil(() => tcs_1.Task.IsCompleted, 20000);
										if (tcs_1.Task.IsCompleted)
										{
											string Content_0 = RestSharp.Extensions.MonoHttp.HttpUtility.HtmlDecode(System.Text.Encoding.GetEncoding("gb2312").GetString(tcs_1.Task.Result.RawBytes));

											if (Regex.Matches(Content_0, Regex.Escape(Environment.NewLine)).Count < 3)
											{
												if ((Content_0.Contains("rr-sc.com-") && Content_0.Split('\n').Length < 2) || (m_1.Groups[0].Value == "解压密码.txt"))
												{
													mi.UnzipPassword += $"{Environment.NewLine}{Content_0}";
												}
												else
												{
													mi.ReferenceNote = Content_0;
												}
											}
											if (!regex_isChinese.Match(Content_0).Success)
											{
												if (!Content_0.Contains("<head><title>404 Not Found</title></head>"))
												{
													mi.UnzipPassword = Content_0;
												}
											}
											else
											{
												forRegex += Environment.NewLine + Content_0;

												MatchCollection m_2 = regex_pwd.Matches(Content_0);
												foreach (Match m_2_f in m_2)
												{
													if (m_2_f.Success)
													{
														if (m_2_f.Groups.Count > 1)
														{
															mi.UnzipPassword = m_2_f.Groups[2].Value;
														}
													}
												}
											}
										}
										else
										{
											mi.Message = "下载文件失败";
										}
									}
								}
							}

							forRegex = Regex.Replace(forRegex, "http", $"{Environment.NewLine}http", RegexOptions.IgnoreCase);

							MatchCollection m_3 = regex_baidupan.Matches(forRegex);
							if (m_3.Count > 0)
							{
								foreach (Match match in m_3)
								{
									GroupCollection g = match.Groups;
									if (g.Count == 4)
									{
										mi.BaiduPan.AddOrUpdate(g[1].Value, g[3].Value, (key, oldValue) => g[3].Value);
									}
								}
							}

							MatchCollection m_3_0 = regex_yunpan.Matches(forRegex);
							if (m_3_0.Count > 0)
							{
								foreach (Match match in m_3_0)
								{
									GroupCollection g = match.Groups;
									if (g.Count == 4)
									{
										mi.YunPan.TryAdd(g[1].Value, g[3].Value);
									}
								}
							}

							MatchCollection m_3_1 = regex_baidupan_only.Matches(forRegex);
							if (m_3_1.Count > 0)
							{
								foreach (Match match in m_3_1)
								{
									GroupCollection g = match.Groups;
									if (g.Count == 2)
									{
										mi.BaiduPan.TryAdd(g[1].Value, "");
									}
								}
							}

							MatchCollection m_3_2 = regex_baidupan_only2.Matches(forRegex);
							if (m_3_2.Count > 0)
							{
								foreach (Match match in m_3_2)
								{
									GroupCollection g = match.Groups;
									if (g.Count == 2)
									{
										mi.BaiduPan.TryAdd(g[1].Value, "");
									}
								}
							}


							MatchCollection m_4 = regex_ctpan.Matches(forRegex);
							if (m_4.Count > 0)
							{
								foreach (Match match in m_4)
								{
									GroupCollection g = match.Groups;
									if (g.Count == 2)
									{
										if (mi.CtPan.FindAll(f => f == g[1].Value).Count == 0)
										{
											mi.CtPan.Add(g[1].Value);
										}
									}
								}
							}


							if (mi.UnzipPassword == "")
							{
								Match m_5 = regex_pwd2.Match(forRegex);
								if (m_5.Success)
								{
									if (m_5.Groups.Count > 1)
									{
										mi.UnzipPassword = m_5.Groups[1].Value;
									}
								}
							}
						}
						catch { }
					}
				}
				else { mi = new MeiniguiItem() { Message = "连线失败" }; }
				return mi;
			}
			catch (Exception ex) { return new MeiniguiItem() { Message = ex.ToString() }; }
		}

	}

}
