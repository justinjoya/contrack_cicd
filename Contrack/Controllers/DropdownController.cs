using System.Web.Mvc;
using System.Linq;
using Contrack;

namespace Contrack.Controllers
{
    public class DropdownController : Controller
    {
        public ActionResult GetVendorDropdown(string AgencyDetailID = "", string name = "vendordetailid")
        {
            DropdownModal modal = new DropdownModal();
            ViewBag.SupplierDropdown = Dropdowns.GetVendorDropdown(AgencyDetailID);
            modal.Name = name + ".EncryptedValue";
            return PartialView("~/Views/Shared/Dropdowns/_VendorDropdown.cshtml", modal);
        }
        public ActionResult GetPortDropdown(string AgencyDetailID = "", string name = "PI.Port")
        {
            DropdownModal modal = new DropdownModal();
            ViewBag.PortDropdown = Dropdowns.GetPortDropdown(AgencyDetailID);
            modal.Name = name + ".EncryptedValue";
            return PartialView("~/Views/Shared/Dropdowns/_PortDropdown.cshtml", modal);
        }
        public ActionResult GetClientDropdown(string AgencyDetailID = "", string name = "PI.ClientDetailID")
        {
            DropdownModal modal = new DropdownModal();
            ViewBag.ClientDropdown = Dropdowns.GetClientsDropdown(AgencyDetailID);
            modal.Name = name + ".EncryptedValue";
            return PartialView("~/Views/Shared/Dropdowns/_ClientDropdown.cshtml", modal);
        }
    }
}