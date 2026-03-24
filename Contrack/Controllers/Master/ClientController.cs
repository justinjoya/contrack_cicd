using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace Contrack.Controllers.Master
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    [AuthorizeRoles(LoginType.Hub, Order = 4)]
    public class ClientController : Controller
    {
        private readonly ClientService _clientService;
        public ClientController()
        {
            IClientRepository repo = new ClientRepository();
            _clientService = new ClientService(repo);
        }
        public ActionResult List()
        {
            ClientListFilter filter = new ClientListFilter();
            switch (Common.UserType)
            {
                case 1:// Hub
                    filter.AgencyID = Common.Encrypt(0);
                    break;
                case 2:// Agency
                    //filter.UserType = Common.Encrypt(2);
                    // Set Agency UID
                    break;
                default:
                    //filter.UserType = Common.Encrypt(10); // To Make not to display anything
                    break;
            }
            SessionManager.ClientListFilter = filter;
            return View();
        }

        public ActionResult GetClientModal(string refid, string agencyid)
        {
            ClientDTO client = new ClientDTO();
            try
            {
                client.agency.agencyid = new EncryptedData() { EncryptedValue = agencyid };
                if (!string.IsNullOrEmpty(refid))
                    client = _clientService.GetClientByUUID(refid);
                ViewBag.CurrencyDropdown = Dropdowns.GetCurrencyDropdown();
                ViewBag.AgenciesDropdown = Dropdowns.GetAgenciesDropdown(false);
            }
            catch (Exception ex)
            {
            }
            return PartialView("~/Views/Shared/Masters/_ModalClient.cshtml", client);
        }

        [HttpPost]
        public ActionResult SaveClient(ClientDTO client)
        {
            _clientService.SaveClient(client);
            return Json(_clientService.result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteClient(string clientid, string clientuuid)
        {
            _clientService.DeleteClient(Common.Decrypt(clientid), clientuuid);
            return Json(_clientService.result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string refid = "")
        {
            Client client = new Client();
            client.client = _clientService.GetClientByUUID(refid);
            client.client.Contacts = _clientService.GetContactList(client.client.clientid.EncryptedValue);
            client.client.Addresses = _clientService.GetAddressList(client.client.clientid.EncryptedValue);
            return View(client);
        }

        public ActionResult OpenClientKeyValuePair(string refid)
        {
            ClientDTO currentclient = SessionManager.CurrentClient;
            if (currentclient == null)
                currentclient = new ClientDTO();
            KeyValuePair pair = currentclient.CustomAttributes.Where(x => x.UUID == refid).FirstOrDefault();
            if (pair == null)
                pair = new KeyValuePair();
            return PartialView("~/Views/Shared/Masters/Agency/_ModalCustomKeyValue.cshtml", pair);
        }

        [HttpPost]
        public ActionResult UpdateClientCustomAttribute(KeyValuePair pair)
        {
            ClientDTO currentclient = SessionManager.CurrentClient;
            if (currentclient == null)
                currentclient = new ClientDTO();

            if (pair != null)
            {
                var found = currentclient.CustomAttributes.FirstOrDefault(x => x.UUID == pair.UUID);
                if (found != null)
                {
                    found.KeyName = pair.KeyName;
                    found.KeyValue = pair.KeyValue;
                }
                else
                {
                    currentclient.CustomAttributes.Add(pair);
                }
            }
            _clientService.SaveCustomAttribute(
                currentclient.CustomAttributes,
                Common.Decrypt(currentclient.clientid.EncryptedValue),
                currentclient.clientuuid
            );
            return PartialView(
                "~/Views/Shared/Masters/Client/_ClientCustomAtrributes.cshtml",
                new Client()
                {
                    client = new ClientDTO() { CustomAttributes = currentclient.CustomAttributes }
                }

            );
        }
        public ActionResult DeleteClientCustomAttribute(string refid)
        {
            ClientDTO currentclient = SessionManager.CurrentClient;
            if (currentclient == null)
                currentclient = new ClientDTO();

            currentclient.CustomAttributes.RemoveAll(x => x.UUID == refid);

            _clientService.SaveCustomAttribute(currentclient.CustomAttributes, currentclient.clientid.NumericValue, currentclient.clientuuid);
            return PartialView("~/Views/Shared/Masters/Client/_ClientCustomAtrributes.cshtml", new Client()
            {
                client = new ClientDTO
                {
                    CustomAttributes = currentclient.CustomAttributes
                }
            });
        }
        public ActionResult OpenClientBankAccount(string refid)
        {
            ClientDTO currentclient = SessionManager.CurrentClient;
            if (currentclient == null)
                currentclient = new ClientDTO();
            CustomBankInfo account = currentclient.BankInfo.Where(x => x.BankAccountUUID == refid).FirstOrDefault();
            if (account == null)
            {
                account = new CustomBankInfo();
            }
            account.FillBankKeyValues();
            ViewBag.CurrencyDropdown = Dropdowns.GetCurrencyDropdown();
            return PartialView("~/Views/Shared/Masters/Agency/_ModalBankAccount.cshtml", account);
        }

        public ActionResult GetClientContactModal(string refid, string clientid)
        {
            ClientContact contact = new ClientContact();
            contact.clientid.EncryptedValue = clientid;
            contact.picid.EncryptedValue = refid;
            if (refid != "")
                contact = _clientService.GetContactByID(refid);
            return PartialView("~/Views/Shared/Masters/Client/_ModalClientContact.cshtml", contact);
        }

        public ActionResult GetAddressModal(string refid, string clientid)
        {
            AddressDTO address = new AddressDTO();
            address.clientid.EncryptedValue = clientid;
            address.addressid.EncryptedValue = refid;
            if (refid != "")
                address = _clientService.GetAddressByID(refid);
            return PartialView("~/Views/Shared/Masters/Client/_ModalAddress.cshtml", address);
        }

        [HttpPost]
        public ActionResult SaveAddress(AddressDTO p_address)
        {
            _clientService.SaveAddress(p_address);
            return Json(_clientService.result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MakeAddressPrimary(string refid, string clientid)
        {
            AddressDTO address = new AddressDTO();
            address.addressid.EncryptedValue = refid;
            _clientService.MakePrimary(address);
            var list = _clientService.GetAddressList(clientid);
            return PartialView("~/Views/Master/Client/_ClientAddress.cshtml", new ClientDTO()
            {
                clientid = new EncryptedData()
                {
                    NumericValue = Common.Decrypt(clientid),
                    EncryptedValue = clientid
                },
                Addresses = list
            });
        }
        public ActionResult DeleteAddress(string refid, string clientid)
        {
            AddressDTO address = new AddressDTO();
            address.addressid.EncryptedValue = refid;
            _clientService.DeleteAddress(address);
            var list = _clientService.GetAddressList(clientid);
            return PartialView("~/Views/Master/Client/_ClientAddress.cshtml", new ClientDTO()
            {
                clientid = new EncryptedData()
                {
                    NumericValue = Common.Decrypt(clientid),
                    EncryptedValue = clientid
                },
                Addresses = list
            });
        }

        public ActionResult Logs(string refid = "")
        {
            ClientDTO client = new ClientDTO();
            client = _clientService.GetClientByUUID(refid);
            ClientLog log = new ClientLog();
            log.Info = client;
            log.Logs = _clientService.GetClientModificationList(refid);
            return View(log);
        }

        public ActionResult MakeContactPrimary(string refid, string clientid)
        {
            _clientService.MakePrimaryContact(refid);
            var list = _clientService.GetContactList(clientid);
            return PartialView("~/Views/Master/Client/_ClientContacts.cshtml", new ClientDTO()
            {
                clientid = new EncryptedData()
                {
                    NumericValue = Common.Decrypt(clientid),
                    EncryptedValue = clientid
                },
                Contacts = list
            });
        }

        [HttpPost]
        public ActionResult SaveContact(ClientContact contact)
        {
            _clientService.SaveContact(contact);
            return Json(_clientService.result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteContact(string refid, string clientid)
        {
            _clientService.DeleteContact(refid);
            var list = _clientService.GetContactList(clientid);
            return PartialView("~/Views/Master/Client/_ClientContacts.cshtml", new ClientDTO()
            {
                clientid = new EncryptedData()
                {
                    NumericValue = Common.Decrypt(clientid),
                    EncryptedValue = clientid
                },
                Contacts = list
            });
        }

        [HttpPost]
        public ActionResult UpdateClientBankAccount(CustomBankInfo bank)
        {
            ClientDTO currentclient = SessionManager.CurrentClient;
            if (currentclient == null)
                currentclient = new ClientDTO();

            if (bank != null)
            {
                bank.BankAttributes = bank.BankAttributes.Where(x => !string.IsNullOrEmpty(x.KeyName) && !string.IsNullOrEmpty(x.KeyValue)).ToList();
                if (bank.BankAttributes.Count == 0)
                {
                    currentclient.BankInfo.RemoveAll(x => x.BankAccountUUID == bank.BankAccountUUID);
                }
                else
                {
                    var found = currentclient.BankInfo.Where(x => x.BankAccountUUID == bank.BankAccountUUID).FirstOrDefault();
                    if (found != null)
                        currentclient.BankInfo.Where(x => x.BankAccountUUID == bank.BankAccountUUID).ToList().ForEach(x =>
                        {
                            x.AliasName = bank.AliasName;
                            x.Currency = bank.Currency;
                            x.BankAttributes = bank.BankAttributes;
                        });
                    else
                        currentclient.BankInfo.Add(bank);
                }
            }
            _clientService.SaveBankInfo(currentclient.BankInfo, currentclient.clientid.NumericValue, currentclient.clientuuid);
            return PartialView("~/Views/Shared/Masters/Client/_ClientBankAccounts.cshtml",
                new Client()
                {
                    client = new ClientDTO() { BankInfo = currentclient.BankInfo }
                });
        }
        public ActionResult DeleteClientBankAccount(string refid)
        {
            ClientDTO currentclient = SessionManager.CurrentClient;
            if (currentclient == null)
                currentclient = new ClientDTO();

            currentclient.BankInfo.RemoveAll(x => x.BankAccountUUID == refid);

            _clientService.SaveBankInfo(currentclient.BankInfo, currentclient.clientid.NumericValue, currentclient.clientuuid);
            return PartialView("~/Views/Shared/Masters/Client/_ClientBankAccounts.cshtml",
                new Client()
                {
                    client = new ClientDTO() { BankInfo = currentclient.BankInfo }
                });
        }

        [HttpGet]
        public ActionResult GetCustomers(string q)
        {
            var data = Dropdowns.GetCustomerSearch(q);
            var list = data
                    .Select(g => new
                    {
                        id = g.clientdetailid.EncryptedValue,
                        text = g.clientname,
                        address = g.address,
                        email = g.email,
                        phone = g.phone,
                        textcolor = g.extras.clientcolor,
                        bgcolor = g.extras.clientbgcolor,
                        shortcode = g.extras.clientshortcode,
                    }).ToList();

            return Json(new { results = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCustomersForPricing(string q, string pricinguuid)
        {
            var data = Dropdowns.GetCustomerSearchForPricing(pricinguuid, q);
            var list = data
                    .Select(g => new
                    {
                        id = g.clientdetailid.EncryptedValue,
                        text = g.clientname,
                        address = g.address,
                        email = g.email,
                        phone = g.phone,
                        textcolor = g.extras.clientcolor,
                        bgcolor = g.extras.clientbgcolor,
                        shortcode = g.extras.clientshortcode,
                    }).ToList();

            return Json(new { results = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClientWithEnquiryStats(string clientdetailid)
        {
            ViewBag.AgenciesDropdown = Dropdowns.GetAgenciesDetailIDDropdown();
            ViewBag.ModeDropdown = Dropdowns.GetTransferTypeDropdown(true);
            ViewBag.FullOREmptyDropdown = Dropdowns.GetFullOREmptyDropdown(true);
            BookingCustomerDTO info = _clientService.GetCustomerBookingWithAnalytics(Common.Decrypt(clientdetailid));
            ContainerBooking booking = new ContainerBooking();
            booking.booking.customer = info;

            SessionManager.Booking = SessionManager.Booking ?? new ContainerBooking();
            SessionManager.Booking.booking.customer = info;

            return PartialView("~/Views/Booking/_CustomerInfo.cshtml", booking);
        }

        public ActionResult GetShipperConsigneeInfo(string clientdetailid, int type)
        {
            if (type != 1 && type != 2)
                return Json(Common.ErrorMessage("Invalid Type"), JsonRequestBehavior.AllowGet);

            if (string.IsNullOrEmpty(clientdetailid))
                return Json(Common.ErrorMessage("Invalid Client"), JsonRequestBehavior.AllowGet);

            ContainerBooking booking = SessionManager.Booking ?? new ContainerBooking();
            ClientDTO info = _clientService.GetClientByDetailID(Common.Decrypt(clientdetailid));
            if (info.clientid.NumericValue > 0)
            {
                var detail = new EncryptedData
                {
                    EncryptedValue = info.clientdetailid.EncryptedValue,
                    NumericValue = info.clientdetailid.NumericValue
                };
                var loc = booking.booking.location;

                if (type == 1) // Shipper
                {
                    loc.shipperdetailid = detail;
                    loc.shippername = info.clientname;
                    loc.shipperemail = info.email;
                    loc.shipperphone = info.phone;
                    loc.shipperaddress = info.address;
                    ViewBag.ShipperPICDropdown = Dropdowns.GetPICByDetailIDDropdown(clientdetailid);
                }
                else if (type == 2) // Consignee
                {
                    loc.consigneedetailid = detail;
                    loc.consigneename = info.clientname;
                    loc.consigneeemail = info.email;
                    loc.consigneephone = info.phone;
                    loc.consigneeaddress = info.address;
                    ViewBag.ConsigneePICDropdown = Dropdowns.GetPICByDetailIDDropdown(clientdetailid);
                }
            }
            SessionManager.Booking = booking;
            return PartialView(type == 1 ? "~/Views/Booking/_Shipper.cshtml" : "~/Views/Booking/_Consignee.cshtml", booking);
        }
    }
}