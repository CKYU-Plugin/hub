using link.toroko.rsshub.Services.Ezhanzy;
using Robot.Code;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wpf.Data;

namespace link.toroko.rsshub.Services
{
    class EzhanzySearch : IServices
    {
        public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> args = null)
        {
            if (!ViewModelData.g.BEnable_Eezhanzy) { _content = string.Empty; return false; }
            ConcurrentDictionary<int, EzhanzyData> dict = new ConcurrentDictionary<int, EzhanzyData>();
            ConcurrentDictionary<int, EzhanzyData> dict_current = null;
            string keyWord = msgContent;
            int page = 1;
            int totalPage = 1;
            string srow = msgContent;
            bool GetData = false;
            string result = _Code.Code_At(qq);

            if (args != null)
            {
                args.TryGetValue("Keyword", out keyWord);
                args.TryGetValue("Page", out page);
                args.TryGetValue("TotalPage", out totalPage);
                args.TryGetValue("Dict", out dict);

                args.TryGetValue("GetData", out GetData);
            }

            if (!GetData)
            {
                int times = 0;
                while (dict_current== null || dict_current.IsEmpty)
                {
                    times++;
                    dict_current = Ezhanzy.Ezhanzy.Search(qq, keyWord, page, out totalPage, dict);
                    if (dict_current == null || dict_current.IsEmpty) { Thread.Sleep(50); }
                    if(page>1 && times > 5) { break; }else if(times > 2) { break; }
                }

                result += $"{Environment.NewLine}第{page}页/共{totalPage}页{Environment.NewLine}";

                if (dict_current == null)
                {
                    result = _Code.Code_At(qq);
                    result += "连线失败。";
                    _content = result;
                    return true;
                }
                else
                {
                    foreach (var d in dict_current?.OrderBy(o => o.Key))
                    {
                        //   index++;
                        result += $"[{d.Key}]{d.Value.Title}{Environment.NewLine}";
                    }

                    result += $"输入 '<' 上一页 '>' 下一页 及 '^序号' 选取。";
                }
            }

            if (dict.Count == 0)
            {
                result = _Code.Code_At(qq);
                result += "找不到相关资源。";
                _content = result;
                return true;
            }

            if (page == 1 && totalPage == 1 && dict.Count == 1)
            {
                result = _Code.Code_At(qq);
                //get
            }
            else if (!GetData)
            {
                _content = result;
                return true;
            }

            List<int> rows = new List<int>();


            if (GetData)
            {
                if (srow.Contains(","))
                {
                    srow.Split(',').ToList().ForEach(f =>
                    {
                        if (int.TryParse(f.Replace("^","").Replace("︿", ""), out int r))
                        {
                            rows.Add(r);
                        }
                    });
                }
                else
                {
                    if (int.TryParse(srow, out int r))
                    {
                        rows.Add(r);
                    }
                }
            }
            else
            {
                rows.Add(1);
            }

            foreach (int row in rows)
            {
                Ezhanzy.EzhanzyItem ei = new Ezhanzy.EzhanzyItem();
                try
                {
                    dict.TryGetValue(row, out EzhanzyData ed);
                    if (ed != null)
                    {
                        ei = Ezhanzy.Ezhanzy.GetData(ed.Url);
                        result += $"{Environment.NewLine}[{row}]{ed.Title}";
                    }
                    else
                    {
                        result += $"{Environment.NewLine}[{row}]为无效的序号";
                        continue;
                    }
                }
                catch (Exception ex) { Debug.WriteLine(ex); }


                ei.BaiduPan?.ToList().ForEach(f =>
                {
                    result += $"{Environment.NewLine}{f.Key}";
                    if (!String.IsNullOrEmpty(f.Value))
                    {
                        result += $"提取码{f.Value}";
                    }
                });

				ei.CtPan?.ToList().ForEach(f =>
				{
					result += $"{Environment.NewLine}{f}";
				});

				ei.YunPan?.ToList().ForEach(f =>
				{
					result += $"{Environment.NewLine}{f.Key}";
					if (!String.IsNullOrEmpty(f.Value))
					{
						result += $"提取码{f.Value}";
					}
				});

				ei.Ed2k?.ToList().ForEach(f =>
                {
                    result += $"{Environment.NewLine}{f.Value}{Environment.NewLine}{f.Key}";
                });

                if (!String.IsNullOrEmpty(ei.UnzipPassword))
                {
                    result += $"{Environment.NewLine}解压密码：{ei.UnzipPassword}";
                }

                if (!String.IsNullOrEmpty(ei.ReferenceData))
                {
                    result += $"{Environment.NewLine}参考资料：{ei.ReferenceData}";
                }


                if ((ei.BaiduPan == null || ei.BaiduPan.IsEmpty) && ei.Ed2k.IsEmpty && ei.CtPan.Count==0 && ei.YunPan.IsEmpty && String.IsNullOrEmpty(ei.UnzipPassword) && String.IsNullOrEmpty(ei.ReferenceData))
                {
                    if (String.IsNullOrEmpty(ei.Message))
                    {
                        result += $"{Environment.NewLine}[{row}]提取失败";
                    }
                    else
                    {
                        result += $"{Environment.NewLine}[{row}]{ei.Message}";
                    }
                }
            }

            _content = result;

            if (rows.Count==0)
            {
                _content = null;
            }
            return GetData ? false : true;
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
