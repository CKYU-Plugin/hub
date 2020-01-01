using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace link.toroko.rsshub.Services.diygod
{
    [XmlRoot(ElementName = "item")]
    public class Item
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "pubDate")]
        public string PubDate { get; set; }
        [XmlElement(ElementName = "guid")]
        public string Guid { get; set; }
        [XmlElement(ElementName = "link")]
        public string Link { get; set; }
    }

    [XmlRoot(ElementName = "channel")]
    public class Channel
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "link")]
        public string Link { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "generator")]
        public string Generator { get; set; }
        [XmlElement(ElementName = "webMaster")]
        public string WebMaster { get; set; }
        [XmlElement(ElementName = "language")]
        public string Language { get; set; }
        [XmlElement(ElementName = "lastBuildDate")]
        public string LastBuildDate { get; set; }
        [XmlElement(ElementName = "ttl")]
        public string Ttl { get; set; }
        [XmlElement(ElementName = "item")]
        public List<Item> Item { get; set; }
    }

    [XmlRoot(ElementName = "rss")]
    public class Rss
    {
        [XmlElement(ElementName = "channel")]
        public Channel Channel { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }


   
    public class RssHub_json
    {
        public string version { get; set; }
        public string title { get; set; }
        public string home_page_url { get; set; }
        public string description { get; set; }
        public OwnAuthor_json author { get; set; }
        public Item_json[] items { get; set; }
    }

    public class OwnAuthor_json
    {
        public string name { get; set; }
    }

    public class Item_json
    {
        public string id { get; set; }
        public string url { get; set; }
        public Author_json author { get; set; }
        public string external_url { get; set; }
        public string title { get; set; }
        public string content_html { get; set; }
        public string content_text { get; set; }
        public string summary { get; set; }
        public string image { get; set; }
        public string banner_image { get; set; }
        public string date_published { get; set; }
        public string date_modified { get; set; }
        public object[] tags { get; set; }
        public object[] attachments { get; set; }
    }

    public class Author_json
    {
        public string name { get; set; }
    }

}
