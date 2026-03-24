using Contrack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
namespace Contrack.Controllers
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    [AuthorizeRoles(LoginType.Hub, Order = 4)]
    public class TrackingController : Controller
    {
        private readonly TrackingService _trackingService;
        private readonly ContainerService _containerService;
        private readonly BookingService _bookingService;

        public TrackingController()
        {
            ITrackingRepository repo = new TrackingRepository();
            _trackingService = new TrackingService(repo);
            _containerService = new ContainerService(new ContainerRepository());
            IBookingRepository bookrepo = new BookingRepository();
            _bookingService = new BookingService(bookrepo, new TermsandConditionsRepository(), new VoyageRepository());
        }
        private void trackingDropDowns()
        {
            ViewBag.MovesDropdown = Dropdowns.GetMovesDropdown(true);
            ViewBag.DamageDropdown = Dropdowns.GetDamageStatusDropdown();
            ViewBag.FullEmptyDropdown = Dropdowns.GetFullEmptyDropdown();
            ViewBag.LocationDropdown = Dropdowns.GetLocationDropdown();
            ViewBag.NewMovesDropdown = Dropdowns.GetNewMovesDropdown();
            ViewBag.VoyageDropdown = Dropdowns.EmptyDropdown();
        }
        public ActionResult List(string refid)
        {
            TrackingFilterPage filter = SessionManager.TrackingListFilter;
            if (filter == null)
                filter = new TrackingFilterPage();
            filter.ContainerUUID = refid;
            if (!string.IsNullOrEmpty(refid))
            {
                var containerModal = _containerService.GetContainerByUUID(refid);
                filter.ContainerInfo = containerModal.container;
            }
            SessionManager.TrackingListFilter = filter;
            trackingDropDowns();
            return View(filter);
        }
        [HttpPost]
        public ActionResult List(string id, TrackingFilterPage filter, string action)
        {
            if (action == "Download")
            {
                filter.ContainerUUID = id;
                string fileName = "Tracking_" + Guid.NewGuid() + ".xls";
                byte[] bytesFile = null;
                string errorMessage = "";
                try
                {
                    string outputpath = TrackingDownload.TrackingListDownload(filter, fileName);
                    if (!string.IsNullOrEmpty(outputpath) && System.IO.File.Exists(outputpath))
                        bytesFile = System.IO.File.ReadAllBytes(outputpath);
                    else
                        errorMessage = "File does not exist on server.";
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
                if (bytesFile == null || bytesFile.Length == 0)
                    return Content("Error: The download file could not be generated. Details: " + errorMessage);

                Response.AddHeader("Content-Disposition", "attachment; filename=TrackingHistory.xls");
                return File(bytesFile, "application/ms-excel");
            }
            if (action == "Reset")
            {
                SessionManager.TrackingListFilter = null;
                return RedirectToAction("List", new { refid = id });
            }
            filter.ContainerUUID = id;
            SessionManager.TrackingListFilter = filter;
            return RedirectToAction("List", new { refid = id });
        }
        public ActionResult RecordMove()
        {
            Tracking tracking = new Tracking();
            tracking.Moves = _trackingService.GetMovesList();
            trackingDropDowns();
            return View(tracking);
        }

        [HttpPost]
        public ActionResult GetRecordMoveModal(Tracking tracking)
        {
            _trackingService.SavePickSelection(tracking.ContainerBookingSelection);

            if (_trackingService.result.ResultId != 1)
            {
                return Json(_trackingService.result, JsonRequestBehavior.AllowGet);
            }
            tracking.AvailList = _containerService.IsContainerAvailable(tracking.ContainerBookingSelection.Select(x => x.ContainerId.EncryptedValue).ToList());

            tracking.Trackingmodel.PickSelectionUuid = _trackingService.result.TargetUUID;
            tracking.Moves = _trackingService.GetMovesList();
            trackingDropDowns();
            return PartialView("~/Views/Shared/Tracking/_ModalMoveRecord.cshtml", tracking);
        }

        [HttpPost]
        public ActionResult SaveRecordMove(Tracking model)
        {
            _trackingService.SaveTracking(model);
            return Json(_trackingService.result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult TrackContainer(string containeruuid, string bookinguuid)
        {
            var booking = _bookingService.GetbookingByUUID(bookinguuid);
            var trackingDetails = _trackingService.GetTrackingDetails(containeruuid, bookinguuid, booking);
            trackingDetails.booking = booking;
            return View(trackingDetails);
        }

        public ActionResult GetRecordMoveModalByUuid(string trackingUuid)
        {
            Tracking tracking = new Tracking();
            tracking = _trackingService.GetTrackingByUUID(trackingUuid);
            tracking.Moves = _trackingService.GetMovesList();
            trackingDropDowns();
            return PartialView("~/Views/Shared/Tracking/_ModalMoveRecord.cshtml", tracking);
        }
    }
}