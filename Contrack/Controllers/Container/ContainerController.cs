using Contrack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
namespace Contrack.Controllers
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    [AuthorizeRoles(LoginType.Hub, Order = 4)]
    public class ContainerController : Controller
    {
        private readonly ContainerService _service;
        protected readonly TrackingService _trackingService;
        public ContainerController()
        {
            IContainerRepository repo = new ContainerRepository();
            ITrackingRepository trackrepo = new TrackingRepository();
            _service = new ContainerService(repo);
            _trackingService = new TrackingService(trackrepo);
        }
        private void containerDropDowns()
        {
            ViewBag.ContainerTypes = Dropdowns.GetContainerTypesDropdown();
            ViewBag.ContainerSizes = Dropdowns.GetContainerSizesDropdown();
            ViewBag.Locations = Dropdowns.GetLocationDropdown(true);
            ViewBag.Ports = Dropdowns.GetPortDropdown("", true);
            ViewBag.ContainerModels = Dropdowns.GetContainerModelsDropdown(true);
            ViewBag.StatusDropdown = Dropdowns.GetContainerStatusDropdown();
        }
        public ActionResult List()
        {
            ContainerFilterPage filter = PageSessionManager.GetFilter<ContainerFilterPage>(PageKeys.Container);
            if (filter.filters == null) filter.filters = new ContainerFilter();
            _service.PopulateStatusCounts(filter);
            SetFilterSession(filter);
            containerDropDowns();
            return View(filter);
        }
        [HttpPost]
        public ActionResult List(ContainerFilterPage filter, string action)
        {
            int ActiveTab = 0;
            if (action == "Reset")
            {
                ModelState.Clear();
                filter = new ContainerFilterPage();
                SessionManager.ContainerListFilter = null;
                ActiveTab = 0;
            }
            else
            {
                SessionManager.ContainerListFilter = filter;
                ActiveTab = 0;
            }
            PageSessionManager.SetAttribute(PageKeys.Container, "ActiveTab", ActiveTab);
            SetFilterSession(filter);
            _service.PopulateStatusCounts(filter);
            containerDropDowns();
            return View(filter);
        }
        public ActionResult ListStatus(int status)
        {
            PageSessionManager.SetAttribute(PageKeys.Container, "ActiveTab", status);
            ContainerFilterPage filter = PageSessionManager.GetFilter<ContainerFilterPage>(PageKeys.Container);
            if (filter.filters == null) filter.filters = new ContainerFilter();
            filter.filters.status = status;
            SetFilterSession(filter);
            return RedirectToAction("List");
        }
        private void SetFilterSession(ContainerFilterPage filter)
        {
            SessionManager.ContainerListFilter = filter;
            PageSessionManager.SetFilter(PageKeys.Container, filter);
        }
        public ActionResult GetContainerModal(string containerid = "")
        {
            ContainerModal model = new ContainerModal();
            try
            {
                ViewBag.ContainerModels = Dropdowns.GetContainerModelsDropdown();
                ViewBag.Operators = Dropdowns.GetContainerOperatorDropdown();
                ViewBag.Locations = Dropdowns.GetLocationDropdown();
                ViewBag.Months = Dropdowns.GetMonthDropdown();
                ViewBag.Years = Dropdowns.GetYearDropdown();

                ViewBag.EmptyFullStatus = new List<SelectListItem>
                {
                    new SelectListItem { Text = "-Select-", Value = "" },
                    new SelectListItem { Text = "Empty", Value = "true" },
                    new SelectListItem { Text = "Full", Value = "false" }
                };

                if (!string.IsNullOrEmpty(containerid))
                {
                    model = _service.GetContainerByID(containerid);
                }
            }
            catch (Exception ex)
            {
            }
            return PartialView("~/Views/Container/_ModalContainer.cshtml", model);
        }
        [HttpPost]
        public ActionResult SaveContainer(ContainerModal model)
        {
            _service.SaveContainer(model);
            return Json(_service.result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string refid)
        {
            ContainerModal model = new ContainerModal();
            model = _service.GetContainerByUUID(refid);
            TrackingFilterPage filter = new TrackingFilterPage();
            model.trackinglist = _trackingService.GetTrackingList(filter);
            return View(model);
        }
    }
}