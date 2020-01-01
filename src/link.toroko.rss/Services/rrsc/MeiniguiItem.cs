using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.rrsc
{
    public class MeiniguiItem
    {
        public ConcurrentDictionary<string, string> BaiduPan { get; set; } = new ConcurrentDictionary<string, string>();
        public List<string> CtPan { get; set; } = new List<string>();
        public List<string> Thunder { get; set; } = new List<string>();
        public ConcurrentDictionary<string, string> YunPan { get; set; } = new ConcurrentDictionary<string, string>();
        public ConcurrentDictionary<string, string> Ed2k { get; set; } = new ConcurrentDictionary<string, string>();
        public string UnzipPassword { get; set; } = "";
        public string Message { get; set; } = "";
        public bool IsFile { get; set; } = false;
        public bool NeedToDownload { get; set; } = false;
        public string FileName { get; set; } = "";
        public string DownloadUrl { get; set; } = "";
        public string ReferenceNote { get; set; } = "";
    }
}
