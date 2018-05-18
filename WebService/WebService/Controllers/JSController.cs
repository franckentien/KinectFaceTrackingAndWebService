using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;
using WebService.Models;

namespace WebService.Controllers
{
    public class JSController : ApiController
    {
        public static bool JSflaginactivity = false;
        public static int JSflagRUserious = 0;

        public static int KTHappyness = 0;
        public static int KTLookAway = 0;

        public static void clearVariable()
        {
            JSflaginactivity = false;
            JSflagRUserious = 0;
            KTHappyness = 0;
            KTLookAway = 0;
        }

        [HttpPost]
        public bool AddEmpDetails(string json)
        {
            //string createText = "Hello and Welcome" + Environment.NewLine;

            //File.WriteAllText("C:\\Users\\franc\\Documents\\Yan.txt",json);

            switch (json)
            {
                case "1":
                    JSflagRUserious++;
                    break;
                case "2":
                    JSflaginactivity = true;
                    break;
                case "3":
                    WebApiConfig.needhelp = true;
                    break;
            }

            return true;
        }
        [HttpGet]
        public string GetEmpDetails(string json)
        {

            var numbers = json.Split('|').Select(int.Parse).ToList();

            //File.WriteAllText("C:\\Users\\franc\\Documents\\franck.txt", numbers[0].ToString());

            KTHappyness += numbers[0];
            KTLookAway += numbers[1];

            return "OK";

        }
        [HttpDelete]
        public string DeleteEmpDetails(string id)
        {
            return "Employee details deleted having Id " + id;

        }
        [HttpPut]
        public string UpdateEmpDetails(string Name, String Id)
        {
            return "Employee details Updated with Name " + Name + " and Id " + Id;

        }
    }
}
