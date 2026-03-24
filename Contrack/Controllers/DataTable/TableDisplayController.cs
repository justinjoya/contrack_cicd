using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Contrack.Controllers
{
    public class JDataTable
    {
        public int page { get; set; }
        public int pageCount { get; set; }
        public string sortField { get; set; }
        public string sortOrder { get; set; }
        public int totalCount { get; set; }
        public dynamic data { get; set; }
        public JDataTable()
        {
        }
        public JDataTable(int _page, string _sortOrder, string _sortField)
        {
            page = _page;
            sortField = _sortField;
            sortOrder = _sortOrder;
        }
    }

    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    public class TableDisplayController : TableDisplayBaseController
    {
        public ActionResult Agencies(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                List<Agency> agencylist = _agencyService.GetAgencyList(search);
                switch (sortField)
                {
                    case "createdat":
                        agencylist.Sort((x, y) => SortDateTime(x.agency.hcreatedat.ToString("yyyy-MM-dd hh:mm:ss"), y.agency.hcreatedat.ToString("yyyy-MM-dd  hh:mm:ss"), sortOrder));
                        break;
                    case "agencyname":
                        agencylist.Sort((x, y) => SortString(x.agency.agencyname, y.agency.agencyname, sortOrder));
                        break;
                    case "imono":
                        agencylist.Sort((x, y) => SortString(x.agency.imono, y.agency.imono, sortOrder));
                        break;
                    case "email":
                        agencylist.Sort((x, y) => SortString(x.agency.email, y.agency.email, sortOrder));
                        break;
                    case "phone":
                        agencylist.Sort((x, y) => SortString(x.agency.phone, y.agency.phone, sortOrder));
                        break;
                    default:
                        break;
                }

                dataTableData.totalCount = agencylist.Count;
                dataTableData.pageCount = agencylist.Count > 0 ? Convert.ToInt32(size / agencylist.Count) : 0;

                agencylist = agencylist.Skip((page - 1) * size).Take(size).ToList();
                dataTableData.data = agencylist;
            }
            catch (Exception)
            { }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Clients(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                ClientListFilter filter = SessionManager.ClientListFilter;
                if (filter == null)
                    filter = new ClientListFilter();

                filter.offset = (page - 1) * size;
                filter.limit = size;
                filter.Search = search;
                filter.sorting = string.IsNullOrEmpty(sortField) ? "" : sortField;
                filter.sortingorder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
                List<Client> clientlist = _clientService.GetClientList(filter);

                int totalcount = 10;
                if (clientlist.Count > 0)
                {
                    totalcount = clientlist[0].client.totalnoofrows;
                }

                dataTableData.totalCount = totalcount;
                dataTableData.pageCount = totalcount > 0 ? Convert.ToInt32(size / totalcount) : 0;

                dataTableData.data = clientlist;
            }
            catch (Exception)
            { }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UoM(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                List<UoMDTO> list = _uomService.GetUoMList(search);

                switch (sortField)
                {
                    case "uomname":
                        list.Sort((x, y) => SortString(x.uomname, y.uomname, sortOrder));
                        break;
                    default:
                        break;
                }

                // Pagination
                dataTableData.totalCount = list.Count;
                dataTableData.pageCount = (int)Math.Ceiling((double)list.Count / size);// list.Count > 0 ? Convert.ToInt32(size / list.Count) : 0;
                list = list.Skip((page - 1) * size).Take(size).ToList();

                dataTableData.data = list;
            }
            catch (Exception ex)
            {
            }

            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult JobTypes(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                List<JobType> tacList = _jobTypeService.GetJobTypeList(search);

                switch (sortField)
                {
                    case "jobtypename":
                        tacList.Sort((x, y) => SortString(x.jobtype.jobtypename, y.jobtype.jobtypename, sortOrder));
                        break;
                    case "useasmaster":
                        tacList.Sort((x, y) => SortString(x.jobtype.useasmaster.ToString(), y.jobtype.useasmaster.ToString(), sortOrder));
                        break;
                    case "lineitemcount":
                        tacList.Sort((x, y) => SortInteger(x.jobtype.lineitemcount, y.jobtype.lineitemcount, sortOrder));
                        break;
                    default:
                        break;
                }

                dataTableData.totalCount = tacList.Count;
                dataTableData.pageCount = tacList.Count > 0 ? Convert.ToInt32(Math.Ceiling((double)tacList.Count / size)) : 0;

                tacList = tacList.Skip((page - 1) * size).Take(size).ToList();
                dataTableData.data = tacList;
            }
            catch (Exception) { }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Vessels(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                VesselFilter filter = SessionManager.VesselListFilter ?? new VesselFilter();

                filter.offset = (page - 1) * size;
                filter.limit = size;
                filter.Search = search;
                filter.sorting = sortField ?? "";
                filter.sortingorder = sortOrder ?? "";

                // Fetch vessel list
                List<Vessel> vesselList = _vesselService.GetVesselList(filter);

                int totalcount = vesselList.FirstOrDefault()?.vessel.totalnoofrows ?? 0;

                dataTableData.totalCount = totalcount;
                dataTableData.pageCount = totalcount > 0 ? (int)Math.Ceiling((double)totalcount / size) : 0;
                dataTableData.data = vesselList;
            }
            catch (Exception ex)
            {
            }

            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Vendors(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                VendorFilter filter = SessionManager.VendorListFilter; // ✅

                if (filter == null)
                    filter = new VendorFilter();

                filter.offset = (page - 1) * size;
                filter.limit = size;
                filter.Search = search;
                filter.sorting = string.IsNullOrEmpty(sortField) ? "" : sortField;
                filter.sortingorder = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
                List<Vendor> vendorlist = _vendorService.GetVendorList(filter);

                int totalcount = 10;
                if (vendorlist.Count > 0)
                {
                    totalcount = vendorlist[0].vendor.totalnoofrows;
                }

                dataTableData.totalCount = totalcount;
                dataTableData.pageCount = totalcount > 0 ? Convert.ToInt32(size / totalcount) : 0;

                dataTableData.data = vendorlist;
            }
            catch (Exception)
            { }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Users(int page, int size, string sortOrder, string sortField, string search)
        {
            var dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                UserFilter filter = SessionManager.UserListFilter ?? new UserFilter();

                filter.offset = (page - 1) * size;
                filter.limit = size;
                filter.Search = search;
                filter.sorting = string.IsNullOrEmpty(sortField) ? "createdat" : sortField;
                filter.sortingorder = string.IsNullOrEmpty(sortOrder) ? "desc" : sortOrder;

                var userlist = _loginService.GetUserLoginList(filter);

                int totalcount = 0;
                if (userlist.Count > 0)
                {
                    totalcount = userlist[0].user.totalnoofrows;
                }

                dataTableData.totalCount = totalcount;
                dataTableData.pageCount = (totalcount > 0 && size > 0) ? (int)Math.Ceiling((double)totalcount / size) : 0;
                dataTableData.data = userlist;
            }
            catch (Exception ex)
            {
                // Handle or log exception
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Locations(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                LocationListFilter filter = SessionManager.LocationListFilter ?? new LocationListFilter();
                filter.SearchText = search;
                var locationList = _locationService.GetLocationList(filter);

                if (!string.IsNullOrEmpty(sortField))
                {
                    switch (sortField.ToLower())
                    {
                        case "portname":
                            locationList.Sort((x, y) => SortString(x.location.PortName, y.location.PortName, sortOrder));
                            break;
                        case "locationname":
                            locationList.Sort((x, y) => SortString(x.location.LocationName, y.location.LocationName, sortOrder));
                            break;
                        case "countryname":
                            locationList.Sort((x, y) => SortString(x.location.CountryName, y.location.CountryName, sortOrder));
                            break;
                        case "available":
                            locationList.Sort((x, y) => SortInteger(x.location.AvailableCount, y.location.AvailableCount, sortOrder));
                            break;
                        case "booked":
                            locationList.Sort((x, y) => SortInteger(x.location.BookedCount, y.location.BookedCount, sortOrder));
                            break;
                        case "repair":
                            locationList.Sort((x, y) => SortInteger(x.location.DamagedCount, y.location.DamagedCount, sortOrder));
                            break;
                        case "total":
                            locationList.Sort((x, y) => SortInteger(x.location.TotalCount, y.location.TotalCount, sortOrder));
                            break;
                        default:
                            locationList.Sort((x, y) => SortInteger(x.location.TotalCount, y.location.TotalCount, sortOrder));
                            break;
                    }
                }

                dataTableData.totalCount = locationList.Count;
                dataTableData.pageCount = locationList.Count > 0 ? (int)Math.Ceiling((double)locationList.Count / size) : 0;
                locationList = locationList.Skip((page - 1) * size).Take(size).ToList();

                dataTableData.data = locationList;
            }
            catch (Exception ex)
            {
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SmtpConfigurations(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                List<SmtpConfig> list = _smtpConfigService.GetSmtpConfigList(search);
                switch (sortField)
                {
                    case "created_for":
                        list.Sort((x, y) => SortString(x.smtpconfig.agency.agencyname, y.smtpconfig.agency.agencyname, sortOrder));
                        break;
                    case "created_at":
                        list.Sort((x, y) => SortDateTime(x.smtpconfig.created_at.ToString("yyyy-MM-dd hh:mm:ss"), y.smtpconfig.created_at.ToString("yyyy-MM-dd  hh:mm:ss"), sortOrder));
                        break;
                    case "smtp_port":
                        list.Sort((x, y) => SortInteger(x.smtpconfig.smtp_port, y.smtpconfig.smtp_port, sortOrder));
                        break;
                    case "smtp_username":
                        list.Sort((x, y) => SortString(x.smtpconfig.smtp_username, y.smtpconfig.smtp_username, sortOrder));
                        break;
                    case "smtp_host":
                        list.Sort((x, y) => SortString(x.smtpconfig.smtp_host, y.smtpconfig.smtp_host, sortOrder));
                        break;
                    default:
                        break;
                }

                dataTableData.totalCount = list.Count;
                dataTableData.pageCount = list.Count > 0 ? Convert.ToInt32(size / list.Count) : 0;

                list = list.Skip((page - 1) * size).Take(size).ToList();
                dataTableData.data = list;
            }
            catch (Exception)
            { }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Voyage(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                VoyageFilter filter = SessionManager.VoyageListFilter;
                if (filter == null)
                    filter = new VoyageFilter();
                filter.startindex = (page - 1) * size;
                filter.noofrows = size;
                filter.searchstr = search ?? "";
                filter.sortby = string.IsNullOrEmpty(sortField) || sortField == "null" ? "" : sortField;
                filter.sortdirection = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
                List<Voyage> list = _voyageService.GetVoyageList(filter);
                int totalCount = 0;
                if (list != null && list.Count > 0)
                {
                    totalCount = list[0].VoyageDTO.totalnoofrows;
                }
                dataTableData.totalCount = totalCount;
                dataTableData.pageCount = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / size) : 0;
                dataTableData.data = list;
            }
            catch (Exception ex)
            {
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ContainerModels(int page, int size, string sortOrder, string sortField, string search = "")
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                ContainerModelFilter filter = SessionManager.ContainerModelFilter ?? new ContainerModelFilter();
                filter.searchstr = search;

                var modelList = _containermodelservice.GetContainerModelList(filter);

                if (!string.IsNullOrEmpty(sortField))
                {
                    switch (sortField.ToLower())
                    {
                        case "typename":
                            modelList.Sort((x, y) => SortString(x.model.typename, y.model.typename, sortOrder));
                            break;

                        case "sizename":
                            modelList.Sort((x, y) => SortString(x.model.sizename, y.model.sizename, sortOrder));
                            break;

                        case "iso_code":
                            modelList.Sort((x, y) => SortString(x.model.iso_code, y.model.iso_code, sortOrder));
                            break;

                        case "length":
                            modelList.Sort((x, y) => SortString(x.model.length, y.model.length, sortOrder));
                            break;

                        case "width":
                            modelList.Sort((x, y) => SortString(x.model.width, y.model.width, sortOrder));
                            break;

                        case "height":
                            modelList.Sort((x, y) => SortString(x.model.height, y.model.height, sortOrder));
                            break;

                        case "available":
                            modelList.Sort((x, y) => SortInteger(x.model.AvailableCount, y.model.AvailableCount, sortOrder));
                            break;

                        case "booked":
                            modelList.Sort((x, y) => SortInteger(x.model.BookedCount, y.model.BookedCount, sortOrder));
                            break;

                        case "repair":
                            modelList.Sort((x, y) => SortInteger(x.model.RepairCount, y.model.RepairCount, sortOrder));
                            break;

                        case "total":
                            modelList.Sort((x, y) => SortInteger(x.model.TotalCount, y.model.TotalCount, sortOrder));
                            break;
                        default:
                            modelList.Sort((x, y) => SortInteger(x.model.TotalCount, y.model.TotalCount, sortOrder));
                            break;
                    }
                }
                dataTableData.totalCount = modelList.Count;
                dataTableData.pageCount = modelList.Count > 0 ? (int)Math.Ceiling((double)modelList.Count / size) : 0;
                modelList = modelList.Skip((page - 1) * size).Take(size).ToList();
                dataTableData.data = modelList;
            }
            catch (Exception ex)
            { }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Containers(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                ContainerFilterPage filter = SessionManager.ContainerListFilter;

                if (filter == null)
                    filter = new ContainerFilterPage();
                filter.startindex = (page - 1) * size;
                filter.noofrows = size;
                filter.searchstr = search ?? "";

                filter.sortby = (string.IsNullOrEmpty(sortField) || sortField == "null")
                    ? "containercreatedat"
                    : sortField;

                filter.sortdirection = string.IsNullOrEmpty(sortOrder)
                    ? "DESC"
                    : sortOrder;
                List<ContainerDTO> containerList = _containerService.GetContainerList(filter);

                int totalcount = 0;
                if (containerList.Count > 0)
                {
                    totalcount = containerList[0].rowcount.totalnoofrows;
                }

                dataTableData.totalCount = totalcount;
                dataTableData.pageCount = (totalcount > 0 && size > 0)
                    ? (int)Math.Ceiling((double)totalcount / size)
                    : 0;

                dataTableData.data = containerList;
            }
            catch (Exception ex)
            {
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BookingLists(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                BookingListFilter filter = SessionManager.BookingListFilter;
                if (filter == null)
                    filter = new BookingListFilter();

                filter.startindex = (page - 1) * size;
                filter.noofrows = size;
                filter.searchstr = search;
                filter.sortby = string.IsNullOrEmpty(sortField) ? "" : sortField;
                filter.sortdirection = string.IsNullOrEmpty(sortOrder) ? "" : sortOrder;
                List<BookingList> bookinglist = _bookingService.GetbookingList(filter);

                int totalcount = 10;
                if (bookinglist.Count > 0)
                {
                    totalcount = bookinglist[0].containerbookinglist.total_count;
                }

                dataTableData.totalCount = totalcount;
                dataTableData.pageCount = totalcount > 0 ? (int)Math.Ceiling((double)size / totalcount) : 0;

                dataTableData.data = bookinglist;
            }
            catch (Exception)
            { }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult QuotationLists(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);

            try
            {
                QuotationListFilter filter = SessionManager.QuotationListFilter ?? new QuotationListFilter();

                filter.startindex = (page - 1) * size;
                filter.noofrows = size;
                filter.searchstr = search ?? "";
                filter.sortby = string.IsNullOrEmpty(sortField) ? "createdat" : sortField;
                filter.sortdirection = string.IsNullOrEmpty(sortOrder) ? "DESC" : sortOrder;

                List<Quotationlist> quotationList = _bookingService.GetQuotationList(filter);

                int totalcount = 0;
                if (quotationList != null && quotationList.Count > 0)
                {
                    totalcount = quotationList[0].QuotationLists.total_count;
                }

                dataTableData.totalCount = totalcount;

                dataTableData.pageCount = totalcount > 0
                    ? (int)Math.Ceiling((double)totalcount / size)
                    : 0;

                dataTableData.data = quotationList;
            }
            catch (Exception ex)
            {
            }

            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TermsAndConditions(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                List<TermsandConditions> termsandconditionslist = _TermsandConditionsService.GetTermsAndConditionsList(search);

                switch (sortField)
                {
                    case "created_for":
                        termsandconditionslist.Sort((x, y) => SortString(x.termsandConditions.agency.agencyname, y.termsandConditions.agency.agencyname, sortOrder));
                        break;
                    case "created_at":
                        termsandconditionslist.Sort((x, y) => SortDateTime(x.termsandConditions.CreatedAt.ToString("yyyy-MM-dd hh:mm:ss"), y.termsandConditions.CreatedAt.ToString("yyyy-MM-dd  hh:mm:ss"), sortOrder));
                        break;
                    case "type":
                        termsandconditionslist.Sort((x, y) => SortString(x.termsandConditions.Type, y.termsandConditions.Type, sortOrder));
                        break;
                    case "created_by":
                        termsandconditionslist.Sort((x, y) => SortString(x.termsandConditions.CreatedBy.ToString(), y.termsandConditions.CreatedBy.ToString(), sortOrder));
                        break;
                    default:
                        break;
                }

                dataTableData.totalCount = termsandconditionslist.Count;
                dataTableData.pageCount = termsandconditionslist.Count > 0 ? Convert.ToInt32(size / termsandconditionslist.Count) : 0;

                termsandconditionslist = termsandconditionslist.Skip((page - 1) * size).Take(size).ToList();
                dataTableData.data = termsandconditionslist;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in TermsAndConditions: " + ex.Message);
            }

            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Trackings(int page, int size, string sortOrder, string sortField, string search, string containeruuid = "")
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                TrackingFilterPage filter = SessionManager.TrackingListFilter ?? new TrackingFilterPage();
                filter.startindex = (page - 1) * size;
                filter.noofrows = size;
                filter.searchstr = search ?? "";
                filter.sortby = (string.IsNullOrEmpty(sortField) || sortField == "null") ? "recorddatetime" : sortField;
                filter.sortdirection = string.IsNullOrEmpty(sortOrder) ? "DESC" : sortOrder;
                if (!string.IsNullOrEmpty(containeruuid))
                    filter.ContainerUUID = containeruuid;
                List<TrackingListDTO> list = _trackingService.GetTrackingList(filter);
                int totalcount = (list != null && list.Count > 0) ? list[0].rowcount.totalnoofrows : 0;
                dataTableData.totalCount = totalcount;
                dataTableData.pageCount = (totalcount > 0 && size > 0) ? (int)Math.Ceiling((double)totalcount / size) : 0;
                dataTableData.data = list;
            }
            catch (Exception ex)
            {
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Pricings(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                PricingListFilter filter = SessionManager.PricingListFilter ?? new PricingListFilter();
                filter.startindex = (page - 1) * size;
                filter.noofrows = size;
                filter.searchstr = search ?? "";
                filter.sortby = (string.IsNullOrEmpty(sortField) || sortField == "null") ? "createdat" : sortField;
                filter.sortdirection = string.IsNullOrEmpty(sortOrder) ? "DESC" : sortOrder;
                List<PricingHeaderDTO> list = _pricingService.GetPricingList(filter);
                int totalcount = (list != null && list.Count > 0) ? list[0].RowCount.totalnoofrows : 0;
                dataTableData.totalCount = totalcount;
                dataTableData.pageCount = (totalcount > 0 && size > 0) ? (int)Math.Ceiling((double)totalcount / size) : 0;
                dataTableData.data = list;
            }
            catch (Exception ex)
            {
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PurchaseOrderList(int page, int size, string sortOrder, string sortField, string search)
        {
            JDataTable dataTableData = new JDataTable(page, sortOrder, sortField);
            try
            {
                PurchaseOrderFilterPage filter = SessionManager.PurchaseOrderFilter ?? new PurchaseOrderFilterPage();
                if (filter.filters == null) filter.filters = new PurchaseOrderFilter();
                filter.startindex = (page - 1) * size;
                filter.noofrows = size;
                filter.searchstr = search;
                filter.sortby = (string.IsNullOrEmpty(sortField) || sortField == "null") ? "createdat" : sortField;
                filter.sortdirection = string.IsNullOrEmpty(sortOrder) ? "DESC" : sortOrder;
                List<PurchaseOrderListDTO> vilist = _poService.GetPurchaseOrderList(filter);
                int totalcount = vilist.Count > 0 ? vilist[0].rowcount.totalnoofrows : 0;
                dataTableData.totalCount = totalcount;
                dataTableData.pageCount = totalcount > 0 ? (int)Math.Ceiling((double)totalcount / size) : 0;
                dataTableData.data = vilist;
            }
            catch (Exception ex)
            {
            }
            return Json(dataTableData, JsonRequestBehavior.AllowGet);
        }


        private int SortString(string s1, string s2, string sortDirection)
        {
            return sortDirection == "asc" ? s1.CompareTo(s2) : s2.CompareTo(s1);
        }

        private int SortInteger(string s1, string s2, string sortDirection)
        {
            int i1 = int.Parse(s1);
            int i2 = int.Parse(s2);
            return sortDirection == "asc" ? i1.CompareTo(i2) : i2.CompareTo(i1);
        }
        private int SortInteger(int s1, int s2, string sortDirection)
        {
            int i1 = s1;
            int i2 = s2;
            return sortDirection == "asc" ? i1.CompareTo(i2) : i2.CompareTo(i1);
        }
        private int SortDecimal(string s1, string s2, string sortDirection)
        {
            decimal i1 = decimal.Parse(s1);
            decimal i2 = decimal.Parse(s2);
            return sortDirection == "asc" ? i1.CompareTo(i2) : i2.CompareTo(i1);
        }

        private int SortDateTime(string s1, string s2, string sortDirection)
        {
            //s1 = string.IsNullOrEmpty(s1) ? DateTime.MinValue.ToString(Constants.DateFormat) : s1;
            //s2 = string.IsNullOrEmpty(s2) ? DateTime.MinValue.ToString(Constants.DateFormat) : s2;

            DateTime d1 = DateTime.Parse(s1, CultureInfo.InvariantCulture); //Common.ToDateTime(s1, 1);//
            DateTime d2 = DateTime.Parse(s2, CultureInfo.InvariantCulture); // Common.ToDateTime(s2, 1);//
            return sortDirection == "asc" ? d1.CompareTo(d2) : d2.CompareTo(d1);
        }

    }
}