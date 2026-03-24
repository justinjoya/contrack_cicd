using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Contrack.Controllers
{
    public class TermsandConditionsController : Controller
    {
        private readonly TermsandConditionsService _termsandconditionsService;
        public TermsandConditionsController()
        {
            ITermsandConditionsRepository repo = new TermsandConditionsRepository();
            _termsandconditionsService = new TermsandConditionsService(repo);
        }
        public ActionResult List()
        {
            return View();
        }

        public ActionResult CreateTermsandConditions(string refid = "")
        {
            var termsandconditions = _termsandconditionsService.GetTermAndConditionsByUUID(refid);
            var model = new TermsandConditions()
            {
                termsandConditions = termsandconditions
            };
            ViewBag.AgencyDropdown = Dropdowns.GetAgenciesDropdown(false);
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveTermsandConditions(TermsandConditions model)
        {
            Result result = Common.ErrorMessage("Cannot save location details");
            try
            {
                _termsandconditionsService.SaveTermsandConditions(model.termsandConditions);
                result = _termsandconditionsService.result;
                if (result.ResultId == 1)
                {
                    result.ResultMessage = Url.Action("List", "TermsandConditions");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteTermsandConditions(string TermUuid)
        {
            Result Result = new Result();
            _termsandconditionsService.DeleteTermsandConditions(TermUuid);
            return Json(Result = _termsandconditionsService.result, JsonRequestBehavior.AllowGet);
        }

    }
}