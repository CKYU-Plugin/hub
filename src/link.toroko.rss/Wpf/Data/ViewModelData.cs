using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using Wpf.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static Wpf.Models.ViewModelProperty;
using Robot.Property;

namespace Wpf.Data
{
    public static class ViewModelData
    {
        public static ViewModelProperty g = new ViewModelProperty();
        public static object filelock = new object();

        public static void Save()
        {
            lock (filelock)
            {
                try
                {
                //    File.WriteAllText(RobotBase.data, JsonConvert.SerializeObject(ViewModelData.g.SRModelList));
                }
                catch { }
            }
        }

        public static void Load()
        {
            lock (filelock)
            {
                try
                {
                }
                catch { }
            }
        }
    }


}
