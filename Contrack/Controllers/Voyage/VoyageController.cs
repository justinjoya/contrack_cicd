using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Contrack.Controllers
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    public class VoyageController : Controller
    {
        private readonly VoyageService _voyageService;
        public VoyageController()
        {
            IVoyageRepository repo = new VoyageRepository();
            _voyageService = new VoyageService(repo);
        }
        private void voyageDropDowns(VoyageFilter filter)
        {
            ViewBag.PortDropdown = Dropdowns.GetPortDropdown("", true);
            ViewBag.StatusDropdown = Dropdowns.GetVoyageStatusDropdown();
            if (!string.IsNullOrEmpty(filter.filters.vesselassignmentencrypted))
            {
                var detailId = Common.Decrypt(filter.filters.vesselassignmentencrypted);
                var vessels = Dropdowns.GetVesselByDetailIDs(detailId.ToString());
                if (vessels.Count > 0)
                {
                    ViewBag.VesselDropdown = vessels;
                }
                else
                {
                    ViewBag.VesselDropdown = Dropdowns.EmptyDropdown();
                }
            }
            else
            {
                ViewBag.VesselDropdown = Dropdowns.EmptyDropdown();
            }
        }
        public ActionResult List()
        {
            VoyageFilter filter = PageSessionManager.GetFilter<VoyageFilter>(PageKeys.Voyage);
            if (filter.filters == null) filter.filters = new InnerFilters();
            _voyageService.PopulateStatusCounts(filter);
            SetFilterSession(filter);
            VoyageData(filter);
            return View(filter);
        }
        [HttpPost]
        public ActionResult List(VoyageFilter filter, string action)
        {
            int ActiveTab = PageSessionManager.GetAttribute<int>(PageKeys.Voyage, "ActiveTab", 0);
            var existingFilter = PageSessionManager.GetFilter<VoyageFilter>(PageKeys.Voyage);
            if (existingFilter == null)
                existingFilter = new VoyageFilter();
            if (existingFilter.filters == null)
                existingFilter.filters = new InnerFilters();
            if (action == "Reset")
            {
                ModelState.Clear();
                filter = new VoyageFilter();
                SessionManager.VoyageListFilter = null;
                ActiveTab = 0;
            }
            else
            {
                var mergedFilter = existingFilter;
                TryUpdateModel(mergedFilter);
                filter = mergedFilter;
                if (existingFilter.filters.status != filter.filters.status)
                {
                    ActiveTab = 0;
                }
                SessionManager.VoyageListFilter = filter;
            }
            PageSessionManager.SetAttribute(PageKeys.Voyage, "ActiveTab", ActiveTab);
            SetFilterSession(filter);
            _voyageService.PopulateStatusCounts(filter);
            VoyageData(filter);
            return View(filter);
        }
        public ActionResult ListStatus(int status)
        {
            PageSessionManager.SetAttribute(PageKeys.Voyage, "ActiveTab", status);
            VoyageFilter filter = PageSessionManager.GetFilter<VoyageFilter>(PageKeys.Voyage);
            if (filter.filters == null) filter.filters = new InnerFilters();
            filter.filters.status = status;
            SetFilterSession(filter);
            return RedirectToAction("List");
        }
        private void SetFilterSession(VoyageFilter filter)
        {
            SessionManager.VoyageListFilter = filter;
            PageSessionManager.SetFilter(PageKeys.Voyage, filter);
        }
        private void VoyageData(VoyageFilter filter)
        {
            SessionManager.VoyageListFilter = filter;
            voyageDropDowns(filter);
        }
        public ActionResult Create(string voyageDetailId = "", string voyageId = "", string mode = "")
        {
            VoyageExtention model = new VoyageExtention();

            if (!string.IsNullOrEmpty(voyageDetailId))
            {
                model = _voyageService.GetVoyageByDetailID(Common.Decrypt(voyageDetailId));
            }
            else if (!string.IsNullOrEmpty(voyageId))
            {
                model = _voyageService.GetVoyageById(Common.Decrypt(voyageId));
            }

            ViewBag.VoyageDropdown = Dropdowns.EmptyDropdown();
            ViewBag.VesselDropdown = Dropdowns.EmptyDropdown();
            ViewBag.PortDropdown = Dropdowns.EmptyDropdown();
            ViewBag.FormMode = mode;

            return PartialView("_ModalVoyage", model);
        }

        [HttpPost]
        public ActionResult SaveVoyage(VoyageExtention model)
        {
            _voyageService.SaveVoyage(model);
            return Json(_voyageService.result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddIntermediatePort(VoyageExtention model)
        {
            _voyageService.AddIntermediatePort(model);
            return Json(_voyageService.result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult MarkAsArrived(string voyageId, string voyageDetailId)
        {
            VoyageExtention model = new VoyageExtention();
            if (!string.IsNullOrEmpty(voyageDetailId))
            {
                model = _voyageService.GetVoyageByDetailID(Common.Decrypt(voyageDetailId));
            }

            model.VoyageExtendedDTO.VoyageId.EncryptedValue = voyageId;
            model.VoyageExtendedDTO.VoyageDetails.VoyageDetailId.EncryptedValue = voyageDetailId;

            return PartialView("_ModalArrived", model);
        }

        [HttpGet]
        public ActionResult MarkAsDepartured(string voyageId, string voyageDetailId)
        {
            VoyageExtention model = new VoyageExtention();
            if (!string.IsNullOrEmpty(voyageDetailId))
            {
                model = _voyageService.GetVoyageByDetailID(Common.Decrypt(voyageDetailId));
            }
            model.VoyageExtendedDTO.VoyageId.EncryptedValue = voyageId;
            model.VoyageExtendedDTO.VoyageDetails.VoyageDetailId.EncryptedValue = voyageDetailId;

            return PartialView("_ModalDepartured", model);
        }
        [HttpPost]
        public ActionResult MarkAsArrived(VoyageExtention model)
        {
            _voyageService.MarkAsArrived(model);
            return Json(_voyageService.result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult MarkAsDepartured(VoyageExtention model)
        {
            _voyageService.MarkAsDepartured(model);
            return Json(_voyageService.result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetVoyageSearch(string q, int addnew = 1)
        {
            var data = Dropdowns.GetVoyageSearch(q, addnew == 1);

            var list = data.Select(g => new
            {
                id = g.VoyageId?.EncryptedValue,
                number = g.VoyageId.NumericValue,
                text = g.VoyageNumber,
                displaytext = g.ActualVoyageNumber,
                vesselId = g.VesseDetailId?.EncryptedValue,
                vesselName = g.Vesselname,
                comments = g.Description
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DisableVoyage(VoyageDetailDTO voyage)
        {
            _voyageService.DeleteVoyageDetail(
                Common.Decrypt(voyage.VoyageDetailId.EncryptedValue),
                Common.Decrypt(voyage.VoyageId.EncryptedValue)
            );

            return Json(_voyageService.result, JsonRequestBehavior.AllowGet);
        }
    }
}