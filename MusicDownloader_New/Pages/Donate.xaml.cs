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

namespace MusicDownloader_New.Pages
{
    /// <summary>
    /// Donate.xaml 的交互逻辑
    /// </summary>
    public partial class Donate : Page
    {
        public Donate()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://cdn.nitian1207.cn/alipay.png");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start("http://cdn.nitian1207.cn/wechat.png");
        }
    }
}
