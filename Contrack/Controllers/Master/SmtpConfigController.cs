using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contrack.Controllers
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    [AuthorizeRoles(LoginType.Hub, Order = 4)]
    public class SmtpConfigController : Controller
    {
        private readonly SmtpConfigService _smtpConfigService;
        public SmtpConfigController()
        {
            ISmtpConfigRepository repo = new SmtpConfigRepository();
            _smtpConfigService = new SmtpConfigService(repo);
        }
        public ActionResult List()
        {
            return View();
        }

        public ActionResult GetSmtpConfigModal(string refid)
        {
            SmtpConfig smtpconfig = new SmtpConfig()
            {
                smtpconfig = _smtpConfigService.GetSmtpConfigById(refid)
            };
            ViewBag.AgencyDropdown = Dropdowns.GetAgenciesDropdown(false);
            return PartialView("~/Views/Shared/Masters/_ModalAddSmtpConfig.cshtml", smtpconfig);
        }

        [HttpPost]
        public ActionResult SaveSmtpConfig(SmtpConfig model)
        {
            _smtpConfigService.SaveSmtpConfig(model);
            return Json(_smtpConfigService.result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteSmtpConfig(SmtpConfig model)
        {
            _smtpConfigService.DeleteSmtpConfig(model);
            return Json(_smtpConfigService.result, JsonRequestBehavior.AllowGet);
        }
    }
}