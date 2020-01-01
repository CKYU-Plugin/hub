using link.toroko.rsshub.startup;
using MonoTorrent;
using MonoTorrent.BEncoding;
using MonoTorrent.Client;
using MonoTorrent.Client.Encryption;
using MonoTorrent.Common;
using MonoTorrent.Dht;
using MonoTorrent.Dht.Listeners;
using RestSharp;
using Robot.Code;
using Robot.Extension;
using Robot.Property;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wpf.Data;

namespace link.toroko.rsshub.Services
{
    public class TorrentClient : IServices
    {
        static ClientEngine engine;
        static List<TorrentManager> torrents = new List<TorrentManager>();
        static TorrentSettings torrentDefaults = new TorrentSettings(4, 150, 0, 0);
        static bool isStart = false;
        static object locker = new object();
        static BEncodedDictionary fastResume = new BEncodedDictionary();
        static string path_torrents;
        static string path_downloads;
        static string path_dht;
        static string path_fastResume;

        public enum TorrentStateChinese
        {
            停止,
            暂停,
            下载中,
            做种中,
            正在整理,
            正在停止,
            错误,
            加载中
        }


        public static void Start(int port)
        {
            lock (locker)
            {
                if (!ViewModelData.g.BEnable_Torrent) { return; }
                if (isStart) { return; }
                path_torrents = Path.Combine(RobotBase.currentfloder, RobotBase.path_torrents);
                path_downloads = Path.Combine(RobotBase.currentfloder, RobotBase.path_downloads);
                path_dht = Path.Combine(RobotBase.currentfloder, RobotBase.path_dhtNodeFile);
                path_fastResume = Path.Combine(RobotBase.currentfloder, RobotBase.path_fastResumeFile);

                EngineSettings engineSettings = new EngineSettings(Path.Combine(RobotBase.currentfloder, RobotBase.path_downloads), port);
                engineSettings.PreferEncryption = false;
                engineSettings.AllowedEncryption = EncryptionTypes.All;
                TorrentSettings torrentDefaults = new TorrentSettings(5, 200, 0, 0);
                engine = new ClientEngine(engineSettings);
                engine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Any, port));
                byte[] nodes = null;
                try
                {
                    nodes = File.ReadAllBytes(Path.Combine(RobotBase.currentfloder, RobotBase.path_dhtNodeFile));
                }
                catch { }

                DhtListener dhtListner = new DhtListener(new IPEndPoint(IPAddress.Any, port));
                DhtEngine dht = new DhtEngine(dhtListner);
                engine.RegisterDht(dht);
                dhtListner.Start();
                engine.DhtEngine.Start(nodes);

                if (!Directory.Exists(engine.Settings.SavePath)) Directory.CreateDirectory(engine.Settings.SavePath);
                if (!Directory.Exists(Path.Combine(RobotBase.currentfloder, RobotBase.path_torrents))) Directory.CreateDirectory(Path.Combine(RobotBase.currentfloder, RobotBase.path_torrents));

                try
                {
                    fastResume = BEncodedValue.Decode<BEncodedDictionary>(File.ReadAllBytes(Path.Combine(RobotBase.currentfloder, RobotBase.path_fastResumeFile)));
                }
                catch
                {
                    fastResume = new BEncodedDictionary();
                }
                isStart = true;

