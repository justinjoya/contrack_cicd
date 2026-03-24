using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace Contrack
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    public class PricingController : Controller
    {
        private readonly PricingService _service;
        // GET: Pricing
        public PricingController()
        {
            IPricingRepository repo = new PricingRepository();
            _service = new PricingService(repo);
        }
        private void PricingDropDowns()
        {
            ViewBag.POLDropdown = Dropdowns.GetPortDropdown("", true);
            ViewBag.PODDropdown = Dropdowns.GetPortDropdown("", true);
            ViewBag.TransferTypeDropdown = Dropdowns.GetTransferTypeDropdown(true);
            ViewBag.ClientDropdown = Dropdowns.GetClientsByUserIDDropdown(true);
        }
        public ActionResult List()
        {
            PricingListFilter filter = SessionManager.PricingListFilter;
            if (filter == null)
                filter = new PricingListFilter();
            SessionManager.PricingListFilter = filter;
            PricingDropDowns();
            return View(filter);
        }
        [HttpPost]
        public ActionResult List(PricingListFilter filter, string action)
        {
            if (action == "Reset")
            {
                SessionManager.PricingListFilter = null;
                return RedirectToAction("List");
            }
            SessionManager.PricingListFilter = filter;
            PricingDropDowns();
            return View(filter);
        }
        public ActionResult GetPricingModal(string pricinguuid = "")
        {
            PricingHeaderDTO header = new PricingHeaderDTO();
            if (!string.IsNullOrEmpty(pricinguuid))
            {
                QuotationPricing pricing = _service.GetPricingByHeader(pricinguuid, "", true);
                header = pricing.pricing;
            }
            var pols = Dropdowns.EmptyDropdown();
            var pods = Dropdowns.EmptyDropdown();
            pols = Common.AlterDropdown(pols, header.POL.PortID.EncryptedValue, header.POL.PortName);
            pods = Common.AlterDropdown(pods, header.POL.PortID.EncryptedValue, header.POL.PortName);
            ViewBag.POLDropdown = pols;
            ViewBag.PODDropdown = pods;
            ViewBag.CurrencyDropdown = Dropdowns.GetCurrencyDropdown();
            return PartialView("_ModalPricing", header);
        }
        public ActionResult OpenPricingType(string pricinguuid, string typeid = "")
        {
            PricingTypeDTO currency = new PricingTypeDTO();
            currency.PricingUUID = pricinguuid;
            ViewBag.TransferTypeDropdown = Dropdowns.GetTransferTypeDropdown(true);
            return PartialView("_ModalType", currency);
        }
        public ActionResult OpenClonePricingCurrency(string pricinguuid, string sourcecurrencyid)
        {
            PricingTypeDTO currency = new PricingTypeDTO();
            currency.PricingUUID = pricinguuid;
            currency.TypeID = new EncryptedData() { EncryptedValue = sourcecurrencyid };
            currency.IsClone = 1;
            ViewBag.CurrencyDropdown = Dropdowns.GetCurrencyDropdown();
            return PartialView("_ModalCurrency", currency);
        }

        public ActionResult OpenClonePricingHeader(string sourcepricingid)
        {
            PricingHeaderDTO header = new PricingHeaderDTO();
            header.PricingID = new EncryptedData() { EncryptedValue = sourcepricingid };
            header.IsClone = 1;
            var pols = Dropdowns.EmptyDropdown();
            var pods = Dropdowns.EmptyDropdown();
            pols = Common.AlterDropdown(pols, header.POL.PortID.EncryptedValue, header.POL.PortName);
            pods = Common.AlterDropdown(pods, header.POL.PortID.EncryptedValue, header.POL.PortName);
            ViewBag.POLDropdown = pols;
            ViewBag.PODDropdown = pods;
            ViewBag.CurrencyDropdown = Dropdowns.GetCurrencyDropdown();
            return PartialView("_ModalPricing", header);
        }

        public ActionResult OpenPricingLineItem(string typeid, string detailid = "")
        {
            PricingDetailDTO detail = new PricingDetailDTO();
            detail.TypeID = new EncryptedData() { EncryptedValue = typeid };
            if (!string.IsNullOrEmpty(detailid))
            {
                detail = _service.GetDetail(typeid, detailid);
            }
            ViewBag.JobTypeDetailDropdown = Dropdowns.GetJobTypeDetailDropdown("5");
            ViewBag.ContainerTypeSizeDropdown = Dropdowns.GetContainerTypeSizeDropdown();
            ViewBag.FullEmptyDropdown = Dropdowns.GetFullOREmptyDropdown();
            ViewBag.UOMDropdown = Dropdowns.GetUOMDropdown();
            return PartialView("_ModalLineItem", detail);
        }

        public ActionResult DeletePricingLineItem(string TypeUUID, string DetailUUID)
        {
            _service.DeleteDetail(TypeUUID, DetailUUID);
            return Json(_service.result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SavePricingTemplate(PricingHeaderDTO header)
        {
            if (header.IsClone == 0)
                _service.SaveHeader(header);
            else
                _service.CloneHeader(header);

            if (_service.result.ResultId == 1)
            {
                _service.result.ResultMessage = Url.Action("Template", "Pricing", new { refid = _service.result.TargetUUID });
            }
            return Json(_service.result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SavePricingType(PricingTypeDTO currencydto)
        {
            if (currencydto.IsClone == 0)
                _service.SaveType(currencydto);
            else
                _service.CloneType(currencydto);

            if (_service.result.ResultId == 1)
            {
                _service.result.ResultMessage = Url.Action("Template", "Pricing", new { refid = currencydto.PricingUUID, currency = _service.result.TargetUUID });
            }
            return Json(_service.result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SavePricingDetail(PricingDetailDTO detail)
        {
            _service.SaveDetail(detail);
            return Json(_service.result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddClientToPricing(string pricingid, string clientid)
        {
            PricingCustomerDTO customer = new PricingCustomerDTO()
            {
                ClientID = new EncryptedData()
                {
                    EncryptedValue = clientid
                },
                PricingID = new EncryptedData()
                {
                    EncryptedValue = pricingid
                }
            };
            _service.SaveCustomer(customer);
            return Json(_service.result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReloadClient(string pricingid, string pricinguuid)
        {
            var clientlist = _service.GetClientsList(pricinguuid);
            return PartialView("_ClientList", new QuotationPricing()
            {
                pricing = new PricingHeaderDTO()
                {
                    PricingID = new EncryptedData()
                    {
                        EncryptedValue = pricingid
                    }
                },
                customers = clientlist
            });
        }

        public ActionResult Template(string refid, string currency = "")
        {
            QuotationPricing pricing = new QuotationPricing();
            pricing = _service.GetPricingByHeader(refid, currency);
            return View(pricing);
        }
    }
}