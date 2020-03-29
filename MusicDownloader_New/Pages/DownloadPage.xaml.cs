using MusicDownloader_New.Json;
using MusicDownloader_New.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace MusicDownloader_New
{
    /// <summary>
    /// DownloadPage.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadPage : Page
    {
        List<ListModel> listitem = new List<ListModel>();
        Music music = null;

        class ListModel
        {
            [DisplayName("标题")]
            public string Title { get; set; }
            [DisplayName("歌手")]
            public string Singer { get; set; }
            [DisplayName("专辑")]
            public string Album { get; set; }
            [DisplayName("状态")]
            public string State { get; set; }
        }

        public DownloadPage(Music m)
        {
            InitializeComponent();
            //Thread th_Update = new Thread(UpdateList);
            //th_Update.Start();
            music = m;
            music.UpdateDownloadPage += UpdateList;
        }

        public void UpdateList()
        {
            listitem.Clear();
            foreach (DownloadList d in music.downloadlist)
            {
                listitem.Add(new ListModel { Album = d.Album, Singer = d.Singer, State = d.State, Title = d.Title });
            }
            Dispatcher.Invoke(new Action(() =>
            {
                List.ItemsSource = listitem;
                List.Items.Refresh();
            }));

        }

        private void Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(music.setting.SavePath);
        }
    }
}
