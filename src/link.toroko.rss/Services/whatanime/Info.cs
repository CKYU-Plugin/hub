using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Robot.API;
using Robot.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.bangumianimesearch.Services.whatanime
{

    public class Info
    {
        public void Get (long anilist_id, Action<InfoResponse> Action)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var client = new RestSharp.RestClient("https://whatanime.ga/");
                client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile/14A403 Safari/602.1";

                var request = new RestSharp.RestRequest($"/info?anilist_id={anilist_id}", Method.GET);
                request.RequestFormat = DataFormat.Json;
                client.ExecuteAsync(request, (response) =>
                {
                    try
                    {
                        string json = response.Content;
                        json = json.Substring(1, json.Length - 2);
                        //      json = System.Text.RegularExpressions.Regex.Unescape(json);
                        //      json = json.Replace(@"\", string.Empty).Trim(new char[] { '\"' });
                        var data = JsonConvert.DeserializeObject<InfoResponse>(json, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                        });
                        Action(data);
                        return;
                    }
                    catch(Exception ex) { Console.WriteLine(ex.ToString()); Action(null); }
                });
            }
            catch(Exception ex) { CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_INFO, "Services.whatanime.Info.Get", ex.Message); }

        }
    }
}
