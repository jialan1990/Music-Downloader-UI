using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

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
    }
}
