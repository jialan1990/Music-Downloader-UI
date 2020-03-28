using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDownloader_New.Json
{
    public class Setting
    {
        public string SavePath { get; set; }
        public string DownloadQuality { get; set; }
        public bool IfDownloadLrc { get; set; }
        public bool IfDownloadPic { get; set; }
        /// <summary>
        /// 0 "标题 - 歌手"; 1 "歌手 - 标题"
        /// </summary>
        public int SaveNameStyle { get; set; }
        /// <summary>
        /// 0 无; 1 "歌手"; 2 "歌手 - 专辑"
        /// </summary>
        public int SavePathStyle { get; set; }
        public string SearchQuantity { get; set; }
    }

    public class Update
    {
        public List<int> Version { get; set; }
        public string Cookie { get; set; }

    }

    public class MusicInfo
    {
        public string Title { get; set; }
        public string Singer { get; set; }
        public string Album { get; set; }
        public long Id { get; set; }
        //public string Url { get; set; }
        public string LrcUrl { get; set; }
        public string PicUrl { get; set; }
    }

    public class MusicDetails
    {
        public class Ar
        {
            /// <summary>
            /// Id
            /// </summary>
            public long id { get; set; }
            /// <summary>
            /// 薛之谦
            /// </summary>
            public string name { get; set; }
        }

        public class Al
        {
            /// <summary>
            /// Id
            /// </summary>
            public long id { get; set; }
            /// <summary>
            /// 尘
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// https://p1.music.126.net/JL_id1CFwNJpzgrXwemh4Q==/109951164172892390.jpg
            /// </summary>
            public string picUrl { get; set; }
        }

        public class Songs
        {
            /// <summary>
            /// 笑场
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// Id
            /// </summary>
            public long id { get; set; }
            /// <summary>
            /// Ar
            /// </summary>
            public List<Ar> ar { get; set; }
            /// <summary>
            /// Al
            /// </summary>
            public Al al { get; set; }
            /// <summary>
            /// Mv
            /// </summary>
            public long mv { get; set; }
        }

        public class Root
        {
            /// <summary>
            /// Songs
            /// </summary>
            public List<Songs> songs { get; set; }
        }

    }

    public class GetUrl
    {
        public class Data
        {
            /// <summary>
            /// Id
            /// </summary>
            public long id { get; set; }
            /// <summary>
            /// http://m8.music.126.net/20190912230430/712bef7a551e78ec25ebef60253543dc/ymusic/7f59/3fb1/2416/00bf36a7fbd8eb8e8a2ed762e39a4cfd.flac
            /// </summary>
            public string url { get; set; }
        }

        public class Root
        {
            /// <summary>
            /// Data
            /// </summary>
            public List<Data> data { get; set; }
        }

    }

    public class SearchResultJson
    {

        public class Songs
        {
            /// <summary>
            /// Id
            /// </summary>
            public long id { get; set; }
        }

        public class Result
        {
            /// <summary>
            /// Songs
            /// </summary>
            public List<Songs> songs { get; set; }
        }

        public class Root
        {
            /// <summary>
            /// Result
            /// </summary>
            public Result result { get; set; }
        }
    }

    public class DownloadList
    {
        public string Title { get; set; }
        public string Singer { get; set; }
        public string Album { get; set; }
        public long Id { get; set; }
        public string LrcUrl { get; set; }
        public string PicUrl { get; set; }
        public bool IfDownloadLrc { get; set; }
        public bool IfDownloadPic { get; set; }
        public bool IfDownloadMusic { get; set; }
        public string Quality { get; set; }
        public string Url { get; set; }
        public string State { get; set; }
    }
}
