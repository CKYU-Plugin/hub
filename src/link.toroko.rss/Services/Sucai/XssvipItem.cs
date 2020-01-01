using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.Sucai
{
    public class XssvipItem
    {
        public string Message { get; set; } = "";
        public Dictionary<string, string> DownloadUrls { get; set; } = new Dictionary<string, string>();
    }
}
