using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenCvSharp;

namespace azurlane_yanxi_wpf
{
    public class GameController
    {
        public _Config c;
        public ADBController a;
        public ImageHanddle imageHanddle;

        public GameController()
        {
            c = Config.init();
            a = ADBController.init();
            imageHanddle = new ImageHanddle();
        }

        public int check()
        {
            a.stopGame();
            Thread.Sleep(c.tapIntervalMilliSeconds);
            a.startGame();
            Thread.Sleep(c.startIntervalSeconds * 1000);
            a.tap(1713, 379);
            Thread.Sleep(c.tapIntervalMilliSeconds * 2);
            a.tap(1623, 537);
            Thread.Sleep(c.tapIntervalMilliSeconds);
            a.tap(1753, 1013);
            Thread.Sleep(c.tapIntervalMilliSeconds);
            var img = a.getScreen();
            int rank = imageHanddle.get1Rank(img);
            Console.WriteLine(rank);
            a.stopGame();
            return rank;
        }

        public void testImg(string imgPath)
        {
            var img = new Mat(imgPath, ImreadModes.Grayscale);
            int rank = imageHanddle.get1Rank(img);
            Console.WriteLine(rank);
        }

        public int get1Rank()
        {
            var img = a.getScreen();
            var imageHanddle = new ImageHanddle();
            return imageHanddle.get1Rank(img);
        }
    }
}
