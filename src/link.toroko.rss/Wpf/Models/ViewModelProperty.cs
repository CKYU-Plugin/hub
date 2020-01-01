using link.toroko.rsshub;
using link.toroko.rsshub.Services;
using link.toroko.rsshub.startup;
using Robot.API;
using Robot.Extension;
using Robot.Property;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wpf.Models
{
    public class ViewModelProperty : ViewModelBase
    {

        private string _sucai_rrsc_username = "";
        private string _sucai_rrsc_password = "";
        private string _sucai_xssvip_username = "";
        private string _sucai_xssvip_password = "";
        private string _sucai_ezhanzy_username = "";
        private string _sucai_ezhanzy_password = "";
        private string _xuty_email = "";
        private int _Iport_Api = 0;
        private int _Iport_Fs = 0;
        private int _Iport_Hangfire = 0;
        private int _Iport_Web = 0;
        private int _Iport_Torrent = 0;
        private bool _BEnable_R18 = false;
        private bool _BEnable_Prefix = false;
        private bool _BEnable_Torrent = false;
        private bool _BEnable_Meinigui = true;
		private bool _BEnable_Xssvip = true;
		private bool _BEnable_Eezhanzy = true;
		private bool _BEnable_Pupuyy = true;

		private object locker = new object();
		private bool _isHentsaiss = false;

        public bool BEnable_Eezhanzy
        {
            get
            {
                return _BEnable_Eezhanzy;
            }
            set
            {
                _BEnable_Eezhanzy = value;
                OnPropertyChanged();
                lock (locker)
                {
                    IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
                    i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "BEnable_Eezhanzy", value.ToString());
                }
            }
        }

        public bool BEnable_Xssvip
        {
            get
            {
                return _BEnable_Xssvip;
            }
            set
            {
                _BEnable_Xssvip = value;
                OnPropertyChanged();
                lock (locker)
                {
                    IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
                    i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "BEnable_Xssvip", value.ToString());
                }
            }
        }
		public bool BEnable_Pupuyy
		{
			get
			{
				return _BEnable_Pupuyy;
			}
			set
			{
				_BEnable_Pupuyy = value;
				OnPropertyChanged();
				lock (locker)
				{
					IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
					i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "BEnable_Pupuyy", value.ToString());
				}
			}
		}

		public bool BEnable_Meinigui
        {
            get
            {
                return _BEnable_Meinigui;
            }
            set
            {
                _BEnable_Meinigui = value;
                OnPropertyChanged();
                lock (locker)
                {
                    IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
                    i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "BEnable_Meinigui", value.ToString());
                }
            }
        }

        public bool IsHentsaiss
		{
			get
			{
				IniFile i = new IniFile(RobotBase.appfolder + RobotBase.iniconf);
				try
				{
					return Convert.ToBoolean(i.IniReadValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "IsHentaiss"));
				}
				catch
				{
					return false;
				}
				//return _isHentsaiss;
			}
			set
			{
				_isHentsaiss = value;

				if (!File.Exists(RobotBase.appfolder + RobotBase.iniconf))
				{
					File.Create(RobotBase.appfolder + RobotBase.iniconf);
				}
				if (Monitor.TryEnter(locker, 1000))
				{
					IniFile i = new IniFile(RobotBase.appfolder + RobotBase.iniconf);
					i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "IsHentaiss", value.ToString());
					OnPropertyChanged();
					Monitor.Exit(locker);
				}
			}
		}

		public string Sucai_ezhanzy_username
        {
            get
            {
                return _sucai_ezhanzy_username;
            }
            set
            {
                _sucai_ezhanzy_username = value;
                OnPropertyChanged();
                lock (locker)
                {
                    IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
                    i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Sucai_ezhanzy_username", value.ToString());
                }
            }
        }

        public string Sucai_ezhanzy_password
        {
            get
            {
                return _sucai_ezhanzy_password;
            }
            set
            {
                _sucai_ezhanzy_password = value;
                OnPropertyChanged();
                lock (locker)
                {
                    IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
                    i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Sucai_ezhanzy_password", value.ToString());
                }
            }
        }


        public string Xuty_email
        {
            get
            {
                return _xuty_email;
            }
            set
            {
                _xuty_email = value;
                OnPropertyChanged();
                lock (locker)
                {
                    IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
                    i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Xuty_email", value.ToString());
                }
            }
        }

        public string Sucai_xssvip_password
        {
            get
            {
                return _sucai_xssvip_password;
            }
            set
            {
                _sucai_xssvip_password = value;
                OnPropertyChanged();
                lock (locker)
                {
                    IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
                    i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Sucai_xssvip_password", value.ToString());
                }
            }
        }

        public string Sucai_xssvip_username
        {
            get
            {
                return _sucai_xssvip_username;
            }
            set
            {
                _sucai_xssvip_username = value;
                OnPropertyChanged();
                lock (locker)
                {
                    IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
                    i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Sucai_xssvip_username", value.ToString());
                }
            }
        }

        public string Sucai_rrsc_password
        {
            get
            {
                return _sucai_rrsc_password;
            }
            set
            {
                _sucai_rrsc_password = value;
                OnPropertyChanged();
                lock (locker)
                {
                    IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
                    i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Sucai_rrsc_password", value.ToString());
                }
            }
        }

        public string Sucai_rrsc_username
        {
            get
            {
                return _sucai_rrsc_username;
            }
            set
            {
                _sucai_rrsc_username = value;
                OnPropertyChanged();
                lock (locker)
                {
                    IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
                    i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Sucai_rrsc_username", value.ToString());
                }
            }
        }

        public bool BEnable_R18
        {
            get
            {
                return _BEnable_R18;
            }
            set
            {
                _BEnable_R18 = value;
                OnPropertyChanged();
                lock (locker)
                {
                    IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
                    i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "R18", value.ToString());
                }
            }
        }
        public bool BEnable_Prefix
        {
            get
            {
                return _BEnable_Prefix;
            }
            set
            {
                _BEnable_Prefix = value;
                OnPropertyChanged();
                lock (locker)
                {
                    Program.CommandList();
                    IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
                    i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Prefix", value.ToString());
                }
            }
        }
        public bool BEnable_Torrent
        {
            get
            {
                return _BEnable_Torrent;
            }
            set
            {
                if (_BEnable_Torrent != value)
                {
                    _BEnable_Torrent = value;
                    OnPropertyChanged();
                    lock (locker)
                    {
                        IniFile i = new IniFile(Path.Combine(RobotBase.appfolder, RobotBase.iniconf));
                        i.IniWriteValue(CQAPI.GetLoginQQ(RobotBase.CQ_AuthCode).ToString(), "Torrent", value.ToString());
                    }
                    if (value) { Startup.Setup_TorrentClient(); } else { TorrentClient.Stop(); }
                }
            }
        }
        public int Iport_Api
        {
            get
            {
                return _Iport_Api;
            }
            set
            {
                _Iport_Api = value;
                OnPropertyChanged();
            }
        }
        public int Iport_Fs
        {
            get
            {
                return _Iport_Fs;
            }
            set
            {
                _Iport_Fs = value;
                OnPropertyChanged();
            }
        }
        public int Iport_Hangfire
        {
            get
            {
                return _Iport_Hangfire;
            }
            set
            {
                _Iport_Hangfire = value;
                OnPropertyChanged();
            }
        }
        public int Iport_Web
        {
            get
            {
                return _Iport_Web;
            }
            set
            {
                _Iport_Web = value;
                OnPropertyChanged();
            }
        }
        public int Iport_Torrent
        {
            get
            {
                return _Iport_Torrent;
            }
            set
            {
                _Iport_Torrent = value;
                OnPropertyChanged();
            }
        }
    }
}
