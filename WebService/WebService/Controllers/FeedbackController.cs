using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebService.Controllers
{
    public class FeedbackController : ApiController
    {
        [HttpGet]
        public bool GetEmpDetails()
        {
            if (WebApiConfig.needhelp)
            {
                WebApiConfig.needhelp = false;
                return true;
            }
            else
            {
                return WebApiConfig.needhelp;
            }

        }
    }
}
