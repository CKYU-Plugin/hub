using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.Sucai
{
    public class XssvipDetail
    {
        public int code { get; set; }
        public string url { get; set; }
        public string link { get; set; }
        public string msg { get; set; }
    }

    public class XssvipDetail2
    {
        public int code { get; set; }
        public string msg { get; set; }
        public string url { get; set; }
        public Link[] link { get; set; }
    }

    public class Link
    {
        public string link { get; set; }
        public string label { get; set; }
    }
}
