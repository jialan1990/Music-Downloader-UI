using MusicDownloader.Json;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TagLib;

namespace MusicDownloader.Library
{
    public class Music
    {
        List<int> version = new List<int> { 1, 0, 2 };
        const string ApiUrl = "";//自行搭建接口
        public Setting setting;
        public List<DownloadList> downloadlist = new List<DownloadList>();
        string cookie = "";
        Thread th_Download;
        public delegate void UpdateDownloadPageEventHandler();
        public delegate void NotifyUpdateEventHandler();
        public delegate void NotifyConnectErrorEventHandler();
        public event UpdateDownloadPageEventHandler UpdateDownloadPage;
        public event NotifyUpdateEventHandler NotifyUpdate;
        public event NotifyConnectErrorEventHandler NotifyConnectError;

        /// <summary>
        /// 获取更新数据
        /// </summary>
        /// <returns></returns>
        public void Update()
        {

            WebClient wc = new WebClient();
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(wc.OpenRead("http://nitian1207.cn/update/MusicDownload.json"));
            }
            catch
            {
                NotifyConnectError();
            }
            Update update = JsonConvert.DeserializeObject<Update>(sr.ReadToEnd());
            cookie = update.Cookie;
            bool needupdate = false;
            if (update.Version[0] > version[0])
            {
                needupdate = true;
            }
            else if (update.Version[1] > version[1])
            {
                needupdate = true;
            }
            else if (update.Version[2] > version[2])
            {
                needupdate = true;
            }
            if (needupdate)
            {
                NotifyUpdate();
            }
        }

        /// <summary>
        /// 构造函数 需要提供设置参数
        /// </summary>
        /// <param name="setting"></param>
        public Music(Setting setting, NotifyConnectErrorEventHandler ConnectErrorEventHandler, NotifyUpdateEventHandler UpdateEventHandler)
        {
            this.setting = setting;
            NotifyConnectError += ConnectErrorEventHandler;
            NotifyUpdate += UpdateEventHandler;
        }

        /// <summary>
        /// 搜索方法
        /// </summary>
        public List<MusicInfo> Search(string Key)
        {
            try
            {
                List<MusicInfo> searchItem = new List<MusicInfo>();
                string key = Key;
                int quantity = Int32.Parse(setting.SearchQuantity);
                int pagequantity = quantity / 100;
                int remainder = quantity % 100;

                if (remainder == 0)
                {
                    remainder = 100;
                }
                if (pagequantity == 0)
                {
                    pagequantity = 1;
                }
                for (int i = 0; i < pagequantity; i++)
                {
                    if (i == pagequantity - 1 && pagequantity >= 1)
                    {
                        searchItem.AddRange(Search(key, i + 1, remainder));
                    }
                    else
                    {
                        searchItem.AddRange(Search(key, i + 1, 100));
                    }
                }
                return searchItem;
            }
            catch { return null; }
        }

        /// <summary>
        /// 带cookie访问
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string GetHTML(string url)
        {
            WebClient wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.Cookie, cookie);
            Stream s = wc.OpenRead(url);
            StreamReader sr = new StreamReader(s);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// 搜索歌曲
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        List<MusicInfo> Search(string Key, int Page = 1, int limit = 100)
        {
            if (Key == null || Key == "")
            {
                return null;
            }
            WebClient wc = new WebClient();
            string offset = ((Page - 1) * 100).ToString();
            string url = ApiUrl + "search?keywords=" + Key + "&limit=" + limit.ToString() + "&offset=" + offset;
            string json = GetHTML(url);
            if (json == null || json == "")
            {
                return null;
            }
            Json.SearchResultJson.Root srj = JsonConvert.DeserializeObject<Json.SearchResultJson.Root>(json);
            List<Json.MusicInfo> ret = new List<Json.MusicInfo>();
            string ids = "";
            for (int i = 0; i < srj.result.songs.Count; i++)
            {
                ids += srj.result.songs[i].id + ",";
            }
            string _u = ApiUrl + "song/detail?ids=" + ids.Substring(0, ids.Length - 1);
            string j = GetHTML(_u);
            Json.MusicDetails.Root mdr = JsonConvert.DeserializeObject<Json.MusicDetails.Root>(j);
            for (int i = 0; i < mdr.songs.Count; i++)
            {
                string singer = "";
                for (int x = 0; x < mdr.songs[i].ar.Count; x++)
                {
                    singer += mdr.songs[i].ar[x].name + "、";
                    //singerid.Add(mdr.songs[i].ar[x].id.ToString());
                }
                Json.MusicInfo mi = new Json.MusicInfo()
                {
                    Album = mdr.songs[i].al.name,
                    Id = mdr.songs[i].id,
                    LrcUrl = ApiUrl + "lyric?id=" + mdr.songs[i].id,
                    PicUrl = mdr.songs[i].al.picUrl,
                    Singer = singer.Substring(0, singer.Length - 1),
                    Title = mdr.songs[i].name,
                    //Url = _url
                };
                ret.Add(mi);
            }
            return ret;
        }

