using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Contrack.Controllers
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    [AuthorizeRoles(LoginType.Hub, Order = 4)]
    public class VendorController : Controller
    {
        private readonly VendorService _vendorService;
        public VendorController()
        {
            IDocumentRepository docRepo = new DocumentRepository();
            IVendorRepository repo = new VendorRepository(docRepo);

            _vendorService = new VendorService(repo);
        }
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Details(string refid = "")
        {
            Vendor vendorModel = new Vendor();
            vendorModel.vendor = _vendorService.GetVendorByUUID(refid);
            vendorModel.vendor.Contacts = _vendorService.GetContactList(vendorModel.vendor.vendorid.EncryptedValue);
            return View(vendorModel);
        }

        public ActionResult GetVendorModal(string refid, string agencyid)
        {
            VendorDTO vendor = new VendorDTO();
            vendor.agency.agencyid = new EncryptedData() { EncryptedValue = agencyid };
            if (!string.IsNullOrEmpty(refid))
                vendor = _vendorService.GetVendorByUUID(refid);
            ViewBag.CurrencyDropdown = Dropdowns.GetCurrencyDropdown();
            ViewBag.AgenciesDropdown = Dropdowns.GetAgenciesDropdown(false);

            return PartialView("~/Views/Shared/Masters/_ModalVendor.cshtml", vendor);
        }

        public ActionResult OpenVendorBankAccount(string refid)
        {
            VendorDTO currentvendor = SessionManager.CurrentVendor;
            if (currentvendor == null)
                currentvendor = new VendorDTO();
            CustomBankInfo account = currentvendor.BankInfo.Where(x => x.BankAccountUUID == refid).FirstOrDefault();
            if (account == null)
            {
                account = new CustomBankInfo();
            }
            account.FillBankKeyValues();
            ViewBag.CurrencyDropdown = Dropdowns.GetCurrencyDropdown();
            return PartialView("~/Views/Shared/Masters/Agency/_ModalBankAccount.cshtml", account);
        }

        [HttpPost]
        public ActionResult UpdateVendorBankAccount(CustomBankInfo bank)
        {
            VendorDTO currentvendor = SessionManager.CurrentVendor ?? new VendorDTO();
            if (bank != null)
            {
                bank.BankAttributes = bank.BankAttributes
                    .Where(x => !string.IsNullOrEmpty(x.KeyName) && !string.IsNullOrEmpty(x.KeyValue))
                    .ToList();
                if (bank.BankAttributes.Count == 0)
                {
                    currentvendor.BankInfo.RemoveAll(x => x.BankAccountUUID == bank.BankAccountUUID);
                }
                else
                {
                    var found = currentvendor.BankInfo
                        .FirstOrDefault(x => x.BankAccountUUID == bank.BankAccountUUID);
                    if (found != null)
                    {
                        currentvendor.BankInfo
                            .Where(x => x.BankAccountUUID == bank.BankAccountUUID)
                            .ToList()
                            .ForEach(x =>
                            {
                                x.AliasName = bank.AliasName;
                                x.Currency = bank.Currency;
                                x.BankAttributes = bank.BankAttributes;
                            });
                    }
                    else
                    {
                        currentvendor.BankInfo.Add(bank);
                    }
                }
            }

            _vendorService.SaveBankInfo(
                currentvendor.BankInfo,
                currentvendor.vendorid.NumericValue,
                currentvendor.vendoruuid
            );
            SessionManager.CurrentVendor = currentvendor;
            return PartialView(
                "~/Views/Shared/Masters/Vendor/_VendorBankAccounts.cshtml",
                new Vendor()
                {
                    vendor = new VendorDTO() { BankInfo = currentvendor.BankInfo }
                }
            );
        }
        public ActionResult DeleteVendorBankAccount(string refid)
        {
            VendorDTO currentvendor = SessionManager.CurrentVendor;
            if (currentvendor == null)
                currentvendor = new VendorDTO();

            currentvendor.BankInfo.RemoveAll(x => x.BankAccountUUID == refid);

            _vendorService.SaveBankInfo(currentvendor.BankInfo, currentvendor.vendorid.NumericValue, currentvendor.vendoruuid);
            return PartialView("~/Views/Shared/Masters/Vendor/_VendorBankAccounts.cshtml",
                new Vendor()
                {
                    vendor = new VendorDTO() { BankInfo = currentvendor.BankInfo }
                });
        }


        public ActionResult GetVendorContactModal(string refid, string vendorid)
        {
            VendorContact contact = new VendorContact();
            contact.vendorid.EncryptedValue = vendorid;
            contact.picid.EncryptedValue = refid;
            if (refid != "")
                contact = _vendorService.GetContactByID(refid);
            return PartialView("~/Views/Shared/Masters/Vendor/_ModalVendorContact.cshtml", contact);
        }
        public ActionResult MakeContactPrimary(string refid, string vendorid)
        {
            _vendorService.MakePrimaryContact(refid);
            var list = _vendorService.GetContactList(vendorid);
            return PartialView("~/Views/Master/Vendor/_VendorContacts.cshtml",
                new Vendor()
                {
                    vendor = new VendorDTO()
                    {
                        vendorid = new EncryptedData()
                        {
                            NumericValue = Common.Decrypt(vendorid),
                            EncryptedValue = vendorid
                        },
                        Contacts = list
                    }
                });
        }
        public JsonResult SaveVendorContact(VendorContact contact)
        {
            _vendorService.SaveContact(contact);
            return Json(_vendorService.result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteContact(string refid, string vendorid)
        {
            _vendorService.DeleteContact(refid);
            var list = _vendorService.GetContactList(vendorid);

            return PartialView("~/Views/Master/Vendor/_VendorContacts.cshtml", new Vendor()
            {
                vendor = new VendorDTO()
                {
                    vendorid = new EncryptedData()
                    {
                        NumericValue = Common.Decrypt(vendorid),
                        EncryptedValue = vendorid
                    },
                    Contacts = list
                }
            });
        }

        public ActionResult OpenVendorKeyValuePair(string refid)
        {
            VendorDTO currentvendor = SessionManager.CurrentVendor;
            if (currentvendor == null)
                currentvendor = new VendorDTO();
            KeyValuePair pair = currentvendor.CustomAttributes.Where(x => x.UUID == refid).FirstOrDefault();
            if (pair == null)
                pair = new KeyValuePair();
            return PartialView("~/Views/Shared/Masters/Agency/_ModalCustomKeyValue.cshtml", pair);
        }

        [HttpPost]
        public ActionResult UpdateVendorCustomAttribute(KeyValuePair pair)
        {
            VendorDTO currentvendor = SessionManager.CurrentVendor;
            if (currentvendor == null)
                currentvendor = new VendorDTO();

            if (pair != null)
            {
                var found = currentvendor.CustomAttributes.FirstOrDefault(x => x.UUID == pair.UUID);
                if (found != null)
                {
                    found.KeyName = pair.KeyName;
                    found.KeyValue = pair.KeyValue;
                }
                else
                {
                    currentvendor.CustomAttributes.Add(pair);
                }
            }
            _vendorService.SaveCustomAttribute(
                currentvendor.CustomAttributes,
                Common.Decrypt(currentvendor.vendorid.EncryptedValue),
                currentvendor.vendoruuid
            );
            return PartialView(
                "~/Views/Shared/Masters/Vendor/_VendorCustomAtrributes.cshtml",
                new Vendor()
                {
                    vendor = new VendorDTO() { CustomAttributes = currentvendor.CustomAttributes }
                }

            );
        }
        public ActionResult DeleteVendorCustomAttribute(string refid)
        {
            VendorDTO currentvendor = SessionManager.CurrentVendor;
            if (currentvendor == null)
                currentvendor = new VendorDTO();

            currentvendor.CustomAttributes.RemoveAll(x => x.UUID == refid);

            _vendorService.SaveCustomAttribute(currentvendor.CustomAttributes, currentvendor.vendorid.NumericValue, currentvendor.vendoruuid);
            return PartialView("~/Views/Shared/Masters/Vendor/_VendorCustomAtrributes.cshtml", new Vendor()
            {
                vendor = new VendorDTO
                {
                    CustomAttributes = currentvendor.CustomAttributes
                }
            });
        }
        public ActionResult Logs(string refid = "")
        {
            VendorDTO VendorDTO = new VendorDTO();
            VendorDTO = _vendorService.GetVendorByUUID(refid);
            VendorLog log = new VendorLog();
            log.Info = VendorDTO;
            log.Logs = _vendorService.GetVendorLogsByUUID(refid);
            return View(log);
        }
        [HttpPost]
        public ActionResult SaveVendor(VendorDTO vendor)
        {
            _vendorService.SaveVendor(vendor);
            return Json(_vendorService.result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteVendor(Vendor model)
        {
            _vendorService.DeleteVendor(model);
            return Json(_vendorService.result, JsonRequestBehavior.AllowGet);
        }
    }
}

