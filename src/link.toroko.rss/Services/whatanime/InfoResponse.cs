using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.rsshub.bangumianimesearch.Services.whatanime
{
    public class InfoResponse
    {
            public int id { get; set; }
            public int idMal { get; set; }
            public Title title { get; set; }
            public string type { get; set; }
            public string format { get; set; }
            public string status { get; set; }
            public string description { get; set; }
            public Startdate startDate { get; set; }
            public Enddate endDate { get; set; }
            public string season { get; set; }
            public int episodes { get; set; }
            public int duration { get; set; }
            public string source { get; set; }
            public string hashtag { get; set; }
            public Trailer trailer { get; set; }
            public int updatedAt { get; set; }
            public Coverimage coverImage { get; set; }
            public string bannerImage { get; set; }
            public List<string> genres { get; set; }
            public List<string> synonyms { get; set; }
            public int averageScore { get; set; }
            public int meanScore { get; set; }
            public int popularity { get; set; }
            public List<Tag> tags { get; set; }
            public JRaw relations { get; set; }
            public JRaw characters { get; set; }
            public JRaw staff { get; set; }
            public JRaw studios { get; set; }
            public bool isAdult { get; set; }
            public List<Externallink> externalLinks { get; set; }
            public List<Ranking> rankings { get; set; }
            public Stats stats { get; set; }
            public string siteUrl { get; set; }
            public JRaw synonyms_chinese { get; set; }
        }

        public class Title
        {
            public string native { get; set; }
            public string romaji { get; set; }
            public string english { get; set; }
            public string chinese { get; set; }
        }

        public class Startdate
        {
            public int year { get; set; }
            public int month { get; set; }
            public int day { get; set; }
        }

        public class Enddate
        {
            public int year { get; set; }
            public int month { get; set; }
            public int day { get; set; }
        }

        public class Trailer
        {
            public string id { get; set; }
            public string site { get; set; }
        }

        public class Coverimage
        {
            public string large { get; set; }
            public string medium { get; set; }
        }

        public class Stats
        {
            public List<Scoredistribution> scoreDistribution { get; set; }
            public List<Statusdistribution> statusDistribution { get; set; }
        }

        public class Scoredistribution
        {
            public int score { get; set; }
            public int amount { get; set; }
        }

        public class Statusdistribution
        {
            public string status { get; set; }
            public int amount { get; set; }
        }

        public class Tag
        {
            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string category { get; set; }
            public int rank { get; set; }
            public bool isGeneralSpoiler { get; set; }
            public bool isMediaSpoiler { get; set; }
            public bool isAdult { get; set; }
        }

        public class Externallink
        {
            public int id { get; set; }
            public string url { get; set; }
            public string site { get; set; }
        }

        public class Ranking
        {
            public int id { get; set; }
            public int rank { get; set; }
            public string type { get; set; }
            public string format { get; set; }
            public int? year { get; set; }
            public string season { get; set; }
            public bool allTime { get; set; }
            public string context { get; set; }
        }

}
