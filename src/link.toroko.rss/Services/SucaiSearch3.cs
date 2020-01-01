using link.toroko.rsshub.Services.Sucai2;
using Robot.API;
using Robot.Code;
using Robot.Property;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Data;

namespace link.toroko.rsshub.Services
{
	public class SucaiSearch3 : IServices
	{
		public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> args = null)
		{
			if (!ViewModelData.g.BEnable_Pupuyy) { _content = string.Empty; return false; }
			Uri uriResult;
			StringBuilder sb = new StringBuilder();
			sb.Append(_Code.Code_At(qq));
			sb.AppendLine();
			bool result = Uri.TryCreate(msgContent.Trim(), UriKind.Absolute, out uriResult);
			if (result)
			{
				//StringBuilder sb1 = new StringBuilder();
				//sb1.Append(_Code.Code_At(qq));
				//sb1.AppendLine();
				//sb1.Append("");
				//if (long.TryParse(gdid, out long gid))
				//{
				//	CQAPI.SendGroupMessage(RobotBase.CQ_AuthCode, gid, sb1.ToString());
				//}

				Result ss = link.toroko.rsshub.Services.Sucai2.Sucai2.Search(uriResult.GetLeftPart(UriPartial.Path));
				if (ss == null)
				{
					sb.Append("发生错误，请重试。");
				}
				else
				{
					if (!ss.Success)
					{
						sb.Append(ss.Reason);
					}
					else
					{
						if (ss.Data.urlList.Any())
						{
							sb.Append(ss.Sources);
							for (int index = 0; index < ss.Data.urlList.Count(); index++)
							{
								if (ss.Data.urlList[index].StartsWith("+"))
								{
									ss.Data.urlList[index] = ss.Data.urlList[index].Substring(1);
								}
								if (ss.Data.urlList[index].StartsWith("//"))
								{
									ss.Data.urlList[index] = ss.Data.urlList[index].Insert(0, "http:");
								}
								ss.Data.urlList[index] = Hideurl.GetShort(ss.Data.urlList[index].Replace(Environment.NewLine,String.Empty));
							}

							if (ss.Data.urltxt.Any())
							{
								if (ss.Data.urlList.Count() == ss.Data.urltxt.Count())
								{
									var dic = ss.Data.urltxt.Zip(ss.Data.urlList, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

									if (dic.Count() == 2 && dic.ElementAt(0).Value == dic.ElementAt(1).Value)
									{
										sb.Append(ss.Data.urlList[0]);
									}
									else
									{
										foreach(var d in dic)
										{
											sb.AppendLine();
											sb.AppendFormat("{0}{1}{2}",
												System.Text.RegularExpressions.Regex.Unescape(d.Key), Environment.NewLine, d.Value);
										}
									}
								}
								else
								{
									for (int index = 0; index < ss.Data.urltxt.Count(); index++)
									{
										if (index < ss.Data.urlList.Count())
										{
											sb.AppendLine();
											sb.AppendFormat("{0}:{1}",
												System.Text.RegularExpressions.Regex.Unescape(ss.Data.urltxt[index]),
												ss.Data.urlList[index]);
										}
										else
										{
											sb.AppendLine();
											sb.Append(ss.Data.urlList[index]);
										}
									}

									if (ss.Data.urlList.Count() > ss.Data.urltxt.Count())
									{
										int total = ss.Data.urlList.Count();
										int idx = ss.Data.urltxt.Count() + 1;
										for (int index = idx; index < total; index++)
										{
											sb.AppendLine();
											sb.Append(ss.Data.urlList[index]);
										}
									}
								}
							}
							else
							{
								ss.Data.urlList.ToList().ForEach(f =>
								{
									sb.AppendLine();
									sb.Append(f);
								});
							}
						}
						else
						{
							sb.Append("没找到符合的搜索结果");
						}
					}
				}
			}
			else
			{
				sb.Append("链接不正确");
			}
			_content = sb.ToString();
			return true;
		}

		public bool Reg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null)
		{
			if (!ViewModelData.g.BEnable_Pupuyy) { _content = string.Empty; return false; }

			StringBuilder sb = new StringBuilder();
			sb.Append(_Code.Code_At(qq));
			sb.AppendLine();
			//Stopwatch sw = new Stopwatch();
			//sw.Start();
			Result ss = link.toroko.rsshub.Services.Sucai2.Sucai2.SetKey(new Action(() =>
			{
				StringBuilder sb1 = new StringBuilder();
				sb1.Append(_Code.Code_At(qq));
				sb1.AppendLine();
				sb1.Append("正在加载…");
				if (long.TryParse(gdid, out long gid))
				{
					CQAPI.SendGroupMessage(RobotBase.CQ_AuthCode, gid, sb1.ToString());
				}
			}));

			if (ss.Success)
			{
				sb.Append("加载KEY数据已经完成");
				sb.AppendLine();
				sb.Append(Sucai2.Sucai2.GetMessage());
				//sb.AppendLine();
				//sb.AppendFormat("{0}：{1}", "耗时", sw.Elapsed.ToString(@"m\:ss"));
			}
			else
			{
				sb.Append(ss.Reason);
			}
			_content = sb.ToString();
			return true;
		}

		public bool Start(string qq, string gdid, string msgContent, int messageid, out string _content)
		{
			throw new NotImplementedException();
		}

		public bool Status(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> args = null)
		{
			if (!ViewModelData.g.BEnable_Pupuyy) { _content = string.Empty; return false; }
			StringBuilder sb = new StringBuilder();
			ConcurrentDictionary<string, long> dict = new ConcurrentDictionary<string, long>();

			foreach (var keys in Sucai2.Sucai2.Keys)
			{
				keys.Value.ForEach(f =>
				{
					dict.TryGetValue(f.Name, out long t);
					long c = t + f.Outstanding;
					dict.AddOrUpdate(f.Name, c, (k, v) => c);
				});
			}

			sb.Append(_Code.Code_At(qq));
			sb.AppendLine();

			if (Sucai2.Sucai2.Keys.Any())
			{
				sb.Append($"目前共有{Sucai2.Sucai2.Keys.Count}项KEY");
				sb.AppendLine();
			}

			if (dict.IsEmpty)
			{
				sb.Append("没找到任何数据，请确保已载入KEY列表。");
			}
			else
			{
				sb.Append("预计剩余用量:");
			}
			foreach (var d in dict)
			{
				sb.AppendLine();
				sb.AppendFormat("{0}:{1}", d.Key, d.Value);
			}

			_content = sb.ToString();
			return true;
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
