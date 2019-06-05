using System;

using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace azurlane_yanxi_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public delegate void SetRankDelegate(int x);

    public partial class MainWindow
    {
        CoreMain coreMain;
        bool _status = false;
        string workPath;
        

        public bool Status {
            get
            {
                return _status;
            }
            set
            {
                if(_status == value)
                {
                    return;
                }
                else
                {
                    _status = value;
                    if (value)
                    {
                        statusValLabel.Content = Static.status1; 
                    }
                    else
                    {
                        statusValLabel.Content = Static.status0;
                    }
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            SetRankDelegate setRank = setRankMethod;
            coreMain = new CoreMain(setRank, label_lastRank);
            workPath = System.Environment.CurrentDirectory;
            image_last.Source = new BitmapImage(new Uri(workPath + "/null.png"));
        }

        private void startButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Status = true;
            coreMain.startT();
            
        }

        private void endButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Status = false;
            coreMain.endT();
        }

        private void restartButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        public void setRankMethod(int rank)
        {
            label_lastRank.Content = rank.ToString();
        }
        public void setRankNotInvoke(int rank)
        {
            
        }

        private void configConfig()
        {

        }
    }
}