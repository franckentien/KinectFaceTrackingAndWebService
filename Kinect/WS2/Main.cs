using System.Threading;
using System.Net.Http;

namespace WS2
{
    using System;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public class Mainclass
    {

        private static readonly HttpClient client = new HttpClient();

        static void Main()
        {
            Console.Title = "Kinect Console";

            KinectControl c = new KinectControl();

            c.GetSensor();
            c.OpenReader();

            Timer t = new Timer(ComputeBoundOp, null, 0, 1000);

            while (true)
            {
                var k = Console.ReadKey(true).Key;

                if (k == ConsoleKey.X)
                {
                    c.CloseReader();
                    c.ReleaseSensor();
                    t.Dispose();
                    break;
                }
            }
        }

        // This method's signature must match the TimerCallback delegate
        private static async void ComputeBoundOp(Object state)
        {
      
            var responseString = await client.GetStringAsync("http://localhost:59378/api/JS/" + TempResult.getjson());


            Console.WriteLine(responseString);
        }

    }


}