        /// <summary>
        /// 下载方法
        /// </summary>
        /// <param name="dl"></param>
        public void Download(List<DownloadList> dl)
        {
            string ids = "";
            int times = dl.Count / 500;
            int remainder = dl.Count % 500;
            if (remainder == 0)
            {
                remainder = 500;
            }
            if (times == 0)
            {
                times = 1;
            }
            for (int i = 0; i < times; i++)
            {
                if (i == times - 1 && times >= 1)
                {
                    ids = "";
                    for (int x = 0; x < remainder; x++)
                    {
                        ids += dl[i * 500 + x].Id + ",";
                    }
                    ids = ids.Substring(0, ids.Length - 1);
                    string u = ApiUrl + "song/url?id=" + ids + "&br=" + dl[0].Quality;
                    Json.GetUrl.Root urls = JsonConvert.DeserializeObject<Json.GetUrl.Root>(GetHTML(u));
                    for (int x = 0; x < remainder; x++)
                    {
                        for (int y = 0; y < dl.Count; y++)
                        {
                            if (urls.data[x].id == dl[y].Id)
                            {
                                dl[y].Url = urls.data[x].url;
                                dl[y].State = "准备下载";
                            }
                        }
                    }
                }
                else
                {
                    ids = "";
                    for (int x = 0; x < 500; x++)
                    {
                        ids += dl[i * 500 + x].Id + ",";
                    }
                    ids = ids.Substring(0, ids.Length - 1);
                    string u = ApiUrl + "song/url?id=" + ids + "&br=" + dl[0].Quality;
                    Json.GetUrl.Root urls = JsonConvert.DeserializeObject<Json.GetUrl.Root>(GetHTML(u));
                    for (int x = 0; x < 100; x++)
                    {
                        for (int y = 0; y < dl.Count; y++)
                        {
                            if (urls.data[x].id == dl[y].Id)
                            {
                                dl[y].Url = urls.data[x].url;
                                dl[y].State = "准备下载";
                            }
                        }
                    }
                }
            }
            downloadlist.AddRange(dl);
            UpdateDownloadPage();
            if (th_Download == null || th_Download?.ThreadState == ThreadState.Stopped)
            {
                th_Download = new Thread(_Download);
                th_Download.Start();
            }
        }

        /// <summary>
        /// 文件名检查
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string NameCheck(string name)
        {
            string re = name.Replace("*", " ");
            re = re.Replace("\\", " ");
            re = re.Replace("\"", " ");
            re = re.Replace("<", " ");
            re = re.Replace(">", " ");
            re = re.Replace("|", " ");
            re = re.Replace("?", " ");
            re = re.Replace("/", ",");
            re = re.Replace(":", "：");
            //re = re.Replace("-", "_");
            return re;
        }

