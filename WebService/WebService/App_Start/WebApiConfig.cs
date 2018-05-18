using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Http;
using WebService.Controllers;
using WebService.Models;

namespace WebService
{
    public static class WebApiConfig
    {

        private static int inactivity = 0;

        public static bool needhelp = false;


        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{json}",
                defaults: new { id = RouteParameter.Optional }
            );

            Timer t = new Timer(Mytick, null, 0, 60000);
        }

        // This method's signature must match the TimerCallback delegate
        private static async void Mytick(Object state)
        {
            JSController.KTHappyness = JSController.KTHappyness / 60;
            JSController.KTLookAway = JSController.KTLookAway / 60;

            //If not need help actualy (The help was not be asked)
            if (!needhelp)
            {
                //User is tracked
                if (JSController.KTHappyness != 0 && JSController.KTLookAway != 0)
                {

                    //If user if not happy
                    if (JSController.KTHappyness < 20)
                    {
                        if (JSController.JSflagRUserious >= 2)
                        {
                            needhelp = true;
                            inactivity = 0;
                        }
                        else if (JSController.JSflaginactivity)
                        {
                            //5 Minute
                            if (++inactivity > 10) needhelp = true;
                        }

                    }

                    //Look of help but still happy
                    if (!needhelp)
                    {
                        //LockAway 
                        if (JSController.KTLookAway > 30)
                        {
                            needhelp = true;
                        }
                    }

                    //Reset inactivity if clickSSSS are found
                    if (!JSController.JSflaginactivity)
                    {
                        inactivity = 0;
                    }
                }
            }

            JSController.clearVariable();
        }
    }
}
