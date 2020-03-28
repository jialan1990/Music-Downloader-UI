using System;
using System.Collections.Generic;
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
using System.Windows.Forms;
using MusicDownloader_New.Json;
using MusicDownloader_New.Library;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using MessageBoxIcon = Panuon.UI.Silver.MessageBoxIcon;
using Application = System.Windows.Application;

namespace MusicDownloader_New.Pages
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        Setting setting;

        public SettingPage(Setting s)
        {
            setting = s;
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                savePathTextBox.Text = fbd.SelectedPath;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            savePathTextBox.Text = setting.SavePath;
            searchQuantityTextBox.Text = setting.SearchQuantity;
            switch (setting.DownloadQuality)
            {
                case "999000":
                    qualityComboBox.SelectedIndex = 0;
                    break;
                case "320000":
                    qualityComboBox.SelectedIndex = 1;
                    break;
                case "128000":
                    qualityComboBox.SelectedIndex = 2;
                    break;
            }
            nameStyleComboBox.SelectedIndex = setting.SaveNameStyle;
            pathStyleComboBox.SelectedIndex = setting.SavePathStyle;
            lrcCheckBox.IsChecked = setting.IfDownloadLrc;
            picCheckBox.IsChecked = setting.IfDownloadPic;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (savePathTextBox.Text == "")
            {
                MessageBoxX.Show("路径不能为空", "Error", Application.Current.MainWindow, MessageBoxButton.OK, new MessageBoxXConfigurations()
                {
                    MessageBoxIcon = MessageBoxIcon.Error,
                    MinWidth = 400,
                    MinHeight = 160
                });
                return;
            }
            if (searchQuantityTextBox.Text == "")
            {
                MessageBoxX.Show("搜索数量不能为空", "Error", Application.Current.MainWindow, MessageBoxButton.OK, new MessageBoxXConfigurations()
                {
                    MessageBoxIcon = MessageBoxIcon.Error,
                    MinWidth = 400,
                    MinHeight = 160
                });
                return;
            }
            Tool.Config.Write("SavePath", savePathTextBox.Text);
            Tool.Config.Write("DownloadQuality", ((System.Windows.Controls.ContentControl)qualityComboBox.SelectedValue).Content.ToString().Substring(("无损(").Length, 6));
            Tool.Config.Write("IfDownloadLrc", lrcCheckBox.IsChecked.ToString());
            Tool.Config.Write("IfDownloadPic", picCheckBox.IsChecked.ToString());
            Tool.Config.Write("SaveNameStyle", nameStyleComboBox.SelectedIndex.ToString());
            Tool.Config.Write("SavePathStyle", pathStyleComboBox.SelectedIndex.ToString());
            Tool.Config.Write("SearchQuantity", searchQuantityTextBox.Text);
            setting.SavePath = savePathTextBox.Text;
            setting.DownloadQuality = ((System.Windows.Controls.ContentControl)qualityComboBox.SelectedValue).Content.ToString().Substring(("无损(").Length, "999000".Length);
            setting.IfDownloadLrc = lrcCheckBox.IsChecked ?? false;
            setting.IfDownloadPic = picCheckBox.IsChecked ?? false;
            setting.SaveNameStyle = nameStyleComboBox.SelectedIndex;
            setting.SavePathStyle = pathStyleComboBox.SelectedIndex;
            setting.SearchQuantity = searchQuantityTextBox.Text;
            MessageBoxX.Show("设置保存成功", "Success", Application.Current.MainWindow, MessageBoxButton.OK, new MessageBoxXConfigurations()
            {
                MessageBoxIcon = MessageBoxIcon.Success,
                MinWidth = 400,
                MinHeight = 160
            });
        }

        private void searchQuantityTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!((74 <= (int)e.Key && (int)e.Key <= 83) || (34 <= (int)e.Key && (int)e.Key <= 43) || e.Key == Key.Back))
            {
                e.Handled = true;
            }
        }
    }
}
