using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Contrack.Controllers
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    public class ReserveController : BookingBaseController
    {
        //private readonly BookingService _service;
        //public ReserveController()
        //{
        //    IBookingRepository repo = new BookingRepository();
        //    _service = new BookingService(repo);
        //}
        public ActionResult AllocationDecider(string refid)
        {
            ContainerBooking booking = _service.GetbookingByUUID(refid);
            string bookingdetailuuid = booking.booking.details.FirstOrDefault()?.bookingdetailuuid ?? "";
            return RedirectToAction("ContainerAllocation", "Reserve", new { refid = refid, bookingdetailuuid = bookingdetailuuid });
        }

        public ActionResult ContainerAllocation(string refid, string bookingdetailuuid)
        {
            ContainerAllocation model = _service.GetContainerAllocations(bookingdetailuuid, refid);
            return View(model);
        }
        [HttpPost]
        public ActionResult AllocateQty(string BookingID, string BookingDetailID, ContainerAllocate allocation)
        {
            _service.SaveContainerAllocation(BookingID, BookingDetailID, allocation);
            return Json(_service.result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteAllocateQty(string BookingID, string BookingDetailID, string LocationID, string ModelID)
        {
            _service.DeleteContainerAllocation(BookingID, BookingDetailID, LocationID, ModelID);
            return Json(_service.result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContainerSelection(string refid)
        {
            ContainerSelection model = _service.GetContainerSelection(refid);
            return View(model);
        }

        [HttpPost]
        public ActionResult ContainerSelection(ContainerSelection bookingmodel, string saveaction = "")
        {
            Result result = Common.ErrorMessage("Error in saving container selection");
            _service.SaveContainerSelection(bookingmodel.Booking.bookingid.EncryptedValue, bookingmodel.Selections);
            if (saveaction == "Confirm")
            {
                _service.ConfirmContainerSelection(bookingmodel.Booking.bookingid.EncryptedValue);
                if (_service.result.ResultId == 1)
                {
                    _service.result.ResultMessage = Url.Action("BookedContainers", "Reserve", new { refid = bookingmodel.Booking.bookinguuid });
                }
            }
            else if (_service.result.ResultId == 1)
            {
                _service.result.ResultMessage = Url.Action("ContainerSelection", "Reserve", new { refid = bookingmodel.Booking.bookinguuid });
            }
            return Json(_service.result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AcquireContainer(string bookinguuid, string modeluuid, string locationuuid, List<AcquireDTO> containerids)
        {
            List<BookingAllocationAcquireDTO> list = _service.AcquireContainer(bookinguuid, containerids, locationuuid, modeluuid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BookedContainers(string refid)
        {
            BookedContainer book = _service.GetBookedContainers(refid, new BookedContainerFilter() { });
            return View(book);
        }
        [HttpPost]
        public ActionResult DeleteContainer(string bookinguuid, string containeruuid)
        {
            _service.DeleteContainerFromBooking(bookinguuid, containeruuid);
            return Json(_service.result, JsonRequestBehavior.AllowGet);
        }
    }
}