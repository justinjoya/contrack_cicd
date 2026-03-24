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
    public class AgencyController : Controller
    {
        private readonly AgencyService _agencyService;
        public AgencyController()
        {
            IAgencyRepository repo = new AgencyRepository();
            _agencyService = new AgencyService(repo);
        }
        public ActionResult List()
        {
            return View();
        }
        public ActionResult Details(string refid = "")
        {
            Agency agency = new Agency()
            {
                agency = _agencyService.GetAgencyByUUID(refid)
            };
            return View(agency);
        }
        public ActionResult GetAgencyModal(string refid)
        {
            Agency agency = new Agency()
            {
                agency = _agencyService.GetAgencyByUUID(refid)
            };
            ViewBag.CountryDropdown = Dropdowns.GetCountryDropdown();
            ViewBag.PortDropdown = Dropdowns.GetPortDropdown();
            return PartialView("~/Views/Shared/Masters/_ModalAgency.cshtml", agency);
        }
        [HttpPost]
        public ActionResult SaveAgency(Agency model)
        {
            _agencyService.SaveAgency(model);
            return Json(_agencyService.result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteAgency(Agency ag)
        {
            _agencyService.DeleteAgency(Common.Decrypt(ag.agency.agencyid.EncryptedValue), ag.agency.uuid);
            return Json(_agencyService.result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OpenKeyValuePair(string refid)
        {
            Agency currentagency = SessionManager.CurrenyAgency;
            if (currentagency == null)
                currentagency = new Agency();
            KeyValuePair pair = currentagency.agency.CustomAttributes.Where(x => x.UUID == refid).FirstOrDefault();
            if (pair == null)
                pair = new KeyValuePair();
            return PartialView("~/Views/Shared/Masters/Agency/_ModalCustomKeyValue.cshtml", pair);
        }

        [HttpPost]
        public ActionResult UpdateCustomAttribute(KeyValuePair pair)
        {
            Agency currentagency = SessionManager.CurrenyAgency;
            if (currentagency == null)
                currentagency = new Agency();

            if (pair != null)
            {
                var found = currentagency.agency.CustomAttributes.Where(x => x.UUID == pair.UUID).FirstOrDefault();
                if (found != null)
                    currentagency.agency.CustomAttributes.Where(x => x.UUID == pair.UUID).ToList().ForEach(x =>
                    {
                        x.KeyName = pair.KeyName;
                        x.KeyValue = pair.KeyValue;
                    });
                else
                    currentagency.agency.CustomAttributes.Add(pair);
            }
            _agencyService.SaveCustomAttribute(currentagency.agency.CustomAttributes, currentagency.agency.agencyid.NumericValue, currentagency.agency.uuid);
            return PartialView("~/Views/Shared/Masters/Agency/_AgencyCustomAtrributes.cshtml", new Agency
            {
                agency = new AgencyDTO
                {
                    CustomAttributes = currentagency.agency.CustomAttributes
                }
            });
        }

        public ActionResult DeleteAgencyCustomAttribute(string refid)
        {
            Agency currentagency = SessionManager.CurrenyAgency;
            if (currentagency == null)
                currentagency = new Agency();

            currentagency.agency.CustomAttributes.RemoveAll(x => x.UUID == refid);
            _agencyService.SaveCustomAttribute(currentagency.agency.CustomAttributes, currentagency.agency.agencyid.NumericValue, currentagency.agency.uuid);
            return PartialView("~/Views/Shared/Masters/Agency/_AgencyCustomAtrributes.cshtml", new Agency()
            {
                agency = new AgencyDTO
                {
                    CustomAttributes = currentagency.agency.CustomAttributes
                }
            });
        }

        public ActionResult OpenBankAccount(string refid)
        {
            Agency currentagency = SessionManager.CurrenyAgency;
            if (currentagency == null)
                currentagency = new Agency();
            CustomBankInfo account = currentagency.agency.BankInfo.Where(x => x.BankAccountUUID == refid).FirstOrDefault();
            if (account == null)
            {
                account = new CustomBankInfo();
            }
            account.FillBankKeyValues();
            ViewBag.CurrencyDropdown = Dropdowns.GetCurrencyDropdown();
            return PartialView("~/Views/Shared/Masters/Agency/_ModalBankAccount.cshtml", account);
        }

        [HttpPost]
        public ActionResult UpdateBankAccount(CustomBankInfo bank)
        {
            Agency currentagency = SessionManager.CurrenyAgency;
            if (currentagency == null)
                currentagency = new Agency();

            if (bank != null)
            {
                bank.BankAttributes = bank.BankAttributes.Where(x => !string.IsNullOrEmpty(x.KeyName) && !string.IsNullOrEmpty(x.KeyValue)).ToList();
                if (bank.BankAttributes.Count == 0)
                {
                    currentagency.agency.BankInfo.RemoveAll(x => x.BankAccountUUID == bank.BankAccountUUID);
                }
                else
                {
                    var found = currentagency.agency.BankInfo.Where(x => x.BankAccountUUID == bank.BankAccountUUID).FirstOrDefault();
                    if (found != null)
                        currentagency.agency.BankInfo.Where(x => x.BankAccountUUID == bank.BankAccountUUID).ToList().ForEach(x =>
                        {
                            x.AliasName = bank.AliasName;
                            x.Currency = bank.Currency;
                            x.BankAttributes = bank.BankAttributes;
                        });
                    else
                        currentagency.agency.BankInfo.Add(bank);
                }
            }
            _agencyService.SaveBankInfo(currentagency.agency.BankInfo, currentagency.agency.agencyid.NumericValue, currentagency.agency.uuid);
            return PartialView("~/Views/Shared/Masters/Agency/_AgencyBankAccounts.cshtml", new Agency()
            {
                agency = new AgencyDTO
                {
                    BankInfo = currentagency.agency.BankInfo
                }
            });
        }

        public ActionResult DeleteBankAccount(string refid)
        {
            Agency currentagency = SessionManager.CurrenyAgency;
            if (currentagency == null)
                currentagency = new Agency();
            currentagency.agency.BankInfo.RemoveAll(x => x.BankAccountUUID == refid);
            _agencyService.SaveBankInfo(currentagency.agency.BankInfo, currentagency.agency.agencyid.NumericValue, currentagency.agency.uuid);
            return PartialView("~/Views/Shared/Masters/Agency/_AgencyBankAccounts.cshtml", new Agency()
            {
                agency = new AgencyDTO
                {
                    BankInfo = currentagency.agency.BankInfo
                }
            });
        }

        [AuthorizeRoles(LoginType.Hub, Order = 4)]
        public ActionResult Logs(string refid = "")
        {
            Agency agency = new Agency();
            agency.agency = _agencyService.GetAgencyByUUID(refid);
            AgencyLog log = new AgencyLog();
            log.Info = agency;
            log.Logs = _agencyService.GetAgencyModificationList(refid);
            return View(log);
        }

        [AuthorizeRoles(LoginType.Hub, Order = 4)]
        public ActionResult Users(string refid = "")
        {
            Agency agency = new Agency();
            agency.agency = _agencyService.GetAgencyByUUID(refid);
            UserFilter filter = new UserFilter();
            filter.UserType = Common.Encrypt(2);
            filter.EntityID = new List<string>();
            filter.EntityID.Add(Common.Encrypt(agency.agency.agencyid.NumericValue));
            SessionManager.UserListFilter = filter;
            return View(agency);
        }

        public ActionResult Clients(string refid = "")
        {
            Agency agency = new Agency();
            agency.agency = _agencyService.GetAgencyByUUID(refid);
            ClientListFilter filter = new ClientListFilter();
            filter.AgencyID = agency.agency.agencyid.EncryptedValue;
            SessionManager.ClientListFilter = filter;
            return View(agency);
        }

        public ActionResult Vessels(string refid = "")
        {
            Agency agency = new Agency();
            agency.agency = _agencyService.GetAgencyByUUID(refid);
            VesselFilter filter = new VesselFilter();
            filter.AgencyID = agency.agency.agencyid.EncryptedValue;
            SessionManager.VesselListFilter = filter;
            return View(agency);
        }

        //public ActionResult AgencyClients()
        //{
        //    ClientListFilter filter = new ClientListFilter();
        //    switch (Common.UserType)
        //    {
        //        case 1:// Hub
        //            filter.AgencyID = Common.Encrypt(0);
        //            break;
        //        case 2:// Agency
        //            //filter.UserType = Common.Encrypt(2);
        //            // Set Agency UID
        //            break;
        //        default:
        //            //filter.UserType = Common.Encrypt(10); // To Make not to display anything
        //            break;
        //    }
        //    SessionManager.ClientListFilter = filter;
        //    return View();
        //}
    }
}