        /// <summary>
        /// 下载线程
        /// </summary>
        private void _Download()
        {
            while (downloadlist.Count != 0)
            {
                downloadlist[0].State = "正在下载音乐";
                if (downloadlist[0].Url == null)
                {
                    downloadlist[0].State = "无版权";
                    UpdateDownloadPage();
                    downloadlist.RemoveAt(0);
                    continue;
                }
                UpdateDownloadPage();
                string savepath = "";
                string filename = ""; ;
                switch (setting.SaveNameStyle)
                {
                    case 0:
                        if (downloadlist[0].Url.IndexOf("flac") != -1)
                            filename = NameCheck(downloadlist[0].Title) + " - " + NameCheck(downloadlist[0].Singer) + ".flac";
                        else
                            filename = NameCheck(downloadlist[0].Title) + " - " + NameCheck(downloadlist[0].Singer) + ".mp3";
                        break;
                    case 1:
                        if (downloadlist[0].Url.IndexOf("flac") != -1)
                            filename = NameCheck(downloadlist[0].Singer) + " - " + NameCheck(downloadlist[0].Title) + ".flac";
                        else
                            filename = NameCheck(downloadlist[0].Singer) + " - " + NameCheck(downloadlist[0].Title) + ".mp3";
                        break;
                }
                switch (setting.SavePathStyle)
                {
                    case 0:
                        savepath = setting.SavePath;
                        break;
                    case 1:
                        savepath = setting.SavePath + "\\" + NameCheck(downloadlist[0].Singer);
                        break;
                    case 2:
                        savepath = setting.SavePath + "\\" + NameCheck(downloadlist[0].Singer) + "\\" + NameCheck(downloadlist[0].Album);
                        break;
                }
                if (Directory.Exists(savepath))
                    Directory.CreateDirectory(savepath);

                if (downloadlist[0].IfDownloadMusic)
                {
                    if (System.IO.File.Exists(savepath + "\\" + filename))
                    {
                        downloadlist[0].State = "音乐已存在";
                        UpdateDownloadPage();
                    }
                    else
                    {
                        using (WebClient wc = new WebClient())
                        {
                            try
                            {
                                wc.DownloadFile(downloadlist[0].Url, savepath + "\\" + filename);
                            }
                            catch
                            {
                                downloadlist[0].State = "音乐下载错误";
                                UpdateDownloadPage();
                                continue;
                            }
                        }
                    }
                }
                if (downloadlist[0].IfDownloadLrc)
                {
                    downloadlist[0].State = "正在下载歌词";
                    UpdateDownloadPage();
                    using (WebClient wc = new WebClient())
                    {
                        try
                        {
                            wc.DownloadFile(downloadlist[0].LrcUrl, savepath + "\\" + filename.Replace(".flac", ".lrc").Replace(".mp3", ".lrc"));
                        }
                        catch
                        {
                            downloadlist[0].State = "歌词下载错误";
                            UpdateDownloadPage();
                        }
                    }
                }
                if (downloadlist[0].IfDownloadPic)
                {
                    downloadlist[0].State = "正在下载图片";
                    UpdateDownloadPage();
                    using (WebClient wc = new WebClient())
                    {
                        try
                        {
                            wc.DownloadFile(downloadlist[0].PicUrl, savepath + "\\" + filename.Replace(".flac", ".jpg").Replace(".mp3", ".jpg"));
                        }
                        catch
                        {
                            downloadlist[0].State = "图片下载错误";
                            UpdateDownloadPage();
                        }
                    }
                }
                try
                {
                    if (filename.IndexOf(".mp3") != -1)
                    {
                        var tfile = TagLib.File.Create(savepath + "\\" + filename);
                        tfile.Tag.Title = downloadlist[0].Title;
                        tfile.Tag.Performers = new string[] { downloadlist[0].Singer };
                        tfile.Tag.Album = downloadlist[0].Album;
                        if (downloadlist[0].IfDownloadPic && System.IO.File.Exists(savepath + "\\" + filename.Replace(".flac", "").Replace(".mp3", "") + ".jpg"))
                        {
                            TagLib.Picture pic = new TagLib.Picture();
                            pic.Type = TagLib.PictureType.FrontCover;
                            pic.Description = "Cover";
                            pic.MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                            pic.Data = TagLib.ByteVector.FromPath(savepath + "\\" + filename.Replace(".flac", "").Replace(".mp3", "") + ".jpg");
                            tfile.Tag.Pictures = new TagLib.IPicture[] { pic };
                        }
                        tfile.Save();
                    }
                    else
                    {
                        var tfile = TagLib.Flac.File.Create(savepath + "\\" + filename);
                        tfile.Tag.Title = downloadlist[0].Title;
                        tfile.Tag.Performers = new string[] { downloadlist[0].Singer };
                        tfile.Tag.Album = downloadlist[0].Album;

                        if (downloadlist[0].IfDownloadPic && System.IO.File.Exists(savepath + "\\" + filename.Replace(".flac", "").Replace(".mp3", "") + ".jpg"))
                        {
                            TagLib.Picture pic = new TagLib.Picture();
                            pic.Type = TagLib.PictureType.FrontCover;
                            pic.Description = "Cover";
                            pic.MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                            pic.Data = TagLib.ByteVector.FromPath(savepath + "\\" + filename.Replace(".flac", "").Replace(".mp3", "") + ".jpg");
                            tfile.Tag.Pictures = new TagLib.IPicture[] { pic };
                        }
                        tfile.Save();
                    }
                }
                catch { }
                downloadlist[0].State = "下载完成";
                UpdateDownloadPage();
                downloadlist.RemoveAt(0);
            }
        }

