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
    public class ContainerTypeController : Controller
    {
        private readonly ContainerTypeService _service;
        public ContainerTypeController()
        {
            IContainerTypeRepository repo = new ContainerTypeRepository();
            _service = new ContainerTypeService(repo);
        }
        public ActionResult List()
        {
            return View();
        }

        public ActionResult GetByUUID(string uuid)
        {
            var data = _service.GetContainerTypeByUUID(uuid);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetContainerTypeModal(string ContainerTypeId = "")
        {
            ContainerTypeDTO model = new ContainerTypeDTO();
            try
            {
                model = _service.GetContainerTypeByID(ContainerTypeId);
                //ViewBag.IconDropdown = Dropdowns.GetContainerIcons();  // icon list
            }
            catch (Exception)
            {
            }
            return PartialView("~/Views/ContainerModel/_ModalContainerType.cshtml", model);
        }

        [HttpPost]
        public ActionResult SaveContainerType(ContainerTypeDTO type)
        {
            _service.SaveContainerType(type);
            return Json(_service.Result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteContainerType(string containertypeid)
        {
            _service.DeleteContainerType(containertypeid);
            return Json(_service.Result, JsonRequestBehavior.AllowGet);
        }
    }
}