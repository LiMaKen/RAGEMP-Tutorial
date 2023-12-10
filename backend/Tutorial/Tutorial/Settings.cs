using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkAPI;

namespace Tutorial
{
    class Settings
    {
        public static Settings _Settings;
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }

        public static bool LoadServerSettings()
        {
            string directory = "./serverdata/settings.json";
            if(File.Exists(directory))
            {
                string settings = File.ReadAllText(directory);
                _Settings = NAPI.Util.FromJson<Settings>(settings);
                NAPI.Util.ConsoleOutput("[Settings] -> Dữ liệu Server được tải thành công!");
                return true;
            }
            else
            {
                NAPI.Util.ConsoleOutput("[Settings] -> Không thể tải Server!");
                NAPI.Task.Run(() =>
                {
                   Environment.Exit(0);
                }, delayTime: 5000);
                return false;
            }
        }
    }
}
