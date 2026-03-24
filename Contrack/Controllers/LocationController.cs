using Contrack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Contrack.Controllers
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    [AuthorizeRoles(LoginType.Hub, Order = 4)]
    public class LocationController : Controller
    {
        private readonly LocationService _locationService;
        public LocationController()
        {
            ILocationRepository repo = new LocationRepository();
            _locationService = new LocationService(repo);
        }
        public ActionResult List()
        {
            LocationListFilter currentFilter = SessionManager.LocationListFilter ?? new LocationListFilter();
            LocationDropdowns();
            return View(currentFilter);
        }
        private void LocationDropdowns()
        {
            ViewBag.CountryDropdown = Dropdowns.GetCountryDropdown();
            ViewBag.PortDropdown = Dropdowns.GetPortDropdown();
            ViewBag.LocationTypeDropdown = Dropdowns.GetLocationTypeDropdown();
        }
        public ActionResult GetLocationModal(string refid = "")
        {
            LocationDTO locationDto = new LocationDTO();
            if (!string.IsNullOrEmpty(refid))
            {
                locationDto = _locationService.GetLocationByUUID(refid);
            }
            var countries = Dropdowns.GetCountryDropdown();
            countries = Common.AlterDropdown(countries, locationDto.CountryID.EncryptedValue, locationDto.CountryName);
            ViewBag.CountryDropdown = countries;
            var locationTypes = Dropdowns.GetLocationTypeDropdown();
            locationTypes = Common.AlterDropdown(locationTypes, locationDto.LocationType.LocationTypeID.EncryptedValue, locationDto.LocationType.LocationTypeName);
            ViewBag.LocationTypeDropdown = locationTypes;
            var ports = Dropdowns.EmptyDropdown();
            ports = Common.AlterDropdown(ports, locationDto.PortID.EncryptedValue, locationDto.PortName);
            ViewBag.PortDropdown = ports;

            return PartialView("~/Views/Location/_ModalLocation.cshtml", new Location { location = locationDto });
        }
        [HttpGet]
        public ActionResult GetPorts(string q)
        {
            var data = Dropdowns.GetPortSearch(q);
            return Json(new { results = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveLocation(Location model)
        {
            _locationService.SaveLocation(model);
            return Json(_locationService.result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DisableLocation(Location loc)
        {
            _locationService.DisableLocation(Common.Decrypt(loc.location.LocationID.EncryptedValue));
            return Json(_locationService.result, JsonRequestBehavior.AllowGet);
        }
    }
}