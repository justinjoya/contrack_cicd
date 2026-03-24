using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contrack.Controllers.Dashboard
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }
    }
}