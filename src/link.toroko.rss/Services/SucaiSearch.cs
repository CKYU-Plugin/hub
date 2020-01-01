using Robot.API;
using Robot.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wpf.Data;

namespace link.toroko.rsshub.Services
{
    public class SucaiSearch : IServices
    {
        public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> args = null)
        {
            if (!ViewModelData.g.BEnable_Meinigui) { _content = string.Empty; return false; }
            string content = _Code.Code_At(qq);

            try
            {
                rrsc.MeiniguiItem i = null;
                Program.RequestArgs RA = new Program.RequestArgs();

                if (args != null)
                {
                    object tmp;
                    if (args.TryGetValue("MeiniguiItem", out tmp))
                    {
                        i = tmp as rrsc.MeiniguiItem;
                        i = link.toroko.rsshub.Services.rrsc.Meinigui.GetFile(i);
                        if (i != null)
                        {
                            content += $"{Environment.NewLine}已下载资源：{i.FileName}";
                        }
                        else
                        {
                            content += $"{Environment.NewLine}下载资源失败";
                        }
                        _content = content;

                        if (args.TryGetValue("UpdateContent", out tmp))
                        {
                            RA.Service = this;
                            RA.Args = new Dictionary<string, object>();
                            RA.Args.Add("UpdateContent", tmp);
                            Program.pandingEnd.TryAdd(qq, RA);
                        }
                        return true;
                    }
                    if (args.TryGetValue("UpdateContent", out tmp))
                    {
                        msgContent = tmp.ToString();
                    }
                }

                //single
                if (msgContent.ToLower().Contains(",title"))
                {
                    try
                    {
                        msgContent = "https://www.rr-sc.com/" + msgContent;
                        msgContent = msgContent.Substring(0, msgContent.LastIndexOf(",title"));
                    }
                    catch { }
                }
                else
                {
                    if (!msgContent.Trim().StartsWith("https://www.rr-sc.com/") || !msgContent.Trim().StartsWith("http://www.rr-sc.com/"))
                    {
                        msgContent = "https://www.rr-sc.com/" + msgContent;
                    }
                }
                //muiltply
                if (Regex.Matches(msgContent.ToLower(), "http").Count > 1)
                {
                    List<string> lines = msgContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
                    if (lines.Count > 0)
                    {
                        if (RA.Args == null)
                        {
                            RA.Service = this;
                            RA.Args = new Dictionary<string, object>();
                        }

                        RA.Args.Add("UpdateContent",
                            String.Join(Environment.NewLine, msgContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList().Skip(1).ToArray()));
                        msgContent = lines[0];
                    }
                }

                try
                {
                    i = link.toroko.rsshub.Services.rrsc.Meinigui.GetData(msgContent.Trim());
                }
                catch { }

                if (i == null)
                {
                    content += $"请发送正确的素材链接{Environment.NewLine}附加信息：{i.Message}";
                }
                else
                {
                    if (i.BaiduPan.Count == 0 & i.CtPan.Count() == 0 & i.YunPan.Count() == 0 & i.Thunder.Count() == 0 & (i.UnzipPassword == null | i.UnzipPassword?.Trim() == "") && i.IsFile == false)
                    {
                        content += $"发生错误{Environment.NewLine}原始信息：{i.Message}";
                        if (i.Ed2k.Count == 0)
                        {
                            Regex regex_ed2k = new Regex(@"(ed2k:..\|file\|(.*)\|([0-9]{1,12})\|([0-9a-fA-F]{32})\|(.*)\|\/)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            MatchCollection mc = regex_ed2k.Matches(i.Message);
                            if (mc.Count > 0)
                            {
                                content = _Code.Code_At(qq);
                                int row = 0;
                                foreach (Match m in mc)
                                {
                                    row++;
                                    if (m.Success)
                                    {
                                        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"AA{m.Groups[0].Value}ZZ");
                                        string thunder = $"thunder://{System.Convert.ToBase64String(plainTextBytes)}";
                                        content += $"{Environment.NewLine}{thunder}";
                                        content += $"{Environment.NewLine}[{row}]{m.Groups[2].Value}{Environment.NewLine}{thunder}";
                                    }
                                }
                            }
                        }
                        else
                        {
                            content = _Code.Code_At(qq);
                            int row = 0;
                            foreach(var tmp in i.Ed2k)
                            {
                                row++;
                                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"AA{tmp.Key}ZZ");
                                string thunder = $"thunder://{System.Convert.ToBase64String(plainTextBytes)}";
                                string filename = tmp.Value;
                                Regex regex_ed2k = new Regex(@"(ed2k:..\|file\|(.*)\|([0-9]{1,12})\|([0-9a-f]{32})\|(.*)\|\/)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                Match m = regex_ed2k.Match(tmp.Key);
                                if (m.Success)
                                {
                                    filename = m.Groups[2].Value + tmp.Value.Substring(tmp.Value.LastIndexOf("(") - 1);
                                }
                                content += $"{Environment.NewLine}[{row}]{filename}{Environment.NewLine}{thunder}";
                            }
                        }
                        if (i.Message.Contains("请先登录")) { rrsc.Meinigui.success = false; }
                    }
                    else
                    {
                        i.YunPan.ToList().ForEach(f =>
                        {
                            string strpw = f.Value == "" ? "" : $"密码{f.Value}";
                            content += $"{Environment.NewLine}{f.Key}{strpw}";
                        });

                        i.BaiduPan.ToList().ForEach(f =>
                        {
                            string strpw = f.Value == "" ? "" : $"密码{f.Value}";
                            content += $"{Environment.NewLine}{f.Key}{strpw}";
                        });

                        i.CtPan.ForEach(f =>
                        {
                            content += $"{Environment.NewLine}{f}";
                        });

                        if (i.BaiduPan.Count == 0 && i.CtPan.Count == 0)
                        {
                            rrsc.MeiniguiItem ii = rrsc.Meinigui.GetData2(msgContent.Trim());
                            if (ii != null)
                            {
                                ii.YunPan?.ToList().ForEach(f =>
                                {
                                    string strpw = f.Value == "" ? "" : $"密码{f.Value}";
                                    content += $"{Environment.NewLine}{f.Key}{strpw}";
                                });

                                ii.BaiduPan?.ToList().ForEach(f =>
                                {
                                    string strpw = f.Value == "" ? "" : $"密码{f.Value}";
                                    content += $"{Environment.NewLine}{f.Key}{strpw}";
                                });

                                ii.CtPan?.ForEach(f =>
                                {
                                    content += $"{Environment.NewLine}{f}";
                                });

                                ii.Thunder?.ForEach(f =>
                                {
                                    content += $"{Environment.NewLine}{f}";
                                });

                                if (ii.UnzipPassword != null && ii.UnzipPassword?.Trim() != "")
                                {
                                    content += $"{Environment.NewLine}解压密码：{ii.UnzipPassword}";
                                }

                                if (ii.ReferenceNote != null && ii.ReferenceNote?.Trim() != "")
                                {
                                    content += $"{Environment.NewLine}参考资料：{ii.ReferenceNote}";
                                }
                            }
                        }

                        if (i.UnzipPassword != null && i.UnzipPassword?.Trim() != "")
                        {
                            content += $"{Environment.NewLine}解压密码：{i.UnzipPassword}";
                        }
                        if (i.ReferenceNote != null && i.ReferenceNote?.Trim() != "")
                        {
                            content += $"{Environment.NewLine}参考资料：{i.ReferenceNote}";
                        }
                    }
                }

                if (i.NeedToDownload)
                {
                    content += $"{Environment.NewLine}发现可下载资源，正在下载…";
                    if (RA.Args == null)
                    {
                        RA.Service = this;
                        RA.Args = new Dictionary<string, object>();
                    }
                    RA.Args.Add("MeiniguiItem", i);
                }

                _content = content;
                if (RA.Args != null)
                {
                    Program.pandingEnd.TryAdd(qq, RA);
                }
                return true;
            }
            catch (Exception ex)
            {
                content += ex.ToString();
                _content = content;
                return true;
            }
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
