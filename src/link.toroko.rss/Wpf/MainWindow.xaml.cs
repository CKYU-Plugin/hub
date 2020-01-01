using link.toroko.rsshub.startup;
using Robot.API;
using Robot.Extension;
using Robot.Property;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Wpf.Data;

namespace Wpf
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer _timer;
        TimeSpan _time;


        public MainWindow()
        {

            InitializeComponent();
            this.DataContext = ViewModelData.g;
            admin.Text = RobotBase.AdminQQ;
            saucenaoapi.Text = RobotBase.SauceNAOApiToken;
            whatanimegaapi.Text = RobotBase.WhatAnimeApiToken;
            _time = TimeSpan.FromSeconds(0);
            //port_api.Text = Startup.Server_api_port.ToString();
            //port_fs.Text = Startup.Server_fs_port.ToString();
            //port_hangfire.Text = Startup.Server_hangfire_port.ToString();
            //port_web.Text = Startup.Server_web_port.ToString();
            //port_torrent.Text = Startup.Server_torrent_port.ToString();

            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                if (_time == TimeSpan.Zero)
                {
                    messageBorder.Visibility = Visibility.Hidden;
                    _timer.Stop();
                }
                _time = _time.Add(TimeSpan.FromSeconds(-1));
            }, this.Dispatcher);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (!File.Exists(Path.Combine(RobotBase.appfolder, RobotBase.iniconf)))
            {
                File.Create(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
            }

            IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));

            Int64 adminQQ = 0;
            bool co = false;
            co = Int64.TryParse(admin.Text, out adminQQ);

            messageBorder.Visibility = Visibility.Visible;
            _time = TimeSpan.FromSeconds(3);
            _timer.Start();

            message.Text = $"已保存";

            if (co)
            {
                i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "AdminQQ", adminQQ.ToString());
                RobotBase.AdminQQ = adminQQ.ToString();
            }
            else
            {
                message.Text = "主人QQ输入错误,请重新输入";
                return;
            }

            if (saucenaoapi.Text.Length == 40)
            {
                RobotBase.SauceNAOApiToken = saucenaoapi.Text;
                i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "SauceNAOToken", saucenaoapi.Text);
            }
            else
            {
                if (saucenaoapi.Text.Length > 0)
                {
                    message.Text = "SauceNAO API Token输入错误,请重新输入";
                }
            }

            if (whatanimegaapi.Text.Length == 40 || whatanimegaapi.Text.Length == 0)
            {
                RobotBase.WhatAnimeApiToken = whatanimegaapi.Text;
                i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "WhatAnimeToken", whatanimegaapi.Text);
            }
            else
            {
                message.Text = "WhatAnime API Token输入错误,请重新输入";
                return;
            }

        }

        private void Button_Click_open_api(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo($"http://localhost:{ViewModelData.g.Iport_Api}/"));
            e.Handled = true;
        }

        private void Button_Click_open_hangfire(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo($"http://localhost:{ViewModelData.g.Iport_Hangfire}/"));
            e.Handled = true;
        }

        private void Button_Click_open_fs(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo($"http://localhost:{ViewModelData.g.Iport_Fs}/"));
            e.Handled = true;
        }

        private void Button_Click_open_web(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo($"http://localhost:{ViewModelData.g.Iport_Web}/"));
            e.Handled = true;
        }

        private void Button_Click_open_fs2(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(Path.Combine(RobotBase.currentfloder, "data\\filesystem")));
            e.Handled = true;
        }

        private void Button_Click_open_web2(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(Path.Combine(RobotBase.currentfloder, "data\\www")));
            e.Handled = true;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{

		}
	}
}
