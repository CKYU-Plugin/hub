using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.Services.bilibili.web_api
{
    public class Timeline
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<Result> result { get; set; }
    }

    public class Result
    {
        public string date { get; set; }
        public int date_ts { get; set; }
        public int day_of_week { get; set; }
        public int is_today { get; set; }
        public Season[] seasons { get; set; }
    }

    public class Season
    {
        public string cover { get; set; }
        public int delay { get; set; }
        public int ep_id { get; set; }
        public int favorites { get; set; }
        public int follow { get; set; }
        public int is_published { get; set; }
        public string pub_index { get; set; }
        public string pub_time { get; set; }
        public int pub_ts { get; set; }
        public int season_id { get; set; }
        public int season_status { get; set; }
        public string square_cover { get; set; }
        public string title { get; set; }
        public string badge { get; set; }
        public int delay_id { get; set; }
        public string delay_index { get; set; }
        public string delay_reason { get; set; }
    }
}
