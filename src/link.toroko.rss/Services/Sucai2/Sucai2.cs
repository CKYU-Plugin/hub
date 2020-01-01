using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using Robot.Property;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.Sucai2
{
	class Sucai2
	{
		private static Uri _baseAddress = new Uri("http://114.116.29.8");
		private static string _useragent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.103 Safari/537.36";
		private static object _updating = new object();
		private static int _updateCount = 0;
		private static int _updateQCount = 0;
		private static int _lockFlag = 0;
		private static object _lockFlag_file = new object();
		private static Stopwatch _stopwatch = new Stopwatch();
		public static ConcurrentDictionary<string, List<SucaiData>> Keys = new ConcurrentDictionary<string, List<SucaiData>>();
		public static Dictionary<string, string> Services = new Dictionary<string, string>()
		{
			{ "千图","58pic.com"},
			{ "包图","ibaotu.com"},
			{ "摄图","699pic.com"},
			{ "我图","ooopic.com"},
			{ "淘图","taopic.com"},
			{ "快图","kuaipng.com" },
			{ "千库","588ku.com"},
			{ "视听","588ku.com"},
			{ "90设计","90sheji.com"},
			{ "云米","yunmiss.com"},
			{ "万素","669pic.com"},
			{ "熊猫","tukuppt.com"},
		};

		public static Result Search(string url)
		{
			List<string> name = Services.Where(w => url.Contains(w.Value)).Select(s => s.Key).ToList();
			List<string> already = new List<string>();
			try
			{
				if (name.Count > 0)
				{
					already = Keys.Where(w => w.Value.Where(ww =>
					{
						if (name.Contains(ww.Name))
						{
							if (ww.Outstanding > 0)
							{
								if (ww.Validate.Date >= DateTime.Now.Date)
								{
									return true;
								}
							}
						}
						return false;
					}).Any()).OrderByDescending(o => o.Value.Count).OrderByDescending(o => o.Value.Where(ww => name.Contains(ww.Name)).Last().Outstanding).Select(s => s.Key).ToList();
				}
				else
				{
					already = Keys.Where(w => w.Value.Where(ww => ww.Outstanding > 0).Any()).OrderByDescending(o => o.Value.Count).Select(s => s.Key).ToList();
				}

				if (already.Count == 0)
				{
					if (Keys.Count == 0)
					{
						return new Result() { Success = false, Reason = "没有可用的KEY数据，请载入KEY列表" };
					}
					return new Result() { Success = false, Reason = "所有KEY数据均已使用。" };
				}

				SucaiResult sr = new SucaiResult();

				foreach (var key in already)
				{
					//Keys.TryGetValue(key, out List<SucaiData> data);
					//if (data.Where(w => name.Contains(w.Name) && w.Outstanding > 0).Any() | name.Count == 0)
					//{
						sr = _Search(key, url);
						if (sr != null)
						{
							if (sr.result)
							{
								return new Result() { Success = true, Data = sr, Sources = name?.First() };
							}
							else
							{
								if (name.Count == 0)
								{
									return new Result() { Success = false, Reason = "可能不是支持的来源。" };
								}
								//return new Result() { Success = false, Reason = $"发生错误，({sr.message})" };
							}
						}
						else
						{
							return new Result() { Success = false, Reason = "发生错误，请重试。" };
						}
					//}
					//UpdateAndSave(key);
				};
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
				return new Result() { Success = false, Reason = "发生错误。" };
			}
			//out last result
			return new Result() { Success = false, Reason = "所有KEY均无效或无结余，请刷新KEY列表。" };
		}

		private static SucaiResult _Search(string key, string url)
		{
			CookieContainer cookieContainer = new CookieContainer();
			HttpResponseMessage response = null;
			try
			{
				using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
				{
					using (var client_0 = new HttpClient(handler) { BaseAddress = _baseAddress })
					{
						client_0.DefaultRequestHeaders.Add("user-agent", _useragent);
						client_0.DefaultRequestHeaders.Add("Referer", _baseAddress.AbsoluteUri);
						var content = new FormUrlEncodedContent(new[]
							{
						new KeyValuePair<string, string>("url", System.Web.HttpUtility.UrlEncode(url)),
						new KeyValuePair<string, string>("id", key),
						});
						response = client_0.PostAsync("/jxx.php", content).Result;
					}
				}
				if (response != null)
				{
					string data = response.Content.ReadAsStringAsync().Result;
					try
					{
						return JsonConvert.DeserializeObject<SucaiResult>(data, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
					}
					catch
					{
						if(response.Headers != null && response.Headers.Select(w=>w.Key== "Content-Length").Any())
						{
							long.TryParse(response.Headers.Where(w => w.Key == "Content-Length").First().Value.ToString(), out long l);
							if (l > 1000)
							{
								return new SucaiResult() { result = false, message = "解析失敗" };
							}
						}
						if(response.StatusCode != HttpStatusCode.OK)
						{
							return new SucaiResult() { result = false, message = $"{response.StatusCode}" };
						}
					}
				}
			}
			catch { }

			return null;
		}

		public static void Initializer()
		{
			try
			{
				if (!File.Exists(Path.Combine(RobotBase.appfolder, "Sucai", "Sucai_services.json")))
				{
					File.WriteAllText(Path.Combine(RobotBase.appfolder, "Sucai", "Sucai_services.json"), JsonConvert.SerializeObject(Services));
				}
				else
				{
					try
					{
						Services = JsonConvert.DeserializeObject<Dictionary<string, string>>(Path.Combine(RobotBase.appfolder, "Sucai", "Sucai_services.json"));
					}
					catch { }
				}
				if (File.Exists(Path.Combine(RobotBase.appfolder, "Sucai", "Sucai_Keys.json")))
				{
					try
					{
						Keys = JsonConvert.DeserializeObject<ConcurrentDictionary<string, List<SucaiData>>>(File.ReadAllText(Path.Combine(RobotBase.appfolder, "Sucai", "Sucai_Keys.json")));
					}
					catch { }
				}
			}
			catch { }
		}

		public static void Save()
		{
			lock (_lockFlag_file)
			{
				//File.WriteAllText(Path.Combine(RobotBase.appfolder, "Sucai", "Sucai_services.json"), JsonConvert.SerializeObject(Services));
				File.WriteAllText(Path.Combine(RobotBase.appfolder, "Sucai", "Sucai_Keys.json"), JsonConvert.SerializeObject(Keys));
			}
		}

		public static string GetMessage()
		{
			return $"更新了{_updateCount}项KEY数据，当中包括{_updateQCount}项剩余次数的数据刷新。{Environment.NewLine}耗时{_stopwatch.Elapsed.ToString(@"m\:ss")}";
		}

		public static Result Checkbefore()
		{
			if (Interlocked.CompareExchange(ref _lockFlag, 1, 0) != 0)
			{
				return new Result() { Success = false, Reason = $"已在更新KEY数据中，目前进度为:{Environment.NewLine}{GetMessage()}" };
			}
			else
			{
				return new Result() { Success = true, Reason = $"开始加载KEY数据" };
			}
		}

		public static Result SetKey(Action act)
		{
			if (Interlocked.CompareExchange(ref _lockFlag, 1, 0) != 0)
			{
				return new Result() { Success = false, Reason = $"已在更新KEY数据中，目前进度为:{Environment.NewLine}{GetMessage()}" };
			}
			try
			{
				act();
				_stopwatch.Restart();
				Monitor.Enter(_updating);
				CookieContainer cookieContainer = new CookieContainer();
				HttpResponseMessage response = null;

				using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
				{
					using (var client_0 = new HttpClient(handler) { BaseAddress = _baseAddress })
					{
						client_0.DefaultRequestHeaders.Add("user-agent", _useragent);
						client_0.DefaultRequestHeaders.Add("Referer", _baseAddress.AbsoluteUri);
						response = client_0.GetAsync("/scc.html").Result;
					}
				}

				if (response != null)
				{
					response.Content.Headers.ContentType.CharSet = "UTF-8";
					string _keys = response.Content.ReadAsStringAsync().Result;
					//Regex r = new Regex("");
					var doc_0 = new HtmlDocument() { OptionReadEncoding = false };
					doc_0.LoadHtml(_keys);
					//	HtmlNode node_keys = doc_0.DocumentNode.SelectNodes("//*[contains(@id,'myselect')]").FirstOrDefault();
					HtmlNode select = doc_0.GetElementbyId("myselect");
					List<HtmlNode> options = new List<HtmlNode>();

					_updateCount = 0;
					_updateQCount = 0;
					int iTotal = Math.Max(0, select.ChildNodes.Where(w => w.Name == "option" && w.HasAttributes).Count() - 1);

					for (int index = 0; index < select.ChildNodes.Count; index++)
					{
						HtmlNode option = select.ChildNodes[index];
						if (option.Name == "option")
						{
							if (option.HasAttributes)
							{
								string title = option.InnerText;
								string key = option.GetAttributeValue("value", String.Empty);
								if (!String.IsNullOrEmpty(key))
								{
									UpdateAndSave(key);
									if (Keys.TryGetValue(key, out List<SucaiData> d))
									{
										_updateQCount += d.Count;
									}
									Console.WriteLine($"已添加或更新{++_updateCount}/{iTotal}({Keys.Count})");
								}
							}
						}
					}
				}
			}
			finally
			{
				Interlocked.Decrement(ref _lockFlag);
				_stopwatch.Stop();
				Save();
				Monitor.Pulse(_updating);
				Monitor.Exit(_updating);
			}
			return new Result() { Success = true, Reason = "" };
		}

		private static void UpdateAndSave(string key)
		{
			List<SucaiData> data = CheckKey(key);
			if (data?.Count > 0)
			{
				Keys.AddOrUpdate(key, data, (k, v) => data);
				Save();
			}
		}
		public static List<SucaiData> CheckKey(string key)
		{
			CookieContainer cookieContainer = new CookieContainer();
			HttpResponseMessage response = null;

			List<SucaiData> tmp = new List<SucaiData>();

			int c = 0;
			while (true)
			{
				using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
				{
					using (var client_0 = new HttpClient(handler) { BaseAddress = _baseAddress })
					{
						client_0.Timeout = TimeSpan.FromSeconds(10);
						client_0.DefaultRequestHeaders.Add("user-agent", _useragent);
						client_0.DefaultRequestHeaders.Add("Referer", _baseAddress.AbsoluteUri);
						var content = new FormUrlEncodedContent(new[]
						{
						new KeyValuePair<string, string>("id", key),
						});
						CancellationTokenSource tokens = new CancellationTokenSource();
						try
						{
							response = client_0.PostAsync("/css.php", content, tokens.Token).Result;
						}
						catch { }
						tokens.Cancel();
					}
				}
				if(response?.StatusCode == HttpStatusCode.OK) { break; }
				if(++c > 1) { break; }
			}

			if (response != null)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				Regex r_time = new Regex("(-{0,1}[0-9]{1,3})");
				Regex r_date = new Regex("([0-9]{4}-[0-9]{2}-[0-9]{2}|永久)");

				Services.ToList().ForEach(f =>
				{
					//Key
					int d = data.IndexOf(f.Key);
					if (d > 0)
					{
						//secord Key
						int d1 = data.IndexOf(f.Key, Math.Min(d+1,data.Length));
						if (d1 > 0)
						{
							//time
							int d_1 = data.IndexOf("剩余", d);
							if (d_1 > 0)
							{
								//date
								int d1_1 = data.IndexOf("天数", d1);
								if (d1_1 > 0)
								{
									string t = data.Substring(Math.Min(d_1, data.Length));
									string t1 = data.Substring(Math.Min(d1_1, data.Length));
									Match m = r_time.Match(t);
									Match m1 = r_date.Match(t1);
									if (m.Success && m1.Success)
									{
										if (int.TryParse(m.Groups[0].Value, out int _t))
										{
											if (!DateTime.TryParse(m1.Groups[0].Value, out DateTime _d))
											{
												_d = DateTime.MaxValue;
											}
											tmp.Add(new SucaiData
											{
												Name = f.Key,
												Outstanding = Math.Max(0, _t),
												Validate = _d,
												Lastupdate = DateTime.Now,
											});
										}
									}
								}
							}
						}
					}
				});

				if (tmp.Count > 0)
				{
					return tmp;
				}
			}
			return new List<SucaiData>();
		}
	}

	public class Result
	{
		public bool Success { get; set; }
		public string Reason { get; set; }
		public SucaiResult Data { get; set; }
		public string Sources { get; set; }
	}

	public class SucaiData
	{
		public string Name { get; set; }
		public long Outstanding { get; set; }
		public DateTime Validate { get; set; }
		public DateTime Lastupdate { get; set; }
	}

	public class SucaiResult
	{
		public string message { get; set; } = String.Empty;
		public bool result { get; set; } = false;
		public int status { get; set; } = 0;
		public int type { get; set; } = 0;
		public int is_zip { get; set; } = 0;
		public string file { get; set; } = String.Empty;	
		public string[] urltxt { get; set; } = new string[] { };
		public string[] urlList { get; set; } = new string[] { };
		public string Id { get; set; } = String.Empty;
	}

}
