using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Robot.API;
using Robot.Property;

namespace link.toroko.rsshub.bangumianimesearch.Services.whatanime
{
    public class Search
    {

        public void GetThumbnail(string anilist_id, string filename, string at, string tokenthumb, Action<Image> act)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var client = new RestSharp.RestClient("https://trace.moe/");
                client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile/14A403 Safari/602.1";
                var request = new RestSharp.RestRequest($"thumbnail.php?anilist_id={anilist_id}&file={RestSharp.Extensions.StringExtensions.UrlEncode(filename)}&t={at}&token={tokenthumb}", Method.GET);
                Image image = null;
                client.ExecuteAsync(request, (response) =>
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (MemoryStream ms = new MemoryStream(response.RawBytes))
                        {
                            image = Image.FromStream(ms);
                            act(image);
                        }
                    }
                    else
                    {
                        act(null);
                    }
                });
            }
            catch { }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="Base64Image"></param>
        /// <param name="Action"></param>
        public void Get(string Token, string Base64Image, Action<ResponseType, SearchResponse> Action)
        {
            //   var certificates = new X509Certificate();
            //   certificates.Import(clientCertificateFilePath, clientCertificatePassword, X509KeyStorageFlags.PersistKeySet);
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var client = new RestSharp.RestClient("https://trace.moe/");
                client.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile/14A403 Safari/602.1";

                var request = new RestSharp.RestRequest($"/api/search", Method.POST);
                if (Token != "")
                {
                    request = new RestSharp.RestRequest($"/api/search?token={Token}", Method.POST);
                }

                request.AddHeader("content-type", "application/x-www-form-urlencoded; charset=UTF-8");
                request.RequestFormat = DataFormat.Json;

                string preImage = RestSharp.Extensions.StringExtensions.UrlEncode($"data:image/jpeg;base64,{Base64Image}");

                request.AddParameter("application/x-www-form-urlencoded", $"image={preImage}", ParameterType.RequestBody);

                client.ExecuteAsync(request, (response) =>
                {
                    ResponseType srt = new ResponseType();
                    srt.Code = -1;
                    srt.Message = "unhandled exception";

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        srt.Code = 200;
                        if (response.Content.Length < 2) { srt.Code = 0; }

                        try
                        {
                            var data = JsonConvert.DeserializeObject<SearchResponse>(response.Content, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore,
                            });

                            Action(srt, data);
                            return;
                        }
                        catch (Exception ex)
                        {
                            srt.Code = 0;
                            srt.Message = ex.ToString();
                            Action(srt, null);
                            return;
                        }
                    }
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // API token is missing
                        srt.Code = (int)System.Net.HttpStatusCode.Unauthorized;
                        srt.Message = "API token is missing";
                        Action(srt, null);
                        return;
                    }
                    if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        // API token is invalid
                        srt.Code = (int)System.Net.HttpStatusCode.Forbidden;
                        srt.Message = "API token is invalid";
                        Action(srt, null);
                        return;
                    }
                    if (response.StatusCode == System.Net.HttpStatusCode.RequestEntityTooLarge)
                    {
                        //encoded image over 1MB
                        srt.Code = (int)System.Net.HttpStatusCode.RequestEntityTooLarge;
                        srt.Message = "Image over 1MB";
                        Action(srt, null);
                        return;
                    }
                    if ((int)response.StatusCode == 429)
                    {
                        //quota exceeded
                        srt.Code = 429;
                        srt.Message = "Quota exceeded";
                        Action(srt, null);
                        return;
                    }

                    //error.
                    Action(srt, null);
                });
            }
            catch (Exception ex) { CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_INFO, "Services.whatanime.Search.Get", ex.Message); }
        }
    }
}
