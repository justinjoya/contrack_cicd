using Contrack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Contrack.Controllers
{
    public class DownloadController : Controller
    {
        private readonly BookingService _service;
        private readonly TermsandConditionsService _termsandconditions;
        private readonly VoyageService _voyageService;

        public DownloadController()
        {
            ITermsandConditionsRepository tcrepo = new TermsandConditionsRepository();
            _termsandconditions = new TermsandConditionsService(tcrepo);
            IVoyageRepository VoyageRepository = new VoyageRepository();
            _voyageService = new VoyageService(VoyageRepository);
            IBookingRepository repo = new BookingRepository();
            _service = new BookingService(repo, tcrepo, VoyageRepository);
        }

        [AllowAnonymous]
        public ActionResult BookingQuotation(string quotationuuid)
        {
            BookingQuotation quote = new BookingQuotation();
            quote = _service.GetQuotationByUUID(quotationuuid);
            return View(quote);
        }

        [AllowAnonymous]
        public ActionResult BookingSummary(string refid)
        {
            ContainerBooking model = new ContainerBooking();
            model = _service.GetbookingByUUID(refid);
            model.bookingSummary = _service.GetBookingSummaryInfo(refid);
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Cabotage(string refid)
        {
            BookedContainer model = new BookedContainer();
            model = _service.GetCabotage(refid);
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ShipmentConfirmation(string refid)
        {
            BookedContainer model = _service.GetBookedContainers(refid, new BookedContainerFilter() { }, true);
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult PDFDownload(string refid, string type, string code, string Download)
        {
            string URLmain = "";
            string agencyname = "";
            switch (type)
            {
                case "quote":
                    URLmain = Url.Action("BookingQuotation", "Download", new { quotationuuid = refid, HubID = Common.Encrypt(Common.HubID) }).Replace("\\", "/");
                    break;
                case "bookingsummary":
                    URLmain = Url.Action("BookingSummary", "Download", new { refid = refid, HubID = Common.Encrypt(Common.HubID) }).Replace("\\", "/");
                    break;
                case "cabotage":
                    URLmain = Url.Action("Cabotage", "Download", new { refid = refid, HubID = Common.Encrypt(Common.HubID) }).Replace("\\", "/");
                    break;
                case "shipmentconfirm":
                    URLmain = Url.Action("ShipmentConfirmation", "Download", new { refid = refid, HubID = Common.Encrypt(Common.HubID) }).Replace("\\", "/");
                    break;
                default:
                    break;
            }

            if (URLmain != "")
            {
                string downloadfilename = "";
                byte[] bytespdf = null;
                try
                {
                    string URL = Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
                    string outpath = Common.DocumentsFolder;
                    URL = URL + "/" + URLmain;
                    string outfilename = code + "_" + Guid.NewGuid() + ".pdf";
                    downloadfilename = code + ".pdf";

                    if (!Directory.Exists(outpath))
                        Directory.CreateDirectory(outpath);
                    outpath = outpath + "\\" + outfilename;

                    Html2PDFConvertor.HTMLToPdf(URL, outpath, "", true, agencyname);

                    if (System.IO.File.Exists(outpath))
                        bytespdf = System.IO.File.ReadAllBytes(outpath);

                }
                catch (Exception ex)
                { }

                if (Download == "1" && bytespdf != null)
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + downloadfilename);

                if (bytespdf == null)
                    return Content("Cannot download file");
                else
                    return File(bytespdf, "application/pdf");
            }
            else
                return Content("Invalid download type");
        }
    }
}