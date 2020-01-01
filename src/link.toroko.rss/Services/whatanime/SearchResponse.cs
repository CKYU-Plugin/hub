using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace link.toroko.rsshub.bangumianimesearch.Services.whatanime
{

    public class SearchResponse
    {
        /// <summary>
        /// Total number of frames searched
        /// </summary>
        [JsonProperty(PropertyName = "RawDocsCount")]
        public long RawDocsCount { get; set; }

        /// <summary>
        /// Time taken to retrieve the frames from database (sum of all cores)
        /// </summary>
        [JsonProperty(PropertyName = "RawDocsSearchTime")]
        public long RawDocsSearchTime { get; set; }

        /// <summary>
        /// Time taken to compare the frames (sum of all cores)
        /// </summary>
        [JsonProperty(PropertyName = "ReRankSearchTime")]
        public long ReRankSearchTime { get; set; }

        /// <summary>
        /// Whether the search result is cached. (Results are cached by extraced image feature)
        /// </summary>
        [JsonProperty(PropertyName = "CacheHit")]
        public bool CacheHit { get; set; }

        /// <summary>
        /// Number of times searched
        /// </summary>
        [JsonProperty(PropertyName = "trial")]
        public long Trial { get; set; }

        /// <summary>
        /// Number of search quota remaining
        /// </summary>
        [JsonProperty(PropertyName = "quota")]
        public long Quota { get; set; }

        /// <summary>
        /// Time until quota resets (seconds)
        /// </summary>
        [JsonProperty(PropertyName = "expire")]
        public long Expire { get; set; }

        /// <summary>
        /// Search results (see table below)
        /// </summary>
        [JsonProperty(PropertyName = "docs")]
        public List<Doc> Docs { get; set; }

    }

    public class Doc
    {
        /// <summary>
        /// Starting time of the matching scene
        /// <return>Number (seconds, in 2 decimal places)</return>
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public float From { get; set; }

        /// <summary>
        /// Ending time of the matching scene
        /// <return>Number (seconds, in 2 decimal places)</return>
        /// </summary>
        [JsonProperty(PropertyName = "to")]
        public double To { get; set; }

        /// <summary>
        /// The matching AniList ID
        /// </summary>
        [JsonProperty(PropertyName = "anilist_id")]
        public long Anilist_id { get; set; }

        /// <summary>
        /// Exact time of the matching scene
        /// </summary>
        [JsonProperty(PropertyName = "at")]
        public float At { get; set; }

        /// <summary>
        /// (deprecated, do not use this)
        /// </summary>
        [JsonProperty(PropertyName = "season")]
        public string Season { get; set; }

        /// <summary>
        /// (deprecated, do not use this)
        /// </summary>
        [JsonProperty(PropertyName = "anime")]
        public string Anime { get; set; }

        /// <summary>
        /// The filename of file where the match is found
        /// </summary>
        [JsonProperty(PropertyName = "filename")]
        public string Filename { get; set; }

        /// <summary>
        /// The extracted episode number from filename
        /// <return>Number, "OVA/OAD", "Special", ""</return>
        /// </summary>
        [JsonProperty(PropertyName = "episode")]
        public string Episode { get; set; }

        /// <summary>
        /// A token for generating preview
        /// </summary>
        [JsonProperty(PropertyName = "tokenthumb")]
        public string Tokenthumb { get; set; }

        /// <summary>
        /// Similarity compared to the search image
        /// </summary>
        [JsonProperty(PropertyName = "similarity")]
        public double Similarity { get; set; }

        /// <summary>
        /// (deprecated, do not use this)
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Native (Japanese) title
        /// </summary>
        [JsonProperty(PropertyName = "title_native")]
        public string Title_native { get; set; }

        /// <summary>
        /// Chinese title
        /// </summary>
        [JsonProperty(PropertyName = "title_chinese")]
        public string Title_chinese { get; set; }

        /// <summary>
        /// English title
        /// </summary>
        [JsonProperty(PropertyName = "title_english")]
        public string Title_english { get; set; }

        /// <summary>
        /// Title in romaji
        /// </summary>
        [JsonProperty(PropertyName = "title_romaji")]
        public string Title_romaji { get; set; }

        /// <summary>
        /// The matching MyAnimeList ID
        /// </summary>
        [JsonProperty(PropertyName = "mal_id")]
        public long? Mal_id { get; set; }

        /// <summary>
        /// Alternate english titles
        /// </summary>
        [JsonProperty(PropertyName = "synonyms")]
        public List<string> Synonyms { get; set; }

        /// <summary>
        /// Alternate chinese titles
        /// </summary>
        [JsonProperty(PropertyName = "synonyms_chinese")]
        public JRaw Synonyms_chinese { get; set; }

        /// <summary>
        /// Whether the anime is hentai
        /// </summary>
        [JsonProperty(PropertyName = "is_adult")]
        public bool Is_adult { get; set; }
    }

}
