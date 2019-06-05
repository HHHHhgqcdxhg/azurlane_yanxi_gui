using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace azurlane_yanxi_wpf
{
    public class _Config
    {
        public String adbPath { get; set; }
        public String cmdPath { get; set; }
        public String packageName { get; set; }
        public String mainActivityName { get; set; }
        public String tmpImgFileName { get; set; }
        public String imgSharePath { get; set; }
        public String deviceName { get; set; }
        public int checkIntervalSeconds { get; set; }
        public int tapIntervalMilliSeconds { get; set; }
        public int fetchImgIntervalMilliSeconds { get; set; }
        public int startIntervalSeconds { get; set; }
    }

    public class Config
    {
        private static _Config config = null;

        private Config()
        {
        }

        public static _Config init()
        {
            if (config is null)
            {
                config = JsonConvert.DeserializeObject<_Config>(File.ReadAllText(@"./config.json"));
            }

            return config;
        }

    }
}
