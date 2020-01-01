using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services
{
	class Hideurl
	{
		private static Uri _baseAddress = new Uri("https://hideuri.com/");
		private static string _useragent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.103 Safari/537.36";

		public static string GetShort(string url)
		{
			HttpResponseMessage response = null;

			using (var handler = new HttpClientHandler())
			{
				using (var client_0 = new HttpClient(handler) { BaseAddress = _baseAddress })
				{
					client_0.DefaultRequestHeaders.Add("user-agent", _useragent);
					client_0.DefaultRequestHeaders.Add("Referer", _baseAddress.AbsoluteUri);
					var content = new FormUrlEncodedContent(new[]
						{
						new KeyValuePair<string, string>("url", url),
						});
					response = client_0.PostAsync("/api/v1/shorten", content).Result;
				}
			}
			if (response != null)
			{
				string shorten = response.Content.ReadAsStringAsync().Result;
				try
				{
					HideurlResult r = JsonConvert.DeserializeObject<HideurlResult>(shorten, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
					if (r != null)
					{
						return r.result_url;
					}
				}
				catch { return url; }
			}
			return url;
		}
	}

	public class HideurlResult
	{
		public string result_url { get; set; }
	}

}

