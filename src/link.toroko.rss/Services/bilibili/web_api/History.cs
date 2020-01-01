using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.bilibili.web_api
{
    public class History
    {
        public int code { get; set; }
        public string message { get; set; }
        public HistoryResult result { get; set; }
    }

    public class HistoryResult
    {
        public List<Datum> data { get; set; }
        public Page page { get; set; }
    }

    public class Page
    {
        public int num { get; set; }
        public int size { get; set; }
        public int total { get; set; }
    }

    public class Datum
    {
        public string badge { get; set; }
        public int badge_type { get; set; }
        public string cover { get; set; }
        public string index_show { get; set; }
        public int is_finish { get; set; }
        public string link { get; set; }
        public int media_id { get; set; }
        public Order order { get; set; }
        public int season_id { get; set; }
        public string title { get; set; }
    }

    public class Order
    {
        public string follow { get; set; }
        public string play { get; set; }
        public int pub_date { get; set; }
        public int pub_real_time { get; set; }
        public int renewal_time { get; set; }
        public string score { get; set; }
    }
}
