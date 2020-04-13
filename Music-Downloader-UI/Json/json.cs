using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDownloader.Json
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

    public class Musiclist
    {
        public class Root
        {
            public Playlist playlist { get; set; }
        }
        public class Playlist
        {
            public List<TrackIdsItem> trackIds { get; set; }
        }
        public class TrackIdsItem
        {
            public long id { get; set; }
        }
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
        public string Id { get; set; }
        //public string Url { get; set; }
        public string LrcUrl { get; set; }
        public string PicUrl { get; set; }
        public int Api { get; set; }

        /// <summary>
        /// 只有QQ音乐需要该参数
        /// </summary>
        public string strMediaMid { get; set; }
    }

    public class NeteaseMusicDetails
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

    public class QQMusicDetails
    {
        public class Root
        {
            public data data { get; set; }
        }

        public class data
        {
            public List<list> list { get; set; }
        }

        public class list
        {
            public string songmid { get; set; }
            public string strMediaMid { get; set; }
            public string albumname { get; set; }
            public string songname { get; set; }
            public string albummid { get; set; }
            public List<singer> singer { get; set; }
        }

        public class singer
        {
            public string name { get; set; }
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
        public string Id { get; set; }
        public string LrcUrl { get; set; }
        public string PicUrl { get; set; }
        public bool IfDownloadLrc { get; set; }
        public bool IfDownloadPic { get; set; }
        public bool IfDownloadMusic { get; set; }
        public string Quality { get; set; }
        public string Url { get; set; }
        public string State { get; set; }
        public int Api { get; set; }
        public string strMediaMid { get; set; }
    }

    public class NeteaseAlbum
    {
        public class Root
        {
            public List<songs> songs { get; set; }
            public album album { get; set; }
        }

        public class al
        {
            public string picUrl { get; set; }
        }

        public class songs
        {
            public al al { get; set; }
            public List<ar> ar { get; set; }
            public string name { get; set; }
            public long id { get; set; }
        }

        public class ar
        {
            public string name { get; set; }
        }

        public class album
        {
            public string name { get; set; }
        }
    }

    public class NeteaseLrc
    {
        public class Root
        {
            public lrc lrc { get; set; }
        }

        public class lrc
        {
            public string lyric { get; set; }
        }
    }

    public class QQLrc
    {
        public class Root
        {
            public data data { get; set; }
        }

        public class data
        {
            public string lrc { get; set; }
        }
    }

    public class QQmusicdetails
    {
        public string data { get; set; }
    }

    public class QQmusiclist
    {
        public class Root
        {
            public data data { get; set; }
        }

        public class data
        {
            public List<songlist> songlist { get; set; }
        }

        public class songlist
        {
            public string songname { get; set; }
            public string songmid { get; set; }
            public string strMediaMid { get; set; }
            public string albumname { get; set; }
            public List<singer> singer { get; set; }
            public string albummid { get; set; }
        }

        public class singer
        {
            public string name { get; set; }
        }
    }

    public class QQAlbum
    {
        public class Root
        {
            public data data { get; set; }
        }

        public class data
        {
            public List<list> list { get; set; }
        }

        public class list
        { 
            public string mid { get; set; }
            public string title { get; set; }
            public List<singer> singer { get; set; }
            public album album { get; set; }
            public ksong ksong { get; set; }
        }

        public class singer
        { 
            public string title { get; set; }
        }

        public class album
        {
            public string title { get; set; }
            public string mid { get; set; }
        }

        public class ksong
        { 
            public string mid { get; set; }
        }
    }
}
