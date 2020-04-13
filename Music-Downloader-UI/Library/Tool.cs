using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;

namespace MusicDownloader.Library
{
    public class Tool
    {
        public class Config
        {
            static public void Write(string key, string value)
            {
                bool Exist = false;
                var conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                foreach (string s in conf.AppSettings.Settings.AllKeys)
                {
                    if (s == key)
                    {
                        conf.AppSettings.Settings[s].Value = value;
                        Exist = true;
                    }
                }
                if (!Exist)
                {
                    conf.AppSettings.Settings.Add(key, value);
                }
                conf.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }

            static public string Read(string key)
            {
                ConfigurationManager.RefreshSection("appSettings");
                var conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                return conf.AppSettings.Settings[key]?.Value;
            }

            static public void Remove(string key)
            {
                var conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                conf.AppSettings.Settings.Remove(key);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        static public List<string> GetMidText(string text, string left, string right, bool ifIncludeLR = true)//ifIncludeLR是是否包括用来定位的前后字符串
        {
            int leftindex = 0;
            int rightindex = -right.Length;
            List<string> re = new List<string>();
            while (text.IndexOf(left, rightindex + right.Length) != -1)
            {
                leftindex = text.IndexOf(left, rightindex + right.Length);
                rightindex = text.IndexOf(right, leftindex + left.Length);
                if (ifIncludeLR)
                {
                    re.Add(left + text.Substring(leftindex + left.Length, rightindex - leftindex - left.Length) + right);
                }
                else
                {
                    re.Add(text.Substring(leftindex + left.Length, rightindex - leftindex - left.Length));
                }
            }
            return re;
        }

        static public string GetRealUrl(string url)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = 20000;
                string _url = req.GetResponse().ResponseUri.AbsoluteUri;
                req.Abort();
                return _url;
            }
            catch
            {
                return null;
            }
        }
    }
}
