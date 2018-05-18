using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using Newtonsoft.Json;

namespace WS2
{
    public static class TempResult
    {
        public static int[] test = new int[2];


//        FaceProperty_Happy = 0
//        FaceProperty_LookingAway = 7

        public static string displayresult()
        {
            
            string rst = String.Empty;
                rst += "Happy :" + (test[0] / 30.0);
            rst += "LookingAway :" + (test[1] / 30.0);






            return rst;


        }

        public static string getjson()
        {

            test[0] = (int) (test[0] *100f / 30f);
            test[1] = (int)(test[1] * 100f / 30f);

            string json = test[0] + "|" + test[1];

            Console.WriteLine(json);

            test[0] = 0;
            test[1] = 0;

            return json;
        }




    }
}