                foreach (string file in Directory.GetFiles(path_torrents))
                {
                    if (file.EndsWith(".torrent"))
                    {
                        try
                        {
                            Torrent torrent = Torrent.Load(file);
                            TorrentManager manager = new TorrentManager(torrent, path_downloads, torrentDefaults);
                            if (fastResume.ContainsKey(torrent.InfoHash.ToHex()))
                            {
                                manager.LoadFastResume(new FastResume((BEncodedDictionary)fastResume[torrent.infoHash.ToHex()]));
                                torrents.Add(manager);
                                engine.Register(manager);
                                manager.Start();
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        public static void Stop()
        {
            lock (locker)
            {
                if (ViewModelData.g.BEnable_Torrent) { return; }
                if (!isStart) { return; }

                for (int i = 0; i < torrents.Count; i++)
                {
                    torrents[i].Stop();
                    SpinWait.SpinUntil(() => torrents[i].State == TorrentState.Stopped, 10000);
                    if (torrents[i].State == TorrentState.Stopped)
                    {
                        if (!fastResume.TryGetValue(torrents[i].Torrent.InfoHash.ToHex(), out BEncodedValue value))
                        {
                            fastResume.Add(torrents[i].Torrent.InfoHash.ToHex(), torrents[i].SaveFastResume().Encode());
                        }
                    }
                }
                File.WriteAllBytes(Path.Combine(RobotBase.currentfloder, RobotBase.path_dhtNodeFile), engine.DhtEngine.SaveNodes());
                File.WriteAllBytes(Path.Combine(RobotBase.currentfloder, RobotBase.path_fastResumeFile), fastResume.Encode());
                engine.Dispose();
                isStart = false;
            }
        }
        public bool Status(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> args = null)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;
            if (!ViewModelData.g.BEnable_Torrent)
            {
                content += "未启用服务";
                _content = content;
                return true;
            }
            StringBuilder sb = new StringBuilder(1024);
            AppendFormat(sb, "总下载速度: {0}/秒", SizeSuffix(engine.TotalDownloadSpeed));
            AppendFormat(sb, "总上传速度: {0}/秒", SizeSuffix(engine.TotalUploadSpeed));
            AppendFormat(sb, "硬盘读取:   {0}/秒", SizeSuffix(engine.DiskManager.ReadRate));
            AppendFormat(sb, "硬盘写入:   {0}/秒", SizeSuffix(engine.DiskManager.WriteRate));
            AppendFormat(sb, "总读取:     {0}", SizeSuffix(engine.DiskManager.TotalRead));
            AppendFormat(sb, "总写入:     {0}", SizeSuffix(engine.DiskManager.TotalWritten));
            AppendFormat(sb, "连接:       {0}", engine.ConnectionManager.OpenConnections);
            AppendFormat(sb, "DHT状态:    {0}", engine.DhtEngine.State);
            content += sb.ToString();
            _content = content;
            return true;
        }


        public bool List(string qq, string gdid, string msgContent, int messageid, out string _content, bool ispro = false, Dictionary<string, object> args = null)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;
            if (!ViewModelData.g.BEnable_Torrent)
            {
                content += "未启用服务";
                _content = content;
                return true;
            }
            StringBuilder sb = new StringBuilder(1024);
            if (torrents.Count == 0) { content += "没有任何下载任务"; }
            foreach (TorrentManager manager in torrents)
            {
                AppendFormat(sb, "ID:{0}", manager.InfoHash.ToString().Replace("-", ""));
                AppendFormat(sb, "状态:{0}　进度:{1:0.00}%", (TorrentStateChinese)manager.State, manager.Progress);
                AppendFormat(sb, "档案名称:{0}", manager.Torrent == null ? "加载中" : manager.Torrent.Name);
                AppendFormat(sb, "下载速度:{0}/s　上传速度:{1}/s", SizeSuffix(manager.Monitor.DownloadSpeed), SizeSuffix(manager.Monitor.UploadSpeed));
                AppendFormat(sb, "已下载:{0}　已上传:{1}", SizeSuffix(manager.Monitor.DataBytesDownloaded), SizeSuffix(manager.Monitor.DataBytesUploaded));

                //MonoTorrent.Client.Tracker.Tracker tracker = manager.TrackerManager.CurrentTracker;
                //   AppendFormat(sb, "上传速度:{0:0.00} kB/s", manager.Monitor.UploadSpeed / 1024.0);
                //   AppendFormat(sb, "进度:{0:0.00}%", manager.Progress);
                //   AppendFormat(sb, "已上传:{0:0.00} MB", manager.Monitor.DataBytesUploaded / (1024.0 * 1024.0));

                //AppendFormat(sb, "Tracker Status:     {0}", tracker == null ? "<no tracker>" : tracker.State.ToString());
                //AppendFormat(sb, "错误:{0}", tracker == null ? "<no tracker>" : tracker.WarningMessage);
                //AppendFormat(sb, "失败:{0}", tracker == null ? "<no tracker>" : tracker.FailureMessage);
                //if (manager.PieceManager != null)
                //    AppendFormat(sb, "请求数:{0}", manager.PieceManager.CurrentRequestCount());

                //foreach (PeerId p in manager.GetPeers())
                //    AppendFormat(sb, "\t{2} - {1:0.00}/{3:0.00}kB/秒 - {0}", p.Peer.ConnectionUri,
                //                                                              p.Monitor.DownloadSpeed / 1024.0,
                //                                                              p.AmRequestingPiecesCount,
                //                                                              p.Monitor.UploadSpeed / 1024.0);

                //if (manager.Torrent != null)
                //    foreach (TorrentFile file in manager.Torrent.Files)
                //        AppendFormat(sb, "{1:0.00}% - {0}", file.Path, file.BitField.PercentComplete);
            }

            content += sb.ToString();
            _content = content;
            return true;
        }

        public bool Reg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null)
        {
            MagnetLink ml = null;
            Torrent torrent = null;
            TorrentManager manager = null;
            Uri uri = null;
            string content = _Code.Code_At(qq) + Environment.NewLine;
            _content = "";
            if (!ViewModelData.g.BEnable_Torrent)
            {
                content += "未启用服务";
                _content = content;
                return true;
            }
            if (Uri.IsWellFormedUriString(msgContent, UriKind.Absolute))
            {
                uri = new Uri(msgContent);
            }
            if (uri != null && uri.PathAndQuery.EndsWith(".torrent"))
            {
                var filename = uri.Segments[uri.Segments.Length - 1];
                var filepath = Path.Combine(RobotBase.currentfloder, RobotBase.path_torrents, filename);
                RestClient client = new RestClient(uri);
                RestRequest request = new RestRequest();
                var tcs_0 = new TaskCompletionSource<IRestResponse>();
                var response_0 = client.ExecuteAsync(request, (r) =>
                {
                    tcs_0.SetResult(r);
                });
                SpinWait.SpinUntil(() => tcs_0.Task.IsCompleted, 5000);
                if (!tcs_0.Task.IsCompleted) { return false; }
                File.WriteAllBytes(filepath, tcs_0.Task.Result.RawBytes);
                try
                {
                    torrent = Torrent.Load(filepath);
                    manager = new TorrentManager(torrent, Path.Combine(RobotBase.currentfloder, RobotBase.path_downloads), torrentDefaults);
                }
                catch { }
            }
            else
            {
                if (!msgContent.Contains("magnet:?xt=urn:btih:") | !msgContent.Contains("magnet:?xt=urn:sha1:"))
                {
                    msgContent = "magnet:?xt=urn:btih:" + msgContent.Trim();
                }
                try
                {
                    ml = new MagnetLink(msgContent);
                    string _filepath = Path.Combine(Path.Combine(RobotBase.currentfloder, RobotBase.path_torrents), $@"{ml.InfoHash.ToString().Replace("-", "")}.torrent");
                    manager = new TorrentManager(ml, Path.Combine(RobotBase.currentfloder, RobotBase.path_downloads), torrentDefaults, _filepath);
                    manager.TorrentStateChanged += new EventHandler<TorrentStateChangedEventArgs>((o, s) =>
                    {
                        if (s.NewState == TorrentState.Downloading && s.OldState == TorrentState.Metadata)
                        {
                            if (fastResume.ContainsKey(manager.InfoHash.ToHex()))
                            {
                                manager.LoadFastResume(new FastResume((BEncodedDictionary)fastResume[torrent.infoHash.ToHex()]));
                            }
                        }
                    });
                }
                catch { }
            }

            if (manager != null)
            {
                if (torrent != null)
                {
                    if (fastResume.ContainsKey(manager.InfoHash.ToHex()))
                    {
                        manager.LoadFastResume(new FastResume((BEncodedDictionary)fastResume[torrent.infoHash.ToHex()]));
                    }
                }

                if (!torrents.Select(s => s.InfoHash).Contains(manager.InfoHash))
                {
                    engine.Register(manager);
                    manager.Start();
                    torrents.Add(manager);
                    string name = manager.Torrent == null ? "N/A" : manager.Torrent.Name;
                    content += $@"《{name}》已被加入到下载队列";
                }
                else
                {
                    content += "已在列表中";
                }
            }
            else
            {
                content += "找不到有效下载方法";
            }

            _content = content;
            return true;
        }

        public bool UnReg(string qq, string gdid, string msgContent, int messageid, out string _content, Dictionary<string, object> args = null)
        {

            string content = _Code.Code_At(qq) + Environment.NewLine;
            _content = content;
            if (!ViewModelData.g.BEnable_Torrent)
            {
                content += "未启用服务";
                _content = content;
                return true;
            }

            if (args == null && msgContent == "")
            {
                content += $"请选取项目{Environment.NewLine}";
                torrents.ForEach(f =>
                {
                    string _name = "";
                    content += $"{f.Torrent.Name.Substring(0, f.Torrent.Name.Length>20?20: f.Torrent.Name.Length)}";
                });
                content += "";
                _content = content;
                return true;
            }

            for (int i = 0; i < torrents.Count; i++)
            {
                if (torrents[i].State == TorrentState.Seeding)
                {
                    torrents[i].Stop();
                    engine.Torrents.Remove(torrents[i]);
                }
            }

            return true;
        }


        private static void AppendFormat(StringBuilder sb, string str, params object[] formatting)
        {
            if (formatting != null)
                sb.AppendFormat(str, formatting);
            else
                sb.Append(str);
            sb.AppendLine();
        }

        ///
        ///https://stackoverflow.com/questions/14488796
        ///
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(Int64 value, int decimalPlaces = 2)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }
            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }
            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }


        public bool Start(string qq, string gdid, string msgContent, int messageid, out string _content)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;
            _content = content;

            if (RobotBase.AdminQQ != qq) { _content += "没有权限执行"; return false; }
            {
                if (ViewModelData.g.BEnable_Torrent)
                {
                    _content += "已启动";
                    return false;
                }
                ViewModelData.g.BEnable_Torrent = true;
                _content += "执行完成";
                return true;
            }
        }

        public bool Stop(string qq, string gdid, string msgContent, int messageid, out string _content)
        {
            string content = _Code.Code_At(qq) + Environment.NewLine;
            _content = content;

            if (RobotBase.AdminQQ != qq) { _content += "没有权限执行"; return false; }
            {
                if (!ViewModelData.g.BEnable_Torrent)
                {
                    _content += "已停止";
                    return false;
                }
                ViewModelData.g.BEnable_Torrent = false;
                _content += "执行完成";
                return true;
            }
        }


    }
}
