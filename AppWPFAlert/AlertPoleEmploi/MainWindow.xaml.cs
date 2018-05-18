using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

namespace AlertPoleEmploi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static readonly HttpClient client = new HttpClient();



        public MainWindow()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(Mytick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();

            InitializeComponent();
            helpOnTheWay();



            
        }

        private async void Mytick(object sender, EventArgs e)
        {
            var responseString = await client.GetStringAsync("http://localhost:59378/api/Feedback/0");

            //Console.Write(responseString);

            if (needHelp == false && responseString == "true")
            {
                setNeedHelp();
            }
            

        }

        public bool needHelp = false;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //setNeedHelp();
            helpOnTheWay();
        }

        public void setNeedHelp() {
            btnAide.Background = Brushes.Red;
        }

        public void helpOnTheWay()
        {
            btnAide.Background = Brushes.Green;
        }
    }
}
