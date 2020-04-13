using MusicDownloader.Json;
using MusicDownloader.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using System.Data;
using System.Net;
using System.Collections;

namespace MusicDownloader.Pages
{
    public partial class SearchPage : Page
    {
        List<MusicInfo> musicinfo = null;
        Music music;
        Setting setting;
        public List<SearchListItemModel> SearchListItem = new List<SearchListItemModel>();

        #region 列表绑定模板
        public class SearchListItemModel
        {
            [DisplayName(" ")]
            public bool IsSelected { get; set; }
            [DisplayName("标题")]
            public string Title { get; set; }
            [DisplayName("歌手")]
            public string Singer { get; set; }
            [DisplayName("专辑")]
            public string Album { get; set; }
        }
        #endregion

        #region 事件
        private void menu_DownloadSelectLrc_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Download(true);
        }

        private void menu_DownloadSelectPic_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Download(false, true);
        }

        private void searchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                searchButton_Click(this, new RoutedEventArgs());
        }

        private void menu_SelectAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (SearchListItemModel m in SearchListItem)
            {
                m.IsSelected = true;
            }
            List.Items.Refresh();
        }

        private void menu_FanSelect_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (SearchListItemModel m in SearchListItem)
            {
                m.IsSelected = !m.IsSelected;
            }
            List.Items.Refresh();
        }

        private void menu_DownloadSelect_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Download();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text?.Replace(" ", "") != "")
            {
                Search(searchTextBox.Text);
            }
        }

        private void musiclistButton_Click(object sender, RoutedEventArgs e)
        {
            if (musiclistTextBox.Text?.Replace(" ", "") != "")
            {
                string id = musiclistTextBox.Text;
                if (apiComboBox.SelectedIndex == 0)
                {
                    if (musiclistTextBox.Text.IndexOf("http") != -1)
                    {
                        string url = musiclistTextBox.Text;
                        if (url.IndexOf("userid") != -1)
                        {
                            id = url.Substring(url.IndexOf("playlist?id=") + "playlist?id=".Length, url.IndexOf("&userid") - url.IndexOf("playlist?id=") - "playlist?id=".Length);
                        }
                        else
                        {
                            id = url.Substring(url.IndexOf("playlist?id=") + "playlist?id=".Length);
                        }
                    }
                }
                if (apiComboBox.SelectedIndex == 1)
                {
                    if (id.IndexOf("https://c.y.qq.com/") != -1)
                    {
                        string qqid = Tool.GetRealUrl(id);
                        List<string> i = Tool.GetMidText(qqid, "id=", "&", false);
                        id = i[0].ToString();
                    }
                    if (id.IndexOf("https://y.qq.com/") != -1)
                    {
                        List<string> i = Tool.GetMidText(id, "playlist/", ".html", false);
                        id = i[0].ToString();
                    }
                }
                GetMusicList(id);
            }
        }

        private void musiclistTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                musiclistButton_Click(this, new RoutedEventArgs());
            if (!((74 <= (int)e.Key && (int)e.Key <= 83) || (34 <= (int)e.Key && (int)e.Key <= 43) || e.Key == Key.Back))
            {
                e.Handled = true;
            }
        }

        private void albumTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                albumButton_Click(this, new RoutedEventArgs());
            if (!((74 <= (int)e.Key && (int)e.Key <= 83) || (34 <= (int)e.Key && (int)e.Key <= 43) || e.Key == Key.Back))
            {
                e.Handled = true;
            }
        }

        private void albumButton_Click(object sender, RoutedEventArgs e)
        {
            if (albumTextBox.Text?.Replace(" ", "") != "")
            {
                string id = albumTextBox.Text;
                if (apiComboBox.SelectedIndex == 0)
                {

                    if (albumTextBox.Text.IndexOf("http") != -1)
                    {
                        string url = albumTextBox.Text;
                        if (url.IndexOf("userid") != -1)
                        {
                            id = url.Substring(url.IndexOf("album?id=") + "album?id=".Length, url.IndexOf("&userid") - url.IndexOf("album?id=") - "album?id=".Length);
                        }
                        else
                        {
                            id = url.Substring(url.IndexOf("album?id=") + "album?id=".Length);
                        }
                    }
                }
                if (apiComboBox.SelectedIndex == 1)
                {
                    if (id.IndexOf("https://c.y.qq.com/") != -1)
                    {
                        MessageBoxX.Show("请将链接复制到浏览器打开后再复制回程序", "提示", configurations: new MessageBoxXConfigurations { MessageBoxIcon = MessageBoxIcon.Warning });
                        return;
                    }
                    if (id.IndexOf("https://y.qq.com/") != -1)
                    {
                        List<string> i = Tool.GetMidText(id, "album/", ".html", false);
                        id = i[0].ToString();
                    }
                }
                GetAblum(id);
            }
        }

        /// <summary>
        /// 热歌榜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            GetMusicList("3778678");
        }

        /// <summary>
        /// 新歌榜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            GetMusicList("3779629");
        }

        /// <summary>
        /// 飙升榜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_PreviewMouseDown_2(object sender, MouseButtonEventArgs e)
        {
            GetMusicList("19723756");
        }

        /// <summary>
        /// 原创榜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_PreviewMouseDown_3(object sender, MouseButtonEventArgs e)
        {
            GetMusicList("2884035");
        }
        #endregion

        public SearchPage(Music m, Setting s)
        {
            music = m;
            setting = s;
            InitializeComponent();
        }

        private async void Search(string key)
        {
            var pb = PendingBox.Show("搜索中...", null, false, Application.Current.MainWindow, new PendingBoxConfigurations()
            {
                MaxHeight = 160,
                MinWidth = 400
            });
            try
            {
                SearchListItem.Clear();
                musicinfo?.Clear();
                int api = apiComboBox.SelectedIndex + 1;
                await Task.Run(() =>
                {
                    musicinfo = music.Search(key, api);
                });
                //musicinfo = music.Search(key, apiComboBox.SelectedIndex + 1);
                if (musicinfo == null)
                {
                    pb.Close();
                    MessageBoxX.Show("搜索错误", "警告", Application.Current.MainWindow, MessageBoxButton.OK, new MessageBoxXConfigurations() { MessageBoxIcon = MessageBoxIcon.Error });
                    return;
                }
                foreach (MusicInfo m in musicinfo)
                {
                    SearchListItemModel mod = new SearchListItemModel()
                    {
                        Album = m.Album,
                        Singer = m.Singer,
                        IsSelected = false,
                        Title = m.Title
                    };
                    SearchListItem.Add(mod);
                }
                List.ItemsSource = SearchListItem;
                List.Items.Refresh();
                pb.Close();
            }
            catch
            {
                pb.Close();
                MessageBoxX.Show("搜索错误", "警告", Application.Current.MainWindow, MessageBoxButton.OK, new MessageBoxXConfigurations() { MessageBoxIcon = MessageBoxIcon.Error });
            }
        }

        private async void GetMusicList(string id)
        {
            var pb = PendingBox.Show("解析中...", null, false, Application.Current.MainWindow, new PendingBoxConfigurations()
            {
                MaxHeight = 160,
                MinWidth = 400
            });
            try
            {
                SearchListItem.Clear();
                musicinfo?.Clear();
                int api = apiComboBox.SelectedIndex + 1;
                await Task.Run(() =>
                {
                    musicinfo = music.GetMusicList(id, api);
                });
                if (musicinfo == null)
                {
                    pb.Close();
                    MessageBoxX.Show("解析错误", "警告", Application.Current.MainWindow, MessageBoxButton.OK, new MessageBoxXConfigurations() { MessageBoxIcon = MessageBoxIcon.Error });
                    return;
                }
                foreach (MusicInfo m in musicinfo)
                {
                    SearchListItemModel mod = new SearchListItemModel()
                    {
                        Album = m.Album,
                        Singer = m.Singer,
                        IsSelected = false,
                        Title = m.Title
                    };
                    SearchListItem.Add(mod);
                }
                List.ItemsSource = SearchListItem;
                List.Items.Refresh();
                pb.Close();
            }
            catch
            {
                pb.Close();
                MessageBoxX.Show("解析错误", configurations: new MessageBoxXConfigurations() { MessageBoxIcon = MessageBoxIcon.Error });
            }
        }

        private async void Download(bool ifonlydownloadlrc = false, bool ifonlydownloadpic = false)
        {
            List<DownloadList> dl = new List<DownloadList>();
            for (int i = 0; i < SearchListItem.Count; i++)
            {
                if (SearchListItem[i].IsSelected)
                {
                    if (ifonlydownloadlrc)
                    {
                        dl.Add(new DownloadList
                        {
                            Id = musicinfo[i].Id.ToString(),
                            IfDownloadLrc = true,
                            IfDownloadMusic = false,
                            IfDownloadPic = false,
                            Album = musicinfo[i].Album,
                            LrcUrl = musicinfo[i].LrcUrl,
                            PicUrl = musicinfo[i].PicUrl,
                            Quality = setting.DownloadQuality,
                            Singer = musicinfo[i].Singer,
                            Title = musicinfo[i].Title,
                            Api = musicinfo[i].Api,
                            strMediaMid = musicinfo[i].strMediaMid
                        });
                    }
                    else if (ifonlydownloadpic)
                    {
                        dl.Add(new DownloadList
                        {
                            Id = musicinfo[i].Id,
                            IfDownloadLrc = false,
                            IfDownloadMusic = false,
                            IfDownloadPic = true,
                            Album = musicinfo[i].Album,
                            LrcUrl = musicinfo[i].LrcUrl,
                            PicUrl = musicinfo[i].PicUrl,
                            Quality = setting.DownloadQuality,
                            Singer = musicinfo[i].Singer,
                            Title = musicinfo[i].Title,
                            Api = musicinfo[i].Api,
                            strMediaMid = musicinfo[i].strMediaMid
                        });
                    }
                    else
                    {
                        dl.Add(new DownloadList
                        {
                            Id = musicinfo[i].Id,
                            IfDownloadLrc = setting.IfDownloadLrc,
                            IfDownloadMusic = true,
                            IfDownloadPic = setting.IfDownloadPic,
                            Album = musicinfo[i].Album,
                            LrcUrl = musicinfo[i].LrcUrl,
                            PicUrl = musicinfo[i].PicUrl,
                            Quality = setting.DownloadQuality,
                            Singer = musicinfo[i].Singer,
                            Title = musicinfo[i].Title,
                            Api = musicinfo[i].Api,
                            strMediaMid = musicinfo[i].strMediaMid
                        });
                    }
                }
            }
            if (dl.Count != 0)
            {
                int api = apiComboBox.SelectedIndex + 1;
                var pb = PendingBox.Show("请求处理中...", null, false, Application.Current.MainWindow, new PendingBoxConfigurations()
                {
                    MaxHeight = 160,
                    MinWidth = 400
                });
                await Task.Run(() =>
                {
                    music.Download(dl, api);
                });
                pb.Close();
            }
        }

        private async void GetAblum(string id)
        {
            var pb = PendingBox.Show("解析中...", null, false, Application.Current.MainWindow, new PendingBoxConfigurations()
            {
                MaxHeight = 160,
                MinWidth = 400
            });
            try
            {
                SearchListItem.Clear();
                musicinfo?.Clear();
                int api = apiComboBox.SelectedIndex + 1;
                await Task.Run(() =>
                {
                    musicinfo = music.GetAlbum(id, api);
                });
                if (musicinfo == null)
                {
                    pb.Close();
                    MessageBoxX.Show("解析错误", "警告", configurations: new MessageBoxXConfigurations() { MessageBoxIcon = MessageBoxIcon.Error });
                    return;
                }
                foreach (MusicInfo m in musicinfo)
                {
                    SearchListItemModel mod = new SearchListItemModel()
                    {
                        Album = m.Album,
                        Singer = m.Singer,
                        IsSelected = false,
                        Title = m.Title
                    };
                    SearchListItem.Add(mod);
                }
                List.ItemsSource = SearchListItem;
                List.Items.Refresh();
                pb.Close();
            }
            catch
            {
                pb.Close();
                MessageBoxX.Show("解析错误", "警告", configurations: new MessageBoxXConfigurations() { MessageBoxIcon = MessageBoxIcon.Error });
            }
        }
    }
}