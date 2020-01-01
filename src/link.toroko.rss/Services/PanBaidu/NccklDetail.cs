using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.PanBaidu
{

    public class NccklDetail
    {
        public int code { get; set; }
        public Body body { get; set; }
    }

    public class Body
    {
        public int code { get; set; }
        public List<Body1> body { get; set; }
        public int pageCurrent { get; set; }
        public int total { get; set; }
        public int pageSize { get; set; }
    }

    public class Body1
    {
        public string _id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string info { get; set; }
        public string password { get; set; }
        public string classify { get; set; }
        public string size { get; set; }
        public string createDate { get; set; }
    }

}
