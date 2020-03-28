using MusicDownloader_New.Json;
using MusicDownloader_New.Library;
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

namespace MusicDownloader_New.Pages
{
    /// <summary>
    /// Search.xaml 的交互逻辑
    /// </summary>
    public partial class SearchPage : Page
    {
        List<MusicInfo> musicinfo = null;
        Music music;
        Setting setting;
        public List<SearchListItemModel> SearchListItem = new List<SearchListItemModel>();

        #region 列表绑定
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

        public SearchPage(Music m, Setting s)
        {
            music = m;
            setting = s;
            InitializeComponent();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text != "")
            {
                Search(searchTextBox.Text);
            }
        }
        private async void Search(string key)
        {
            var pb = PendingBox.Show("搜索中...", null, false, Application.Current.MainWindow, new PendingBoxConfigurations()
            {
                MaxHeight = 160,
                MinWidth = 400
            });
            SearchListItem.Clear();
            musicinfo?.Clear();
            await Task.Run(() =>
            {
                musicinfo = music.Search(key);
            });
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

        private void searchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchButton_Click(this, new RoutedEventArgs());
            }
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

        private async void Download()
        {
            var pb = PendingBox.Show("搜索中...", null, false, Application.Current.MainWindow, new PendingBoxConfigurations()
            {
                MaxHeight = 160,
                MinWidth = 400
            });
            List<DownloadList> dl = new List<DownloadList>();
            for (int i = 0; i < SearchListItem.Count; i++)
            {
                if (SearchListItem[i].IsSelected)
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
                    });
                }
            }
            await Task.Run(() =>
            {
                music.Download(dl);
            });
            pb.Close();
        }
    }
}