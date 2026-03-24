using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Contrack.Controllers
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    [AuthorizeRoles(LoginType.Hub, Order = 4)]
    public class UoMController : Controller
    {
        private readonly UoMService _uomService;

        public UoMController()
        {
            IUoMRepository repo = new UoMRepository();
            _uomService = new UoMService(repo);
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult GetUoMModal()
        {
            return PartialView("~/Views/Shared/Masters/_ModalUoM.cshtml");
        }

        [HttpPost]
        public JsonResult SaveUoM(UoMDTO model)
        {
            _uomService.SaveUoM(model);

            if (_uomService.result.ResultId == 1)
                _uomService.result.ResultMessage = Url.Action("List", "UoM");

            return Json(_uomService.result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteUoM(string encryptedId)
        {
            int decryptedId = Common.Decrypt(encryptedId);
            _uomService.DeleteUoM(decryptedId);
            return Json(_uomService.result, JsonRequestBehavior.AllowGet);
        }

    }
}
