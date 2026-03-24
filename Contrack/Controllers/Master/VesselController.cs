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
    public class VesselController : Controller
    {
        private readonly VesselService _vesselService;
        public VesselController()
        {
            IVesselRepository repo = new VesselRepository();
            _vesselService = new VesselService(repo);
        }

        public ActionResult List()
        {
            VesselFilter filter = new VesselFilter();
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
            SessionManager.VesselListFilter = filter;
            return View();
        }

        public ActionResult GetVesselModal(string refid = "", string agencyid = "")
        {
            try
            {
                VesselDTO vessel = new VesselDTO();
                vessel.vassignment.agencyid = new EncryptedData() { EncryptedValue = agencyid };
                if (!string.IsNullOrEmpty(refid))
                    vessel = _vesselService.GetVesselByUUID(refid);

                ViewBag.AgenciesDropdown = Dropdowns.GetAgenciesDropdown();
                ViewBag.CountryDropdown = Dropdowns.GetCountryDropdown(true);
                ViewBag.PortDropdown = Dropdowns.GetPortDropdown("", true);
                ViewBag.VesselTypeDropdown = Dropdowns.GetVesselTypeDropdown();
                ViewBag.VesselSubTypeDropdown = Dropdowns.GetVesselSubTypeDropdown();

                return PartialView("~/Views/Shared/Masters/_ModalVessel.cshtml", vessel);
            }
            catch (Exception ex)
            {
                var result = Common.ErrorMessage("Unable to load vessel modal. Error: " + ex.Message);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult SaveVessel(VesselDTO model)
        {
            _vesselService.SaveVessel(model);
            Result result = _vesselService.result;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteVessel(VesselAssignmentDTO model)
        {
            _vesselService.DeleteVessel(model);
            Result result = _vesselService.result;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ValidateVessel(string vesselname, string imono, string mmsino)
        {
            try
            {
                var vessels = _vesselService.ValidateVessel(vesselname, imono, mmsino);
                if (vessels.Count == 0)
                {
                    return Json(Common.SuccessMessage("Available"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return PartialView("~/Views/Shared/Masters/Vessel/_ExistingVesselList.cshtml", vessels);
                }
            }
            catch (Exception ex)
            {
                return Json(Common.ErrorMessage("Error validating vessel: " + ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult MoveVesselToHub(VesselDTO vessel)
        {
            var result = _vesselService.MoveVesselToHub(vessel.vassignment);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string refid = "")
        {
            var vessel = _vesselService.GetVesselByUUID(refid);
            vessel.Contacts = _vesselService.GetContacts(vessel.vassignment.vesselassignmentid.EncryptedValue);
            return View(vessel);
        }


        public ActionResult GetVesselContactModal(string refid, string vassignmentid)
        {
            VesselContactDTO contact = new VesselContactDTO();
            contact.vesselassignmentid = new EncryptedData()
            {
                EncryptedValue = vassignmentid
            };
            if (!string.IsNullOrEmpty(refid))
                contact = _vesselService.GetContactById(refid);
            return PartialView("~/Views/Shared/Masters/Vessel/_ModalVesselContact.cshtml", contact);
        }

        [HttpPost]
        public JsonResult SaveContact(VesselContactDTO contact)
        {
            var result = _vesselService.SaveContact(contact);
            return Json(result);
        }

        public ActionResult MakePrimary(string refid, string vassignmentid)
        {
            _vesselService.MakePrimary(refid);
            var list = _vesselService.GetContacts(vassignmentid);
            return PartialView("~/Views/Master/Vessel/_VesselContacts.cshtml", new VesselDTO()
            {
                vassignment = new VesselAssignmentDTO()
                {
                    vesselassignmentid = new EncryptedData()
                    {
                        EncryptedValue = vassignmentid
                    }
                },
                Contacts = list
            });
        }

        public ActionResult DeleteContact(string refid, string vassignmentid)
        {
            _vesselService.DeleteContact(refid);
            var list = _vesselService.GetContacts(vassignmentid);
            return PartialView("~/Views/Master/Vessel/_VesselContacts.cshtml", new VesselDTO()
            {
                vassignment = new VesselAssignmentDTO()
                {
                    vesselassignmentid = new EncryptedData()
                    {
                        EncryptedValue = vassignmentid
                    }
                },
                Contacts = list
            });
        }

        public JsonResult GetVesselDropdownList(string q, string AgencyDetailID = "", string multiple = "")
        {
            List<System.Web.UI.WebControls.ListItem> results = new List<System.Web.UI.WebControls.ListItem>();
            if (AgencyDetailID == "" || AgencyDetailID == "0")
            {
                results = Dropdowns.GetVesselDropdownSearch(q, multiple);
            }
            else
            {
                results = Dropdowns.GetVesselDropdown(AgencyDetailID, q, multiple);
            }
            var json = (from System.Web.UI.WebControls.ListItem dr in results.ToList()
                        select new dropdown()
                        {
                            id =  dr.Value,
                            text = dr.Text
                        }).ToList();
            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}
