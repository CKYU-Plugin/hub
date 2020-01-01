using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using link.toroko.rsshub;
using Robot.API;
using Robot.Property;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Robot.Code;
using Robot.Extension;
using Microsoft.Owin.Hosting;
using System.Diagnostics;
using link.toroko.rsshub.Services.bilibili.task;
using link.toroko.rsshub.Services.bilibili.web_api;
using link.toroko.rsshub.OwinStartup;
using System.Runtime.ExceptionServices;
using System.Security;
using Hangfire;
using System.CodeDom.Compiler;
using RestSharp;
using link.toroko.rsshub.Services;
using Wpf.Data;
using System.IO.Compression;
using link.toroko.rsshub.Services.Sucai2;

namespace link.toroko.rsshub.startup
{
    public static class Startup
    {

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void Setup_LoadData()
        {
            DataController.Load();
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void Setup_Assembly()
        {

        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void Setup_RunTask()
        {

            foreach (var s in Data.bilibili_bangumi.bilibili_bangumi_user)
            {
                if (s.Value.Count > 0)
                {
                    Data.bilibili_bangumi.bilibili_bangumi_session.TryGetValue(s.Key, out string name);
                    RecurringJob.AddOrUpdate($"追番{s.Key}", () => Hangfire.job.Bilibili.Bangumi(name, s.Key), Cron.MinuteInterval(Data.hangfire_conf.CronMinuteInterval));
                }
            }
            foreach (var s in Data.youtube.youtube_channel_user)
            {
                if (s.Value.Count > 0)
                {
                    Data.youtube.youtube_channel.TryGetValue(s.Key, out string name);
                    RecurringJob.AddOrUpdate($"YT_{s.Key}", () => Hangfire.job.Youtube.Channel(name, s.Key), Cron.MinuteInterval(Data.hangfire_conf.CronMinuteInterval));
                }
            }
            foreach (var s in Data.bilibili_live.bilibili_live_user)
            {
                if (s.Value.Count > 0)
                {
                    Data.bilibili_live.bilibili_live_status.TryGetValue(s.Key, out string name);
                    RecurringJob.AddOrUpdate($"B站开播通知{s.Key}", () => Hangfire.job.Bilibili.live(name, s.Key), Cron.MinuteInterval(Data.hangfire_conf.CronMinuteInterval));
                }
            }
            foreach (var s in Data.bilibili_up.bilibili_up_user)
            {
                if (s.Value.Count > 0)
                {
                    Data.bilibili_up.bilibili_up_status.TryGetValue(s.Key, out string name);
                    RecurringJob.AddOrUpdate($"关注B站UP主{s.Key}", () => Hangfire.job.Bilibili.up(name, s.Key), Cron.MinuteInterval(Data.hangfire_conf.CronMinuteInterval));
                }
            }

            //Loop Trigger
            Task.Factory.StartNew(() =>
            {
                bool locker = false;

                foreach (var s in Data.bilibili_bangumi.bilibili_bangumi_user)
                {
                    RecurringJob.Trigger($"追番{s.Key}");
                    SpinWait.SpinUntil(() => locker, 1000);
                }
                foreach (var s in Data.youtube.youtube_channel_user)
                {
                    RecurringJob.Trigger($"YT_{s.Key}");
                    SpinWait.SpinUntil(() => locker, 1000);
                }
                foreach (var s in Data.bilibili_live.bilibili_live_user)
                {
                    RecurringJob.Trigger($"B站开播通知{s.Key}");
                    SpinWait.SpinUntil(() => locker, 1000);
                }
                foreach (var s in Data.bilibili_up.bilibili_up_user)
                {
                    RecurringJob.Trigger($"关注B站UP主{s.Key}");
                    SpinWait.SpinUntil(() => locker, 1000);
                }
            }, TaskCreationOptions.LongRunning);
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void Setup_bangumi()
        {
            link.toroko.rsshub.Services.bilibili.task.Bangumi.Get(((h) =>
            {
                h?.result?.data?.ForEach(f =>
                {
                    Data.bilibili_bangumi.bilibili_bangumi_session.TryAdd(f.season_id, f.title);
                });
                DataController.Save();
            }));
        }

        private static IDisposable Server_api;
        private static IDisposable Server_hangfire;
        private static IDisposable Server_fs;
        private static IDisposable Server_web;

        private static object serviceslocker = new object();
        private static object TorrentClientlocker = new object();

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void Setup_services_stop()
        {
            lock (serviceslocker)
            {
                try
                {
                    Server_api?.Dispose();
                    Server_hangfire?.Dispose();
                    Server_fs?.Dispose();
                    Server_web?.Dispose();
                }
                catch (Exception ex)
                {
                    CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "建置网站", $"Dispose失败:{Environment.NewLine}{ex.ToString()}");
                }
            }
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void Setup_TorrentClient()
        {
            lock (TorrentClientlocker)
            {
                try
                {
                    ViewModelData.g.Iport_Torrent = Basic.GetAvailablePort(60000);
                    TorrentClient.Start(ViewModelData.g.Iport_Torrent);
                }
                catch (Exception ex)
                {
                    CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "启动TorrentClient", $"启动失败:{Environment.NewLine}{ex.ToString()}");
                }
            }
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void Setup_services()
        {
            lock (serviceslocker)
            {
                try
                {
                    try
                    {
                        ViewModelData.g.Iport_Api = Basic.GetAvailablePort(6067);
                      //  Server_api = WebApp.Start<Startup_Api>(url: $"http://localhost:{ViewModelData.g.Iport_Api}/");
                        Server_api = WebApp.Start(url: $"http://localhost:{ViewModelData.g.Iport_Api}/", (app) => Startup_Api.Configuration(app));
                    }
                    catch (System.Reflection.TargetInvocationException tie)
                    {
                        CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "建置网站", $"建置Api失败:{Environment.NewLine}{tie.ToString()}");
                    }

                    try
                    {
                        ViewModelData.g.Iport_Hangfire = Basic.GetAvailablePort(6097);
                    //    Server_hangfire = WebApp.Start<Startup_Hangfire>(url: $"http://localhost:{ViewModelData.g.Iport_Hangfire}/");
                        Server_hangfire = WebApp.Start(url: $"http://localhost:{ViewModelData.g.Iport_Hangfire}/", (app) => Startup_Hangfire.Configuration(app));
                    }
                    catch (System.Reflection.TargetInvocationException tie)
                    {
                        CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "建置网站", $"建置Hangfire失败:{Environment.NewLine}{tie.ToString()}");
                    }

                    try
                    {
                        ViewModelData.g.Iport_Fs = Basic.GetAvailablePort(6127);
                     //   Server_fs = WebApp.Start<Startup_Files>(url: $"http://localhost:{ViewModelData.g.Iport_Fs}/");
                        Server_fs = WebApp.Start(url: $"http://localhost:{ViewModelData.g.Iport_Fs}/", (app) => Startup_Files.Configuration(app));
                    }
                    catch (System.Reflection.TargetInvocationException tie)
                    {
                        CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "建置网站", $"建置文件系统失败:{Environment.NewLine}{tie.ToString()}");
                    }
                    try
                    {
                        ViewModelData.g.Iport_Web = Basic.GetAvailablePort(6157);
                     //   Server_web = WebApp.Start<Startup_BaseWeb>(url: $"http://localhost:{ViewModelData.g.Iport_Web}/");
                        Server_web = WebApp.Start(url: $"http://localhost:{ViewModelData.g.Iport_Web}/", (app) => Startup_BaseWeb.Configuration(app));
                    }
                    catch (System.Reflection.TargetInvocationException tie)
                    {
                        CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "建置网站", $"建置Html网站失败:{Environment.NewLine}{tie.ToString()}");
                    }
                }
                catch (Exception ex)
                {
                    CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "ERROR", ex.ToString());
                }
            }
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void Setup()
        {
        //    AppDomain.CreateDomain("Owin.Loader").AssemblyResolve += CurrentDomain_AssemblyResolve;
        //    AppDomain.CreateDomain("Owin").AssemblyResolve += CurrentDomain_AssemblyResolve;

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.AppendPrivatePath(@"bin\");
            AppDomain.CurrentDomain.SetCachePath(@"bin\");
            AppDomain.CurrentDomain.SetShadowCopyFiles();

            Directory.CreateDirectory("bin");
            Directory.CreateDirectory(RobotBase.appfolder);
            Directory.CreateDirectory(Path.Combine(RobotBase.appfolder, "conf"));
            Directory.CreateDirectory(Path.Combine(RobotBase.appfolder, "conf\\hangfire"));
            Directory.CreateDirectory(Path.Combine(RobotBase.appfolder, "data"));
            Directory.CreateDirectory(Path.Combine(RobotBase.appfolder,"data\\bilibili"));
            Directory.CreateDirectory(Path.Combine(RobotBase.appfolder, "data\\youtube"));
			Directory.CreateDirectory(Path.Combine(RobotBase.appfolder, "sucai"));
			try
            {
                Directory.CreateDirectory(Path.Combine(RobotBase.currentfloder, "data\\www"));
                Directory.CreateDirectory(Path.Combine(RobotBase.currentfloder, "data\\filesystem"));
                Directory.CreateDirectory(Path.Combine(RobotBase.currentfloder, "data\\Mono"));
                Directory.CreateDirectory(Path.Combine(RobotBase.currentfloder, RobotBase.path_rrsc));
                Directory.CreateDirectory(Path.Combine(RobotBase.currentfloder, RobotBase.path_torrents));
                Directory.CreateDirectory(Path.Combine(RobotBase.currentfloder, RobotBase.path_downloads));
                Directory.CreateDirectory(Path.Combine(RobotBase.currentfloder, "data\\webapi\\controller"));
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image\", RobotBase.PluginName));
                if (!File.Exists(Path.Combine(RobotBase.currentfloder, "data\\www\\index.html")))
                {
                    File.WriteAllText(Path.Combine(RobotBase.currentfloder, "data\\www\\index.html"),
$@"<html>
<title>{CQAPI.GetLoginNick(RobotBase.CQ_AuthCode)}</title>
<body>
Hello World!<p>
by {CQAPI.GetLoginNick(RobotBase.CQ_AuthCode)}({CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode)})
</body>
</html>");
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            try
            {
                IniFile i = new IniFile(RobotBase.appfolder + RobotBase.iniconf);
                long admin = 0;
                Int64.TryParse(i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "AdminQQ"), out admin);
                if (admin > 0)
                {
                    RobotBase.AdminQQ = admin.ToString();
                }

                try
                {
                    ViewModelData.g.Xuty_email = i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Xuty_email");
                }
                catch { }

                try
                {
                    ViewModelData.g.BEnable_Meinigui = Convert.ToBoolean(i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "BEnable_Meinigui"));
                }
                catch { ViewModelData.g.BEnable_Meinigui = true; }

                try
                {
                    ViewModelData.g.BEnable_Xssvip = Convert.ToBoolean(i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "BEnable_Xssvip"));
                }
                catch { ViewModelData.g.BEnable_Xssvip = true; }

				try
				{
					ViewModelData.g.BEnable_Pupuyy = Convert.ToBoolean(i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "BEnable_Pupuyy"));
				}
				catch { ViewModelData.g.BEnable_Pupuyy = true; }

				try
                {
                    ViewModelData.g.BEnable_Eezhanzy = Convert.ToBoolean(i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "BEnable_Eezhanzy"));
                }
                catch { ViewModelData.g.BEnable_Eezhanzy = true; }

                try
                {
					ViewModelData.g.IsHentsaiss = Convert.ToBoolean(i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "IsHentaiss"));
				}
				catch { ViewModelData.g.IsHentsaiss = false; }

