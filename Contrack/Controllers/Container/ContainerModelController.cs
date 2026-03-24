using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Contrack.Controllers
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    [AuthorizeRoles(LoginType.Hub, Order = 4)]
    public class ContainerModelController : Controller
    {
        private readonly ContainerModelService _service;
        public ContainerModelController()
        {
            IContainerModelRepository repo = new ContainerModelRepository();
            _service = new ContainerModelService(repo);
        }

        public ActionResult List()
        {
            ContainerModelFilter filter = SessionManager.ContainerModelFilter;
            if (filter == null)
                filter = new ContainerModelFilter();
            SessionManager.ContainerModelFilter = filter;
            return View(filter);
        }

        //[HttpPost]
        //public ActionResult List(ContainerModelFilter filter, string action)
        //{
        //    if (action == "Reset")
        //    {
        //        SessionManager.ContainerModelFilter = null;
        //        return RedirectToAction("List");
        //    }
        //    SessionManager.ContainerModelFilter = filter;
        //    return View(filter);
        //}

        public ActionResult GetContainerModelById(string modelid)
        {
            ContainerModelDTO model = new ContainerModelDTO();
            try
            {
                model = _service.GetContainerModelByID(Common.Decrypt(modelid));
            }
            catch (Exception)
            { }
            return PartialView("~/Views/ContainerModel/_ModalContainerModel.cshtml", model);
        }

        [HttpPost]
        public ActionResult SaveContainerModel(ContainerModel model)
        {
            _service.SaveContainerModel(model.containermodel);
            return Json(_service.Result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteContainerModel(string containermodelid)
        {
            _service.DeleteContainerModel(Common.Decrypt(containermodelid));
            return Json(_service.Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetContainerModelModal(string modelid = "", string typeid = "", string sizeid = "")
        {
            ContainerModel model = new ContainerModel();
            try
            {
                if (!string.IsNullOrEmpty(modelid))
                {
                    var dto = _service.GetContainerModelByID(Common.Decrypt(modelid));
                    if (dto != null)
                        model.containermodel = dto;
                }
                else
                {
                    if (!string.IsNullOrEmpty(typeid))
                        model.containermodel.typeid.EncryptedValue = typeid;
                    if (!string.IsNullOrEmpty(sizeid))
                        model.containermodel.sizeid.EncryptedValue = sizeid;
                }
                ViewBag.ContainerTypesDropdown = Dropdowns.GetContainerTypesDropdown();
                ViewBag.ContainerSizesDropdown = Dropdowns.GetContainerSizesDropdown();
            }
            catch (Exception)
            { }
            return PartialView("~/Views/ContainerModel/_ModalContainerModel.cshtml", model);
        }

        [HttpGet]
        public ActionResult GetModels(string q)
        {
            var data = Dropdowns.GetContainerModelSearch(q);
            return Json(new { results = data }, JsonRequestBehavior.AllowGet);
        }
    }
}