        /// <summary>
        ///解析歌单，为了稳定每次请求200歌曲信息，所以解析歌单的方法分为两部分，这个方法根据歌曲数量分解请求
        /// </summary>
        public List<MusicInfo> GetMusicList(string Id)
        {
            Musiclist.Root musiclistjson = new Musiclist.Root();
            try
            {
                musiclistjson = JsonConvert.DeserializeObject<Musiclist.Root>(GetHTML(ApiUrl + "playlist/detail?id=" + Id));
            }
            catch
            {
                return null;
            }
            string ids = "";
            for (int i = 0; i < musiclistjson.playlist.trackIds.Count; i++)
            {
                ids += musiclistjson.playlist.trackIds[i].id.ToString() + ",";
            }
            ids = ids.Substring(0, ids.Length - 1);

            if (musiclistjson.playlist.trackIds.Count > 200)
            {
                string[] _id = ids.Split(',');

                int times = musiclistjson.playlist.trackIds.Count / 200;
                int remainder = musiclistjson.playlist.trackIds.Count % 200;
                if (remainder != 0)
                {
                    times++;
                }
                List<MusicInfo> re = new List<MusicInfo>();
                for (int i = 0; i < times; i++)
                {
                    string _ids = "";
                    if (i != times - 1)
                    {
                        for (int x = 0; x < 200; x++)
                        {
                            _ids += _id[i * 200 + x] + ",";
                        }
                    }
                    else
                    {
                        for (int x = 0; x < remainder; x++)
                        {
                            _ids += _id[i * 200 + x] + ",";
                        }
                    }
                    re.AddRange(_GetMusicList(_ids.Substring(0, _ids.Length - 1)));
                }
                return re;
            }
            else
            {
                return _GetMusicList(ids);
            }
        }

        /// <summary>
        /// 解析歌单的内部方法
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private List<MusicInfo> _GetMusicList(string ids)
        {
            List<Json.MusicInfo> ret = new List<Json.MusicInfo>();
            string _u = ApiUrl + "song/detail?ids=" + ids;
            string j = GetHTML(_u);
            Json.MusicDetails.Root mdr = JsonConvert.DeserializeObject<Json.MusicDetails.Root>(j);
            string u = ApiUrl + "song/url?id=" + ids + "&br=" + setting.DownloadQuality;
            Json.GetUrl.Root urls = JsonConvert.DeserializeObject<Json.GetUrl.Root>(GetHTML(u));
            for (int i = 0; i < mdr.songs.Count; i++)
            {
                string singer = "";
                List<string> singerid = new List<string>();
                string _url = "";

                for (int x = 0; x < mdr.songs[i].ar.Count; x++)
                {
                    singer += mdr.songs[i].ar[x].name + "、";
                    singerid.Add(mdr.songs[i].ar[x].id.ToString());
                }

                for (int x = 0; x < urls.data.Count; x++)
                {
                    if (urls.data[x].id == mdr.songs[i].id)
                    {
                        _url = urls.data[x].url;
                    }
                }

                MusicInfo mi = new MusicInfo()
                {
                    Album = mdr.songs[i].al.name,
                    Id = mdr.songs[i].id,
                    LrcUrl = ApiUrl + "lyric?id=" + mdr.songs[i].id,
                    PicUrl = mdr.songs[i].al.picUrl,
                    Singer = singer.Substring(0, singer.Length - 1),
                    Title = mdr.songs[i].name
                };
                ret.Add(mi);
            }
            return ret;
        }

        /// <summary>
        /// 解析专辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<MusicInfo> GetAlbum(string id)
        {
            List<MusicInfo> res = new List<MusicInfo>();
            string url = ApiUrl + "album?id=" + id;
            Album.Root json;
            try
            {
                json = JsonConvert.DeserializeObject<Album.Root>(GetHTML(url));
            }
            catch
            {
                return null;
            }
            for (int i = 0; i < json.songs.Count; i++)
            {
                string singer = "";
                for (int x = 0; x < json.songs[i].ar.Count; x++)
                {
                    singer += json.songs[i].ar[x].name + "、";
                }

                MusicInfo mi = new MusicInfo()
                {
                    Title = json.songs[i].name,
                    Album = json.album.name,
                    Id = json.songs[i].id,
                    LrcUrl = ApiUrl + "lyric?id=" + json.songs[i].id,
                    PicUrl = json.songs[i].al.picUrl,
                    Singer = singer.Substring(0, singer.Length - 1)
                };

                res.Add(mi);
            }
            return res;
        }
    }
}
