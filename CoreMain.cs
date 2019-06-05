using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Threading;


namespace azurlane_yanxi_wpf
{
    class CoreMain
    {
        public _Config c;
        public GameController g;
        public Thread t;
        Label label_lastRank;
        SetRankDelegate SetRankDelegateInstance;
        public CoreMain(SetRankDelegate SetRankDelegateInstance,Label label_lastRank)
        {
            c = Config.init();
            g = new GameController();
            t = new Thread(new ThreadStart(doLoop));
            this.SetRankDelegateInstance = SetRankDelegateInstance;
            this.label_lastRank = label_lastRank;
        }

        public void startT()
        {
            if (t.IsAlive)
            {
                return;
            }
            t.Start();
        }
        public void endT()
        {
            if (!t.IsAlive)
            {
                return;
            }
            t.Abort();
            t = new Thread(new ThreadStart(doLoop));
        }
        public void restartT()
        {
            endT();
            startT();
        }

        public void doLoop()
        {
            int oldRank = 0,newRank=0;

            while (true)
            {
                newRank = g.check();

                if(oldRank == 0)
                {
                    oldRank = newRank;
                }
                else
                {
                    if(newRank < oldRank)
                    {
                        notice(newRank);
                        oldRank = newRank;
                    }
                }

                label_lastRank.Dispatcher.BeginInvoke(SetRankDelegateInstance, new object[] { newRank });

                Thread.Sleep(c.checkIntervalSeconds);
                
                //label_lastRank.Content = rank;
            }
        }

        public void notice(int rank)
        {

        }
    }
}
