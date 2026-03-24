using Contrack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Contrack.Controllers.CRM
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    public class BookingController : BookingBaseController
    {
        //private readonly BookingService _service;
        //private readonly ClientService _clientService;
        //private readonly ContainerModelService _containermodelservice;
        //private readonly VoyageService _voyageService;
        //private readonly AgencyService _agencyService;
        //private readonly TermsandConditionsService _termsandconditions;
        //private readonly ChatService _chatService;
        //private readonly PricingService _pricingService;

        //public BookingController()
        //{
        //    ITermsandConditionsRepository tcrepo = new TermsandConditionsRepository();
        //    _termsandconditions = new TermsandConditionsService(tcrepo);
        //    IVoyageRepository VoyageRepository = new VoyageRepository();
        //    _voyageService = new VoyageService(VoyageRepository);
        //    IPricingRepository pricingRepository = new PricingRepository();
        //    _pricingService = new PricingService(pricingRepository);
        //    IClientRepository client = new ClientRepository();
        //    _clientService = new ClientService(client);

        //    IBookingRepository repo = new BookingRepository();
        //    _service = new BookingService(repo, tcrepo, VoyageRepository, pricingRepository, client);


        //    IContainerModelRepository ContainerModel = new ContainerModelRepository();
        //    _containermodelservice = new ContainerModelService(ContainerModel);

        //    IAgencyRepository agencyRepository = new AgencyRepository();
        //    _agencyService = new AgencyService(agencyRepository);
        //    IChatRepository chatRepository = new ChatRepository();
        //    _chatService = new ChatService(chatRepository);
        //}
        private void ContainerBookingDropDowns(BookingList bookinglist)
        {
            ViewBag.PortDropdown = Dropdowns.GetPortDropdown();
            ViewBag.AgenciesUUIDDropdown = Dropdowns.GetAgenciesUUIDDropdown();
            ViewBag.ClientUUIDDropdown = Dropdowns.GetClientsByUserIDDropdown();
            ViewBag.CreatedByDropdown = Dropdowns.GetLoginUsersByRole(Common.Encrypt(0), "", false, false);
            ViewBag.StatusDropdown = Dropdowns.GetStatusDropdown(105, false);
            ViewBag.VesselDropdown = Dropdowns.EmptyDropdown(false);
        }
        public ActionResult List()
        {
            BookingListFilter filter = PageSessionManager.GetFilter<BookingListFilter>(PageKeys.Booking);
            if (filter.filters == null) filter.filters = new BookingFilters();
            _service.PopulateBookingStatusCounts(filter);
            SetFilterSession(filter);
            ContainerbookingData(filter);
            return View(filter);
        }
        [HttpPost]
        public ActionResult List(BookingListFilter filter, string action)
        {
            int ActiveTab = 0;
            if (action == "Reset")
            {
                ModelState.Clear();
                filter = new BookingListFilter();
                SessionManager.BookingListFilter = null;
                ActiveTab = 0;
            }
            else
            {
                SessionManager.BookingListFilter = filter;
                ActiveTab = 0;
            }
            PageSessionManager.SetAttribute(PageKeys.Booking, "ActiveTab", ActiveTab);
            SetFilterSession(filter);
            _service.PopulateBookingStatusCounts(filter);
            ContainerbookingData(filter);
            return View(filter);
        }

        public ActionResult BookingListStatus(int status)
        {
            PageSessionManager.SetAttribute(PageKeys.Booking, "ActiveTab", status);
            BookingListFilter filter = PageSessionManager.GetFilter<BookingListFilter>(PageKeys.Booking);
            if (filter.filters == null)
                filter.filters = new BookingFilters();
            if (status == 0)
            {
                filter.filters.status = new List<int>();
            }
            else
            {
                filter.filters.status = new List<int> { status };
            }
            SetFilterSession(filter);
            return RedirectToAction("List");
        }

        private void ContainerbookingData(BookingListFilter filter)
        {
            SessionManager.BookingListFilter = filter;
            ContainerBookingDropDowns(new BookingList());
        }
        private void SetFilterSession(BookingListFilter filter)
        {
            SessionManager.BookingListFilter = filter;
            PageSessionManager.SetFilter(PageKeys.Booking, filter);
        }
        public ActionResult BookingDecider(string refid = "")
        {
            List<BookingStepCount> list = BookingStepCount.GetBookingCount(refid);

            int CustomerStepCount = list.Where(x => x.document_type == "Step1").Sum(x => x.count);
            int LocationStepCount = list.Where(x => x.document_type == "Step2").Sum(x => x.count);
            int ContainerStepCount = list.Where(x => x.document_type == "Step3").Sum(x => x.count);
            if (ContainerStepCount > 0)
                return RedirectToAction("FinalSummary", "Booking", new { refid = refid });
            else if (LocationStepCount > 0)
                return RedirectToAction("ContainerSelection", "Booking", new { refid = refid });
            else if (CustomerStepCount > 0)
                return RedirectToAction("LocationSelection", "Booking", new { refid = refid });
            else
                return RedirectToAction("CustomerSelection", "Booking", new { refid = refid });
        }
        public ActionResult CustomerSelection(string refid = "", bool SaveOnly = false)
        {
            ViewBag.SaveOnly = SaveOnly;
            ContainerBooking model = new ContainerBooking();
            model = _service.GetbookingByUUID(refid);
            ViewBag.ClientDropdown = Dropdowns.EmptyDropdown();
            ViewBag.PortDropdown = Dropdowns.EmptyDropdown();
            ViewBag.FullOREmptyDropdown = Dropdowns.GetFullOREmptyDropdown(true);
            var agencies = Dropdowns.GetAgenciesDetailIDDropdown();
            ViewBag.AgenciesDropdown = Common.AlterDropdown(agencies, model.booking.customer.agencydetailid.EncryptedValue, model.booking.customer.agencyname);
            ViewBag.ModeDropdown = Dropdowns.GetTransferTypeDropdown(true);
            if (model.booking.location.shipperpic.NumericValue > 0 && model.booking.location.shipperdetailid.NumericValue > 0)
            {
                ViewBag.ShipperPICDropdown = Dropdowns.GetPICByDetailIDDropdown(model.booking.location.shipperdetailid.EncryptedValue);
            }
            if (model.booking.location.consigneepic.NumericValue > 0 && model.booking.location.consigneedetailid.NumericValue > 0)
            {
                ViewBag.ConsigneePICDropdown = Dropdowns.GetPICByDetailIDDropdown(model.booking.location.consigneedetailid.EncryptedValue);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult CustomerSelection(ContainerBooking model, bool SaveOnly = false)
        {
            Result result = Common.ErrorMessage("Cannot save booking details");
            try
            {
                if (model == null || model.booking == null || model.booking.customer == null)
                {
                    result = Common.ErrorMessage("Invalid data.");
                }
                else
                {
                    _service.SaveCustomer(Common.Decrypt(model.booking.bookingid.EncryptedValue), model.booking.customer);
                    result = _service.result;
                    if (result.ResultId == 1)
                    {
                        if (SaveOnly)
                        {
                            result.ResultMessage = Url.Action("FinalSummary", "Booking", new { refid = result.TargetUUID });
                        }
                        else
                            result.ResultMessage = Url.Action("LocationSelection", "Booking", new { refid = result.TargetUUID });
                    }
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LocationSelection(string refid = "", bool SaveOnly = false)
        {
            ViewBag.SaveOnly = SaveOnly;
            ContainerBooking model = _service.GetbookingByUUID(refid);
            ViewBag.ClientDropdown = Dropdowns.EmptyDropdown();
            ViewBag.PortDropdown = Dropdowns.EmptyDropdown();
            ViewBag.ShipperPICDropdown = Dropdowns.GetPICByDetailIDDropdown(model.booking.location.shipperdetailid.EncryptedValue);
            ViewBag.ConsigneePICDropdown = Dropdowns.GetPICByDetailIDDropdown(model.booking.location.consigneedetailid.EncryptedValue);
            return View(model);
        }

        public ActionResult LoadShipperPIC(string clientdetailid)
        {
            var booking = SessionManager.Booking;
            ViewBag.ShipperPICDropdown = Dropdowns.GetPICByDetailIDDropdown(clientdetailid);
            return PartialView("~/Views/Booking/_ShipperPIC.cshtml", booking);
        }

        public ActionResult LoadConsigneePIC(string clientdetailid)
        {
            var booking = SessionManager.Booking;
            ViewBag.ConsigneePICDropdown = Dropdowns.GetPICByDetailIDDropdown(clientdetailid);
            return PartialView("~/Views/Booking/_ConsigneePIC.cshtml", booking);
        }

        [HttpPost]
        public ActionResult GetDirectVoyageSearch(string originportid, string destinationportid, string selectedvoyageuuid = "", bool ischange = false)
        {
            var list = _voyageService.GetDirectVoyageSearch(originportid, destinationportid, selectedvoyageuuid, ischange);
            return PartialView("_VoyageOptions", list);
        }


        [HttpPost]
        public ActionResult LocationSelection(ContainerBooking model, bool SaveOnly = false)
        {
            Result result = Common.ErrorMessage("Cannot save location details");
            try
            {
                _service.SaveLocation(model.booking.bookinguuid, model.booking.location);
                result = _service.result;
                if (result.ResultId == 1)
                {
                    if (SaveOnly)
                    {
                        result.ResultMessage = Url.Action("FinalSummary", "Booking", new { refid = model.booking.bookinguuid });
                    }
                    else
                        result.ResultMessage = Url.Action("ContainerSelection", "Booking", new { refid = model.booking.bookinguuid });
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContainerSelection(string refid = "", bool SaveOnly = false)
        {
            ViewBag.SaveOnly = SaveOnly;
            var booking = _service.GetbookingByUUID(refid);
            ViewBag.ClientDropdown = Dropdowns.EmptyDropdown();
            ViewBag.UOMDropdown = Dropdowns.GetUOMDropdown();
            return View(booking);
        }

        [HttpGet]
        public ActionResult GetContainerModal(string bookingdetailuuid = "")
        {
            var sessionBooking = SessionManager.Booking;
            ContainerBookingDetailDTO detail = new ContainerBookingDetailDTO();
            if (!string.IsNullOrEmpty(bookingdetailuuid) && sessionBooking.booking.details != null && sessionBooking.booking.details.Any())
            {
                detail = sessionBooking.booking.details.FirstOrDefault(x => x.bookingdetailuuid == bookingdetailuuid) ?? new ContainerBookingDetailDTO();
            }
            if (sessionBooking.booking.bookingid != null)
            {
                detail.bookingid = sessionBooking.booking.bookingid;
            }
            ViewBag.OwnershipList = Dropdowns.GetOwnershipDropdown();
            ViewBag.ContainerType = Dropdowns.GetContainerTypesDropdown();
            ViewBag.ContainerSize = Dropdowns.GetContainerSizesDropdown();
            ViewBag.PackageType = Dropdowns.GetPackageTypeDropdown();
            ViewBag.ShuntingServices = _service.GetShuntingServices();
            ViewBag.emptyorfull = Dropdowns.GetFullOREmptyDropdown(true);
            if (!string.IsNullOrEmpty(detail.bookingdetailuuid))
            {
                ViewBag.ContainerModelDropdown = Dropdowns.GetContainerModelDropDownByTypeSize(detail.containertypeid.EncryptedValue, detail.sizeid.EncryptedValue);
            }
            else
            {
                ViewBag.ContainerModelDropdown = Dropdowns.EmptyDropdown();
            }
            return PartialView("_ModalContainerBooking", detail);
        }

        public ActionResult LoadContainerModel(string typeid, string sizeid)
        {
            var model = new ContainerBookingDetailDTO();
            ViewBag.ContainerModelDropdown = Dropdowns.GetContainerModelDropDownByTypeSize(typeid, sizeid);
            return PartialView("~/Views/Booking/_ContainerModelDropdown.cshtml", model);
        }

        [HttpPost]
        public ActionResult SaveBookingContainer(ContainerBookingDetailDTO model)
        {
            Result result = Common.ErrorMessage("Cannot save Container details");
            try
            {
                var sessionBooking = SessionManager.Booking;
                string bookingid = model.bookingid.EncryptedValue;
                _service.SaveContainers(bookingid, model);
                result = _service.result;
                if (result.ResultId == 1)
                {
                    result.ResultMessage = Url.Action("ContainerSelection", "Booking", new { refid = sessionBooking.booking.bookinguuid });
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteBookingContainer(string bookingdetailuuid)
        {
            Result result = Common.ErrorMessage("Cannot save Container details");
            try
            {
                var sessionBooking = SessionManager.Booking;
                _service.DeleteBookingContainerByUUID(bookingdetailuuid);
                result = _service.result;
                if (result.ResultId == 1)
                {
                    result.ResultMessage = Url.Action("ContainerSelection", "Booking", new { refid = sessionBooking.booking.bookinguuid });
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetCabatoge(string refid)
        //{
        //    BookedContainer book = _service.GetBookedContainers(refid, new BookedContainerFilter() { });
        //    return PartialView("_ModalCabotage", book);
        //}

        //[HttpPost]
        //public ActionResult SaveContainerBookingPickSelection(ContainerBooking model)
        //{
        //    Result result = Common.ErrorMessage("Cannot save Pick Selection");
        //    try
        //    {
        //        _service.SaveContainerBookingPickSelection(model);
        //        result = _service.result;
        //        if (result.ResultId == 1)
        //        {
        //            result.ResultMessage = Url.Action("PDFDownload", "Download", new { refid = result.TargetUUID, type = "releaseorder", code = "release_" + Guid.NewGuid().ToString("N"), Download = 0 });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = Common.ErrorMessage(ex.Message);
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult SaveBookingAdditionalServices(ContainerBooking model)
        //{
        //    var sessionBooking = SessionManager.Booking;
        //    Result result = Common.ErrorMessage("Cannot save Container details");
        //    try
        //    {
        //        var services = model.booking.additionalservices;
        //        _service.SaveBookingAdditionalServices(sessionBooking.booking.bookingid.EncryptedValue, services);
        //        result = _service.result;
        //        result.ResultId = 1;
        //        result.ResultMessage = Url.Action("FinalSummary", "Booking", new { refid = sessionBooking.booking.bookinguuid });
        //    }
        //    catch (Exception ex)
        //    {
        //        result = Common.ErrorMessage(ex.Message);
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult SaveBookingAdditionalServices(ContainerBooking model)
        {
            var sessionBooking = SessionManager.Booking;
            Result result = Common.ErrorMessage("Cannot save Container details");
            try
            {
                _service.SaveBookingAdditionalServices(sessionBooking.booking.bookingid.EncryptedValue, model);
                result = _service.result;
                result.ResultId = 1;
                result.ResultMessage = Url.Action("FinalSummary", "Booking", new { refid = sessionBooking.booking.bookinguuid });
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult ContainerSelection(ContainerBooking model)
        //{
        //    return RedirectToAction("FinalSummary");
        //}

        public ActionResult FinalSummary(string refid = "")
        {
            ViewBag.CurrencyDropdown = Dropdowns.GetCurrencyDropdown();
            var booking = _service.GetbookingByUUID(refid);
            return View(booking);
        }

        [HttpGet]
        public ActionResult OtherFeesLineItem(string otherfeeuuid = "")
        {
            var model = _service.GetOtherFeesLineItem(otherfeeuuid);
            ViewBag.UOMDropdown = Dropdowns.GetUOMDropdown();
            return PartialView("_ModalOtherFeesLineItem", model);
        }

        [HttpPost]
        public ActionResult SaveOtherFeesLineItem(OtherFeesDTO model)
        {
            var updatedQuotation = _service.SaveOtherFeesLineItem(model);
            return PartialView("_OtherFeesTable", updatedQuotation);
        }

        [HttpPost]
        public ActionResult DeleteOtherFeesLineItem(string otherfeeuuid)
        {
            var details = SessionManager.Booking;
            details.bookingSummary.otherfees.RemoveAll(x => x.otherfeeuuid == otherfeeuuid);
            SessionManager.Booking = details;
            return PartialView("_OtherFeesTable", details);
        }

        [HttpPost]
        public ActionResult SaveBookingSummaryDetails(ContainerBooking model)
        {
            Result result = new Result();
            try
            {
                var sessionBooking = SessionManager.Booking;
                _service.SaveSummaryInfo(model.bookingSummary);
                result = _service.result;
                if (result.ResultId == 1)
                {
                    result.ResultMessage = Url.Action("CreateQuotation", "Booking", new { bookinguuid = sessionBooking.booking.bookinguuid });
                }
            }
            catch (Exception ex)
            {
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReloadSummary()
        {
            var booking = SessionManager.Booking ?? new ContainerBooking();
            return PartialView("_SummaryFixed", booking);
        }

        [HttpPost]
        public ActionResult SetCustomerType(string customertype)
        {
            SessionManager.Booking = SessionManager.Booking ?? new ContainerBooking();
            SessionManager.Booking.booking.customer.customertype = new EncryptedData() { EncryptedValue = customertype, NumericValue = Common.Decrypt(customertype) };
            return Json(Common.SuccessMessage("Success"), JsonRequestBehavior.AllowGet);
        }

        #region Quotation
        public ActionResult BookingQuotationList(string refid)
        {
            BookingQuotationList model = new BookingQuotationList();
            model = _service.GetQuotationListBookingUUID(refid);
            return View(model);
        }

        [HttpGet]
        public ActionResult CreateQuotation(string bookinguuid = "", string refid = "")
        {
            BookingQuotation quotation = new BookingQuotation();
            if (!string.IsNullOrEmpty(refid))
            {
                quotation = _service.GetQuotationByUUID(refid);
            }
            else
            {
                quotation = _service.PrePopulateQuotation(bookinguuid);
            }
            BindQuotationDropdowns(quotation);
            return View(quotation);
        }

        private void BindQuotationDropdowns(BookingQuotation model)
        {
            var agencies = Dropdowns.GetAgenciesDetailIDDropdown();
            ViewBag.AgenciesDropdown = Common.AlterDropdown(agencies, model.quotation.agencydetailid?.EncryptedValue, model.quotation.agencyname);
            ViewBag.CurrencyDropdown = Dropdowns.GetCurrencyDropdown();
            ViewBag.CustomerTypeDropdown = Dropdowns.GetCustomerTypeDropdown();
            ViewBag.ClientDropdown = Common.AlterDropdown(Dropdowns.EmptyDropdown(), model.quotation.quotationfor.EncryptedValue, string.IsNullOrEmpty(model.quotation.quotationforname) ? "Select Client" : model.quotation.quotationforname);
        }

        [HttpGet]
        public JsonResult GetTermsAndConditionsByAgency(string Agency)
        {
            var termsandconditions = _termsandconditions.GetTermsAndConditionsbyTypeandAgency("ContainerQuotation", Agency);
            if (termsandconditions == null)
            {
                return Json(new { success = false, termsText = "" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                success = true,
                termsText = termsandconditions.TermsText
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetBillToByCustomerType(string customertype)
        {
            string billToAddress = _service.GetBillToAddressByCustomerType(customertype);
            return Json(new
            {
                success = true,
                billTo = billToAddress
            });
        }

        [HttpPost]
        public ActionResult GetBillToByClient(string clientdetailid)
        {
            ClientDTO client = _service.GetBillToAddressByClient(clientdetailid);
            return Json(new
            {
                success = true,
                billTo = client.address,
                currency = client.preferredcurrency
            });
        }

        [HttpGet]
        public ActionResult QuotationLineItem(string quotationdetailuuid = "")
        {
            var model = _service.GetQuotationLineItem(quotationdetailuuid);
            ViewBag.UOMDropdown = Dropdowns.GetUOMDropdown();
            ViewBag.JobTypeDetailDropdown = Dropdowns.GetJobTypeDetailDropdown("5");
            ViewBag.ContainerTypeSizeDropdown = Dropdowns.GetContainerTypeSizeDropdown();
            ViewBag.FullEmptyDropdown = Dropdowns.GetFullOREmptyDropdown(true);
            return PartialView("_ModalQuotationLineItem", model);
        }

        [HttpPost]
        public ActionResult SaveQuotationLineItem(QuotationDetailDTO model)
        {
            var updatedQuotation = _service.SaveQuotationLineItem(model);
            return PartialView("_QuotationLineItemsTable", updatedQuotation);
        }

        [HttpPost]
        public ActionResult CreateQuotation(BookingQuotation model)
        {
            Result result = new Result();
            _service.SaveQuotation(model.quotation.bookinguuid, model.quotation);
            result = _service.result;
            if (result.ResultId == 1)
            {
                result.ResultMessage = Url.Action("BookingQuotationList", "Booking", new { refid = model.quotation.bookinguuid });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteQuotationLineItem(string quotationdetailuuid)
        {
            var details = SessionManager.Quotation;
            details.quotation.details.RemoveAll(x => x.quotationdetailuuid == quotationdetailuuid);
            SessionManager.Quotation = details;
            return PartialView("_QuotationLineItemsTable", details);
        }

        [HttpPost]
        public ActionResult DeleteQuotationByUUID(string quotationuuid)
        {
            Result result = new Result();
            try
            {
                _service.DeleteQuotationByUUID(quotationuuid);
                result = _service.result;
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private void QuotationDropDowns(Quotationlist bookinglist)
        {

            ViewBag.PortDropdown = Dropdowns.EmptyDropdown();
            ViewBag.AgenciesUUIDDropdown = Dropdowns.GetAgenciesUUIDDropdown();
            ViewBag.ClientUUIDDropdown = Dropdowns.GetClientsByUserIDDropdown();
            ViewBag.CreatedByDropdown = Dropdowns.GetLoginUsersByRole(Common.Encrypt(0), "", false, false);
            ViewBag.StatusDropdown = Dropdowns.GetQuotationStatusDropdown();
            ViewBag.HODApprovalUserDropdown = Dropdowns.GetLoginUsersByRole(Common.Encrypt(UserRols.Approver_HOD), "");
        }
        public ActionResult QuotationList()
        {
            QuotationListFilter filter = PageSessionManager.GetFilter<QuotationListFilter>(PageKeys.Quotation);
            if (filter.filters == null) filter.filters = new QuotationFilter();
            _service.PopulateStatusCounts(filter);
            SetFilterSession(filter);
            QuotationData(filter);
            return View(filter);
        }
        [HttpPost]
        public ActionResult QuotationList(QuotationListFilter filter, string action)
        {
            int ActiveTab = PageSessionManager.GetAttribute<int>(PageKeys.Quotation, "ActiveTab", 0); ;
            if (action == "Reset")
            {
                ModelState.Clear();
                filter = new QuotationListFilter();
                SessionManager.QuotationListFilter = null;
            }
            else
            {
                if (filter.filters.status != ActiveTab)
                {
                    ActiveTab = 0;
                }
                SessionManager.QuotationListFilter = filter;
            }
            PageSessionManager.SetAttribute(PageKeys.Quotation, "ActiveTab", ActiveTab);
            SetFilterSession(filter);
            _service.PopulateStatusCounts(filter);
            QuotationData(filter);
            return View(filter);
        }
        public ActionResult ListStatus(int status)
        {
            PageSessionManager.SetAttribute(PageKeys.Quotation, "ActiveTab", status);
            QuotationListFilter filter = PageSessionManager.GetFilter<QuotationListFilter>(PageKeys.Quotation);
            if (filter.filters == null) filter.filters = new QuotationFilter();
            filter.filters.status = status;
            SetFilterSession(filter);
            return RedirectToAction("QuotationList");
        }
        private void SetFilterSession(QuotationListFilter filter)
        {
            SessionManager.QuotationListFilter = filter;
            PageSessionManager.SetFilter(PageKeys.Quotation, filter);
        }
        private void QuotationData(QuotationListFilter filter)
        {
            SessionManager.QuotationListFilter = filter;
            QuotationDropDowns(new Quotationlist());
        }
        #endregion

        public ActionResult GetQuoteApproval(string QuoteUUID)
        {
            BookingQuotation quote = _service.GetQuotationByUUID(QuoteUUID);
            string comments = IsAutoApproveEligible(quote);
            if (comments == "")
            {
                SendApproval approval = new SendApproval();
                approval.UUID = QuoteUUID;
                approval.ID = quote.quotation.quotationid;
                approval.ApproveStatus = ApproveStatus.SendForApproval;
                approval.ApprovalType = "HOD Approval";
                approval.ApproveOrder = 1;

                AgencyDTO agency = _agencyService.GetAgencyByDetailID(quote.booking.customer.agencydetailid.NumericValue);

                ViewBag.ApprovalUserDropdown = Dropdowns.GetLoginUsersByRole(Common.Encrypt(UserRols.Approver_HOD), agency.agencyid.EncryptedValue);
                return PartialView("_ModalSendApproval", approval);
            }
            else
            {
                var result = AutoApprove(quote, comments);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        private Result AutoApprove(BookingQuotation quote, string comments)
        {
            SendApproval approval = new SendApproval();
            approval.UUID = quote.quotation.quotationuuid;
            approval.ID = quote.quotation.quotationid;
            approval.ApproveStatus = ApproveStatus.AutoApproved;
            approval.ApprovalType = "HOD Approval";
            approval.ApproveOrder = 1;

            _service.SendQuotationForApproval(approval);
            if (!string.IsNullOrEmpty(comments))
            {
                if (_service.result.ResultId == 1)
                {
                    ChatDTO chat = new ChatDTO();
                    chat.target_id = approval.ID.EncryptedValue;
                    chat.type = ChatType.Quotation;
                    chat.message_text = comments;
                    chat.autoapprove = true;
                    _chatService.SaveMessage(chat);
                    _service.result = _chatService.result;
                }
            }
            if (_service.result.ResultId == 1)
            {
                _service.result.ResultMessage = NavigationStackManager.GetListURL(ChatType.Quotation);
            }
            return _service.result;
        }

        private string IsAutoApproveEligible(BookingQuotation quote)
        {
            string result = "";
            if ((quote.quotation.status.Value == QuoteStatus.Draft || quote.quotation.status.Value == QuoteStatus.HOD_Rejected)
                && Common.RoleID == UserRols.Approver_HOD)
            {
                result = "Auto-approved by HOD Approver (" + Common.LoginName + ")";
            }
            else
            {
                int count = quote.quotation.details.Count(x => x.amount != x.templateprice);
                if (count == 0)
                {
                    result = "Auto-approved since pricing matches the template with no changes";
                }
            }
            return result;
        }
        [HttpPost]
        public ActionResult SaveApprovalAction(SendApproval approval)
        {
            _service.SendQuotationForApproval(approval);
            if (!string.IsNullOrEmpty(approval.Comments))
            {
                if (_service.result.ResultId == 1)
                {
                    ChatDTO chat = new ChatDTO();
                    chat.target_id = approval.ID.EncryptedValue;
                    chat.type = ChatType.Quotation;
                    chat.message_text = approval.Comments;
                    _chatService.SaveMessage(chat);
                    _service.result = _chatService.result;
                }
            }
            if (_service.result.ResultId == 1)
            {
                _service.result.ResultMessage = NavigationStackManager.GetListURL(ChatType.Quotation);
            }
            return Json(_service.result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApproveQuotation(string refid)
        {
            BookingQuotation quotation = new BookingQuotation();
            quotation = _service.GetQuotationByUUID(refid);
            ViewBag.ReadOnly = true;
            return View(quotation);
        }

        public ActionResult GetApproveReject(string QuoteUUID, int ApprovalStatus)
        {
            BookingQuotation quote = _service.GetQuotationByUUID(QuoteUUID);

            SendApproval approval = new SendApproval();
            approval.UUID = QuoteUUID;
            approval.ID = quote.quotation.quotationid;
            approval.ApprovalType = "HOD Approval";
            approval.ApproveOrder = 1;
            approval.ApproveStatus = ApprovalStatus;

            return PartialView("_ModalApproveReject", approval);
        }

        [HttpPost]
        public ActionResult GetPricingByLineItem(string BookingUUID, string ClientDetailID, string Currency, string LineItemUUID, string TypeID, string SizeID, string FullEmpty)
        {
            PricingDetailDTO type = _pricingService.GetPricingByLineItemUUID(BookingUUID, ClientDetailID, Currency, LineItemUUID, TypeID, SizeID, FullEmpty);
            return Json(type, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetPricingByBookingUUID(string BookingUUID, string Currency, string ClientDetailID)
        {
            var pricelist = _pricingService.GetPricingListByBookingUUID(BookingUUID, Currency, ClientDetailID);
            return PartialView("_QuotationLineItemsTable", new BookingQuotation()
            {
                quotation = new QuotationHeaderDTO() { details = pricelist }
            });
        }

        [HttpGet]
        public ActionResult GetBookingSummaryModal(string bookinguuid)
        {
            BookingSummaryDTO model = new BookingSummaryDTO();
            model = _service.GetBookingSummaryInfo(bookinguuid);
            if (model == null || string.IsNullOrEmpty(model.bookinguuid))
            {
                model.bookinguuid = bookinguuid;
            }
            ViewBag.CurrencyDropdown = Dropdowns.GetCurrencyDropdown();
            return PartialView("_ModalBookingSummary", model);
        }

        [HttpGet]
        public ActionResult ValidateContainerDetail(string bookinguuid, string bookingdetailuuid, int qty)
        {
            if (!string.IsNullOrEmpty(bookingdetailuuid))
            {
                _service.ValidateContainerQty(bookinguuid, bookingdetailuuid, qty);
                return Json(_service.result, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(Common.SuccessMessage("Success"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveBookingSummary(BookingSummaryDTO model)
        {
            Result result = new Result();
            _service.SaveSummaryInfo(model);
            result = _service.result;
            if (result.ResultId == 1)
            {
                string generatedCode = "SUM_" + Guid.NewGuid() + "";
                result.ResultMessage = Url.Action("PDFDownload", "Download", new { refid = model.bookinguuid, type = "bookingsummary", code = generatedCode, Download = "1" });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ShipmentConfirmation(string refid)
        {
            BookedContainer book = _service.GetBookedContainers(refid, new BookedContainerFilter() { }, true);
            return View(book);
        }
        public ActionResult CabotageHistory(string refid)
        {
            Cabotage model = _service.GetCabotageList(refid);
            return View(model);
        }

        [HttpPost]
        public ActionResult ShipmentConfirmation(BookedContainer book, string buttonaction)
        {
            if (buttonaction == "Cabotage")
            {
                _service.SaveCabotage(book.booking.bookinguuid, book.containers.Where(x => x.IsChecked).Select(x => x.ContainerUuid).ToList());
                if (_service.result.ResultId == 1)
                {
                    _service.result.ResultMessage = Url.Action("PDFDownload", "Download", new { refid = _service.result.TargetUUID, type = "cabotage", code = "Cabotage", Download = 1 });
                }
            }
            else
            {
                _service.SaveShipmentConfimation(book.booking.bookinguuid, book.containers);
                if (_service.result.ResultId == 1)
                {
                    _service.result.ResultMessage = Url.Action("PDFDownload", "Download", new { refid = book.booking.bookinguuid, type = "shipmentconfirm", code = "ShipmentConfirm", Download = 1 });
                }
            }
            return Json(_service.result, JsonRequestBehavior.AllowGet);
        }
    }
}