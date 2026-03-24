using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Contrack.Controllers
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    [AuthorizeRoles(LoginType.Hub, Order = 4)]
    public class PurchaseOrderController : Controller
    {
        private readonly PurchaseOrderService _service;

        public PurchaseOrderController()
        {
            IPurchaseOrderRepository repo = new PurchaseOrderRepository();
            _service = new PurchaseOrderService(repo);
        }
        private void SetListDropdowns(PurchaseOrderFilterPage filter)
        {
            ViewBag.CreatedByDropdown = Dropdowns.GetLoginUsersByRole(Common.Encrypt(0), "", false, false);
            ViewBag.StatusDropdown = Dropdowns.GetStatusDropdown(4, false);
            ViewBag.AgenciesFilterDropdown = Dropdowns.GetAgenciesUUIDDropdown();
            var selectedAgencies = filter.filters.agencyuuids;
            ViewBag.VendorFilterDropdown = _service.GetVendorFilterDropdown(selectedAgencies);
        }
        private void SetCreateDropdowns(PurchaseOrderModel model)
        {
            var Agencies = Dropdowns.GetAgenciesDetailIDDropdown();
            ViewBag.AgenciesDropdown = Common.AlterDropdown(Agencies, model.ContainerPO.PO.agencydetailid.EncryptedValue, model.ContainerPO.PO.agencyname);
            string agencyId = model.ContainerPO.PO.agencydetailid.EncryptedValue;
            ViewBag.CurrencyDropdown = Dropdowns.GetCurrencyDropdown();
            ViewBag.UOMDropdown = Dropdowns.GetUOMDropdown();
            ViewBag.VendorDropdown = Dropdowns.GetVendorDropdown(agencyId, "", false);
        }
        public ActionResult List()
        {
            PurchaseOrderFilterPage filter = PageSessionManager.GetFilter<PurchaseOrderFilterPage>(PageKeys.PurchaseOrder);
            if (filter.filters == null) filter.filters = new PurchaseOrderFilter();
            _service.PopulateStatusCounts(filter);
            SetFilterSession(filter);
            SetListDropdowns(filter);
            return View(filter);
        }
        [HttpPost]
        public ActionResult List(PurchaseOrderFilterPage filter, string action)
        {
            int ActiveTab = 0;
            if (action == "Reset")
            {
                ModelState.Clear();
                filter = new PurchaseOrderFilterPage();
                SessionManager.PurchaseOrderFilter = null;
                ActiveTab = 0;
            }
            else
            {
                SessionManager.PurchaseOrderFilter = filter;
                ActiveTab = 0;
            }
            if (filter.filters == null) filter.filters = new PurchaseOrderFilter();
            filter.filters.status = ActiveTab;
            PageSessionManager.SetAttribute(PageKeys.PurchaseOrder, "ActiveTab", ActiveTab);
            SetFilterSession(filter);
            _service.PopulateStatusCounts(filter);
            SetListDropdowns(filter);
            return View(filter);
        }
        public ActionResult ListStatus(int status)
        {
            PageSessionManager.SetAttribute(PageKeys.PurchaseOrder, "ActiveTab", status);
            PurchaseOrderFilterPage filter = PageSessionManager.GetFilter<PurchaseOrderFilterPage>(PageKeys.PurchaseOrder);
            if (filter.filters == null) filter.filters = new PurchaseOrderFilter();

            filter.filters.status = status;
            if (filter.filters.status_list != null)
            {
                filter.filters.status_list.Clear();
            }
            SetFilterSession(filter);
            return RedirectToAction("List");
        }
        private void SetFilterSession(PurchaseOrderFilterPage filter)
        {
            SessionManager.PurchaseOrderFilter = filter;
            PageSessionManager.SetFilter(PageKeys.PurchaseOrder, filter);
        }
        public ActionResult Create(string refid = "")
        {
            PurchaseOrderModel model = _service.GetPurchaseOrderByUUID(refid);
            SetCreateDropdowns(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(PurchaseOrderModel model, string saveaction)
        {
            _service.SaveDirectPurchaseOrder(model);
            if (_service.result.ResultId == 1)
            {
                _service.result.ResultMessage = Url.Action("Create", "PurchaseOrder", new { refid = _service.result.TargetUUID });
            }
            return Json(_service.result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetVendorFilterDropdown(List<string> agencyuuids)
        {
            try
            {
                var vendors = _service.GetVendorFilterDropdown(agencyuuids);
                return Json(vendors, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult GetPOLineItemModal(string podetailuuid = "")
        {
            PurchaseOrderDetailDTO model = _service.GetPOLineItem(podetailuuid);
            ViewBag.UOMDropdown = Dropdowns.GetUOMDropdown();
            ViewBag.JobTypeDetailDropdown = Dropdowns.GetJobTypeDetailDropdown(model.jobtype.ToString());
            return PartialView("_ModalPOLineItem", model);
        }
        [HttpPost]
        public ActionResult SavePOLineItem(PurchaseOrderDetailDTO lineItem)
        {
            ModelState.Clear();
            PurchaseOrderModel updatedModel = _service.SavePOLineItem(lineItem);
            ViewBag.UOMDropdown = Dropdowns.GetUOMDropdown();
            return PartialView("_PODetail", updatedModel);
        }
        [HttpPost]
        public ActionResult DeletePOLineItem(string podetailuuid)
        {
            ModelState.Clear();
            PurchaseOrderModel updatedModel = _service.DeletePOLineItem(podetailuuid);
            ViewBag.UOMDropdown = Dropdowns.GetUOMDropdown();
            return PartialView("_PODetail", updatedModel);
        }
    }
}