				try
                {
                    ViewModelData.g.Sucai_xssvip_username = i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Sucai_xssvip_username");
                }
                catch { }
                try
                {
                    ViewModelData.g.Sucai_xssvip_password = i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Sucai_xssvip_password");
                }
                catch { }

                try
                {
                    ViewModelData.g.Sucai_ezhanzy_username = i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Sucai_ezhanzy_username");
                }
                catch { }
                try
                {
                    ViewModelData.g.Sucai_ezhanzy_password = i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Sucai_ezhanzy_password");
                }
                catch { }

                try
                {
                    ViewModelData.g.Sucai_rrsc_username = i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Sucai_rrsc_username");
                }
                catch { }
                try
                {
                    ViewModelData.g.Sucai_rrsc_password = i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Sucai_rrsc_password");
                }
                catch { }

                try
                {
                    ViewModelData.g.BEnable_Torrent = Boolean.Parse(i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Torrent"));
                }
                catch { }
                try
                {
                    ViewModelData.g.BEnable_Prefix = Boolean.Parse(i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Prefix"));
                }
                catch { }
                try
                {
                    ViewModelData.g.BEnable_R18 = Boolean.Parse(i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "R18"));
                }
                catch { }
                try
                {
                    RobotBase.SauceNAOApiToken = i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "SauceNAOToken");
                }
                catch { }
                try
                {
                    RobotBase.WhatAnimeApiToken = i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "WhatAnimeToken");
                }
                catch { }

				try
				{
					Sucai2.Initializer();
				}
				catch { }
		}
            catch { }
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TorrentClient.Stop();
            CQAPI.AddLog(RobotBase.CQ_AuthCode, CQAPI.LogPriority.CQLOG_ERROR, "ERROR", $"UnhandledException:{Environment.NewLine}{e.ToString()}");
        }

        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
			var assembly = Assembly.GetExecutingAssembly();

			using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.ThirdpartyDll.{args.Name.Split(',')[0]}.dll"))
            {
                if (stream == null)
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    if (assemblies.Where(w => w.FullName == args.Name).Count() > 0)
                    {
                        return assemblies.First(f => f.FullName == args.Name);
                    }
                    else
                    {
                        if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "/bin", $"{assembly.GetName().Name}.{args.Name.Split(',')[0]}.dll")))
                        {
                            return Assembly.LoadFrom(Path.Combine(Directory.GetCurrentDirectory(), "/bin", $"{assembly.GetName().Name}.{args.Name.Split(',')[0]}.dll"));
                        }
                        return assembly;
                    }
                }
                else
                {
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    stream.Flush();
                    stream.Close();
                    return Assembly.Load(buffer);
                }
            }
        }


		[HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void Setup_OutPut()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Queue<string> LoadLibrary = new Queue<string>();

            LoadLibrary.Enqueue("Owin.dll");
            LoadLibrary.Enqueue("Microsoft.Owin.Hosting.dll");
            LoadLibrary.Enqueue("Microsoft.Owin.StaticFiles.dll");
            LoadLibrary.Enqueue("Microsoft.Owin.FileSystems.dll");
            LoadLibrary.Enqueue("Hangfire.Core.dll");
            LoadLibrary.Enqueue("Hangfire.MemoryStorage.dll");

            //LoadLibrary.Enqueue("MahApps.Metro.dll");
            //LoadLibrary.Enqueue("MahApps.Metro.IconPacks.Material.dll");
            //LoadLibrary.Enqueue("ControlzEx.dll");

            //LoadLibrary.Enqueue("System.Windows.Interactivity.dll");
            //LoadLibrary.Enqueue("Microsoft.Owin.Host.HttpListener.dll");

            //LoadLibrary.Enqueue("Microsoft.Owin.dll");
            //   LoadLibrary.Enqueue("System.Web.Http.dll");
            //   LoadLibrary.Enqueue("System.Web.Http.Owin.dll");
            //   LoadLibrary.Enqueue("System.Net.Http.Formatting.dll");
            //LoadLibrary.Enqueue("Microsoft.WebSockets.dll");

            //LoadLibrary.Enqueue("System.ValueTuple.dll");
            //LoadLibrary.Enqueue("System.Threading.Tasks.Extensions.dll");
            //LoadLibrary.Enqueue("MessagePack.dll");
            //LoadLibrary.Enqueue("me.kotone.owin.startup.dll");
            //LoadLibrary.Enqueue("Newtonsoft.Json.dll");


            while (LoadLibrary.Count() != 0)
            {
                string resource = LoadLibrary.Dequeue();

                try
                {
                    using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.ThirdpartyDll.{resource}"))
                    {
                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        stream.Flush();
                        stream.Close();
                        string path = $@"{AppDomain.CurrentDomain.BaseDirectory}bin\{resource}";
                        if (!File.Exists(path))
                        {
                            File.WriteAllBytes(path, buffer);
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }

    }
}
