using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.Ezhanzy
{
    public class EzhanzyItem
    {
        public ConcurrentDictionary<string, string> BaiduPan { get; set; } = new ConcurrentDictionary<string, string>();
        public List<string> CtPan { get; set; } = new List<string>();
        public List<string> Thunder { get; set; } = new List<string>();
        public ConcurrentDictionary<string,string> Ed2k { get; set; } = new ConcurrentDictionary<string,string>();
        public ConcurrentDictionary<string, string> YunPan { get; set; } = new ConcurrentDictionary<string, string>();
        public string UnzipPassword { get; set; } = "";
        public string ReferenceData { get; set; } = "";
        public string TxtFile { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
