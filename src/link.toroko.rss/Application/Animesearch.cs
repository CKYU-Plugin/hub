using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Robot.API;
using Robot.Property;
using System.Text.RegularExpressions;
using Robot.Code;
using RestSharp;
using System.Drawing;
using System.Drawing.Imaging;
using me.kodone.hub.bangumianimesearch.Services.whatanime;
using System.Collections.Concurrent;
using Wpf.Data;

namespace me.kodone.hub.Application
{
    public class Animesearch
    {
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _start = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        private static bool IsTrunOn = true;

        public static void Run(string robotQQ, Int32 msgType, Int32 msgSubType, string msgSrc, string targetActive, string targetPassive, string msgContent, int messageid)
        {
            if (RobotBase.isinit && RobotBase.isenableplugin)
            {
                msgContent = msgContent.Trim();
                if (msgContent == "打開以圖搜番" && msgSrc == RobotBase.AdminQQ)
                {
                    IsTrunOn = true;
                    _API.SendMessage(msgSrc, msgType, "已打開", targetActive, robotQQ, msgSubType);
                }
                if (msgContent == "關閉以圖搜番" && msgSrc == RobotBase.AdminQQ)
                {
                    IsTrunOn = false;
                    _API.SendMessage(msgSrc, msgType, "已關閉", targetActive, robotQQ, msgSubType);
                }

                if (IsTrunOn && (msgContent.StartsWith("以圖搜番") | (_start.TryGetValue(targetActive, out ConcurrentDictionary<string, string> val) && val.Where(w => w.Key == msgSrc).Count() > 0)))
                {
                    List<string> imagecode = new List<string>();
                    foreach (Match regmatch in Regex.Matches(msgContent, _Code.Code_Image_Regex()))
                    {
                        if (regmatch.Success)
                        {
                            imagecode.Add(regmatch.Groups[0].Value);
                        }
                    }

                    if (imagecode.Count == 0)
                    {
                        if (val == null | val?.Where(w => w.Key == msgSrc).Count() == 0)
                        {
                            ConcurrentDictionary<string, string> tmp = new ConcurrentDictionary<string, string>();

                            if (val == null)
                            {

                                tmp.TryAdd(msgSrc, targetActive);
                                _start.TryAdd(targetActive, tmp);
                                val = new ConcurrentDictionary<string, string>();
                            }
                            else
                            {
                                val?.TryAdd(msgSrc, targetActive);
                            }
                            _API.SendMessage(msgSrc, msgType, "請發送一張番劇圖", targetActive, robotQQ, msgSubType);
                        }
                        return;
                    }
                    val?.TryRemove(msgSrc, out string _gdid);
                    string url = _API.Api_GuidGetPicLink(imagecode[0]);

                    if (url != "")
                    {
                        var client = new RestClient(url);
                        var request = new RestRequest("", Method.GET);
                        client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                        client.ExecuteAsync(request, (response) =>
                        {
                            try
                            {
                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    Search s = new Search();
                                    byte[] imageset = response.RawBytes;
                                    byte[] tmp = null;
                                    if (response.Content.StartsWith("GIF"))
                                    {

                                        using (var ms = new MemoryStream(response.RawBytes))
                                        {
                                            Image gifImg = Image.FromStream(ms);
                                            FrameDimension dimension = new FrameDimension(gifImg.FrameDimensionsList[0]);
                                            gifImg.SelectActiveFrame(dimension, gifImg.GetFrameCount(dimension) > 2 ? gifImg.GetFrameCount(dimension) / 2 : 0);
                                            using (Bitmap bmp = new Bitmap(gifImg))
                                            {
                                                ImageConverter converter = new ImageConverter();
                                                tmp = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
                                            }
                                        }
                                        imageset = tmp;
                                        tmp = null;
                                    }

                                    s.Get(RobotBase.WhatAnimeApiToken, Convert.ToBase64String(imageset),
                                        new Action<ResponseType, SearchResponse>((t, r) =>
                                        {
                                            try
                                            {
                                                if (t.Code == 200)
                                                {
                                                    string message = "";
                                                    if (r != null)
                                                    {
                                                        if (r.Docs.Count() > 0)
                                                        {

                                                            Info i = new Info();
                                                            i.Get(r.Docs.FirstOrDefault().Anilist_id, new Action<InfoResponse>((ir) =>
                                                            {

                                                                TimeSpan t_at = TimeSpan.FromSeconds(r.Docs.First().At);
                                                                string s_at = string.Format("{0:D2}:{1:D2}:{2:D2}", t_at.Hours, t_at.Minutes, t_at.Seconds);

                                                                try
                                                                {
                                                                    if ((string)r.Docs.First().Synonyms_chinese != @"""[]""")
                                                                    {
                                                                        var arr = Newtonsoft.Json.Linq.JArray.Parse(r.Docs.First().Synonyms_chinese.ToString());
                                                                        message += arr.First().ToString() + Environment.NewLine;
                                                                    }
                                                                }
                                                                catch { }

                                                                message += r.Docs.First().Title + Environment.NewLine;

                                                                if (ir != null)
                                                                {
                                                                    message += $"{ir.startDate.year}-{ir.startDate.month}-{ir.startDate.day}";
                                                                    if (ir.endDate.year > 0)
                                                                    {
                                                                        message += $"~{ir.endDate.year}-{ir.endDate.month}-{ir.endDate.day}";
                                                                        message += $"之間放送";
                                                                    }
                                                                    else
                                                                    {
                                                                        message += $"開始放送";
                                                                    }

                                                                    if (ir.episodes > 0)
                                                                    {
                                                                        message += $"，共{ir.episodes}話";
                                                                    }
                                                                    if (ir.duration > 0)
                                                                    {
                                                                        if (ir.episodes == 1)
                                                                        {
                                                                            message += $"(時長{ir.duration}分鐘)";
                                                                        }
                                                                        else
                                                                        {
                                                                            message += $"(每話{ir.duration}分鐘)";
                                                                        }
                                                                    }

                                                                    message += Environment.NewLine;
                                                                    message += $"評分:{ir.meanScore}";
                                                                    message += Environment.NewLine;
                                                                }

                                                                message = _Code.Code_At(msgSrc) + Environment.NewLine + message;

                                                                message += $"EP#{r.Docs.First().Episode} {r.Docs.First().Synonyms?.FirstOrDefault()} {s_at}" + Environment.NewLine;
                                                                message += string.Format("{0:P2}", r.Docs.First().Similarity) + "相似度";

																if (RobotBase.ispro)
																{
																	if (r.Docs.Count > 0)
																	{
																		Action<Image> act = new Action<Image>((thumb) =>
																		{
																			if (r.Docs.First().Is_adult && !ViewModelData.g.IsHentsaiss)
																			{
																				_API.SendMessage(msgSrc, msgType, message, targetActive, robotQQ, msgSubType);
																				return;
																			}

																			if (thumb != null)
																			{
																				message += Environment.NewLine;
																				message += $"較相似的截圖";
																				message += Environment.NewLine;
																				message += _API.Api_UploadPic(thumb, out string path);
																			}

																			_API.SendMessage(msgSrc, msgType, message, targetActive, robotQQ, msgSubType);
																		});
																		s.GetThumbnail(r.Docs.First()?.Anilist_id.ToString(), r.Docs.First()?.Filename, r.Docs.First()?.At.ToString(), r.Docs.First()?.Tokenthumb, act);
																	}
																}
																else
                                                                {
                                                                    _API.SendMessage(msgSrc, msgType, message, targetActive, robotQQ, msgSubType);
                                                                }

                                                            }));

                                                            return;
                                                        }
                                                        else
                                                        {
                                                            _API.SendMessage(msgSrc, msgType, _Code.Code_At(msgSrc) + "找不到該圖片", targetActive, robotQQ, msgSubType);
                                                            return;
                                                        }
                                                    }
                                                    _API.SendMessage(msgSrc, msgType, _Code.Code_At(msgSrc) + "解析失敗", targetActive, robotQQ, msgSubType);
                                                    return;
                                                }
                                                else
                                                {
                                                    _API.SendMessage(msgSrc, msgType, t.Message, targetActive, robotQQ, msgSubType);
                                                    return;
                                                }
                                            }
                                            catch { }
                                        }));
                                }
                            }
                            catch (Exception ex) { CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_INFO, "Program", ex.Message); }
                        });
                    }
                }
            }
        }
    }
}
