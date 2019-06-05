using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using OpenCvSharp;

namespace azurlane_yanxi_wpf
{
    public struct size
    {
        public bool getted { get; set; }
        public int wid { get; set; }
        public int hei { get; set; }

        public size(int wid, int hei)
        {
            this.wid = wid;
            this.hei = hei;
            this.getted = true;
        }
    }
    public class ADBController
    {
        _Config c = Config.init();
        private Process p_cmd;
        private Process p_adbShell;
        private size _size;
        private static ADBController _instance = null;
        private ADBController()
        {
            p_cmd = _initCMD();
            p_adbShell = _initADBShell();

            _size.getted = false;
            getSize();
        }

        public static ADBController init()
        {
            if (_instance is null)
            {
                _instance = new ADBController();
            }

            return _instance;
        }

        private Process _initCMD()
        {
            var p = new Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = c.cmdPath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            return p;
        }

        private Process _initADBShell()
        {
            var p = new Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = c.adbPath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            if (c.deviceName == "")
            {
                p.StartInfo.Arguments = "-e shell";
            }
            else
            {
                p.StartInfo.Arguments = $"-e -s {c.deviceName} shell";
            }

            return p;
        }

        public string adb(string cmd)
        {
            this.p_cmd.Start();
            this.p_cmd.StandardInput.WriteLine(c.adbPath + " -e " + cmd);
            this.p_cmd.StandardInput.WriteLine("exit");
            string res = string.Empty;
            res = this.p_cmd.StandardOutput.ReadToEnd();
            this.p_cmd.Close();
            return res;
        }

        public string shell(string cmd)
        {
            p_adbShell.Start();
            p_adbShell.StandardInput.WriteLine(cmd);
            p_adbShell.StandardInput.WriteLine("exit");

            string res = string.Empty;
            res = p_adbShell.StandardOutput.ReadToEnd();
            p_adbShell.Close();
            return res;
        }


        public size getSize2()
        {
            if (_size.getted)
            {
                return _size;
            }
            var res = adb("shell wm size");

            var s = Regex.Match(res, @"(?<wid>\d+)x(?<hei>\d+)");
            //            Console.WriteLine(s.Groups["wid"]);
            //            Console.WriteLine(s.Groups["hei"]);
            _size.wid = Convert.ToInt16(s.Groups["wid"].ToString());
            _size.hei = Convert.ToInt16(s.Groups["hei"].ToString());
            return _size;
        }

        public size getSize()
        {
            if (_size.getted)
            {
                return _size;
            }
            var res = shell("wm size");
            var s = Regex.Match(res, @"(?<wid>\d+)x(?<hei>\d+)");
            Console.WriteLine($"wm size: {s.Groups["wid"]} x {s.Groups["hei"]}");
            _size.wid = Convert.ToInt16(s.Groups["wid"].ToString());
            _size.hei = Convert.ToInt16(s.Groups["hei"].ToString());
            return _size;
        }

        public void startGame()
        {
            shell($"am start -n {c.mainActivityName}");
        }

        public void stopGame()
        {
            shell($"am force-stop {c.packageName}");
        }

        public void tap(int x, int y)
        {
            int realX = x * _size.wid / 1920;
            int realY = y * _size.hei / 1080;
            shell($"input tap {realX} {realY}");
        }

        public void back()
        {
            shell("input keyevent 4");
        }

        public Mat getScreen()
        {
            shell($"screencap -p /sdcard/Pictures/{c.tmpImgFileName}");
            Thread.Sleep(c.fetchImgIntervalMilliSeconds);
            Console.WriteLine($@"{c.imgSharePath}\{c.tmpImgFileName}");
            var img = new Mat($@"{c.imgSharePath}\{c.tmpImgFileName}", ImreadModes.Grayscale);

            return img;
        }

    }
}
