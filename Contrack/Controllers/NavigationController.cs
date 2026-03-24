using Contrack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contrack.Controllers
{
    public class NavigationController : Controller
    {
        public ActionResult GoBack()
        {
            var url = NavigationStackManager.PopUrl();
            return Redirect(url);
        }
    }
}