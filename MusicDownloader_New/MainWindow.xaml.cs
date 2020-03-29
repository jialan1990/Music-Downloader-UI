using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
using MusicDownloader_New.Pages;
using Panuon.UI.Silver;
using MusicDownloader_New.Library;
using MusicDownloader_New.Json;
using Panuon.UI.Silver.Core;

namespace MusicDownloader_New
{
    public partial class MainWindow : WindowX
    {
        Music music = null;
        Setting setting;
        List<DownloadList> downloadlist = new List<DownloadList>();
        Page HomePage;
        Page DownloadPage;
        Page SettingPage;
        Page Donate = new Donate();

        #region 界面
        private void BlogButton_Click(object sender, RoutedEventArgs e) => Process.Start("https://www.nitian1207.cn/");
        private void LeftMenu_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (frame != null)
            {
                switch (((System.Windows.Controls.HeaderedItemsControl)e.NewValue).Header)
                {
                    case "主页":
                        frame.Content = HomePage;
                        break;
                    case "下载":
                        frame.Content = DownloadPage;
                        break;
                    case "设置":
                        frame.Content = SettingPage = new SettingPage(setting);
                        break;
                    case "赞助":
                        frame.Content = Donate;
                        break;
                }
            }
        }
        #endregion

        #region 事件
        private void NotifyUpdate()
        {
            var result = MessageBoxX.Show("检测到新版,是否更新", null, Application.Current.MainWindow, MessageBoxButton.YesNo, new MessageBoxXConfigurations()
            {
                MessageBoxIcon = MessageBoxIcon.Warning
            });
            if (result == MessageBoxResult.Yes)
            {
                Process.Start("https://www.nitian1207.cn/archives/86");
            }
        }

        private void NotifyError()
        {
            var result = MessageBoxX.Show("连接服务器错误", null, Application.Current.MainWindow, MessageBoxButton.OK, new MessageBoxXConfigurations()
            {
                MessageBoxIcon = MessageBoxIcon.Error
            });
            Environment.Exit(0);
        }
        #endregion

        public MainWindow()
        {
            setting = new Setting()
            {
                SavePath = Tool.Config.Read("SavePath") ?? Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                DownloadQuality = Tool.Config.Read("DownloadQuality") ?? "999000",
                IfDownloadLrc = Boolean.Parse(Tool.Config.Read("IfDownloadLrc") ?? "true"),
                IfDownloadPic = Boolean.Parse(Tool.Config.Read("IfDownloadPic") ?? "true"),
                SaveNameStyle = int.Parse(Tool.Config.Read("SaveNameStyle") ?? "0"),
                SavePathStyle = int.Parse(Tool.Config.Read("SavePathStyle") ?? "0"),
                SearchQuantity = Tool.Config.Read("SearchQuantity") ?? "100"
            };
            music = new Music(setting);
            HomePage = new SearchPage(music, setting);
            DownloadPage = new DownloadPage(music);

            music.NotifyConnectError += NotifyError;
            music.NotifyUpdate += NotifyUpdate;
            
            InitializeComponent();
            frame.Content = HomePage;
        }
    }
}
