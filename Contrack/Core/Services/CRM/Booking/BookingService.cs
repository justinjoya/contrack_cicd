using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using NSAX.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Services.Description;
using static System.Collections.Specialized.BitVector32;

namespace Contrack
{
    public class BookingService : CustomException, IBookingService
    {
        public Result result = new Result();
        private readonly IBookingRepository _repo;
        private readonly ITermsandConditionsRepository _TermsandConditionsRepository;
        private readonly IVoyageRepository _VoyageRepository;
        private readonly IPricingRepository _PricingRepository;
        private readonly IClientRepository _clientRepository;
        public BookingService(IBookingRepository repo, ITermsandConditionsRepository termsandConditionsRepository = null, IVoyageRepository VoyageRepository = null, IPricingRepository pricingRepository = null, IClientRepository clientRepository = null)
        {
            _repo = repo;
            _TermsandConditionsRepository = termsandConditionsRepository;
            _VoyageRepository = VoyageRepository;
            _PricingRepository = pricingRepository;
            _clientRepository = clientRepository;
        }

        public ContainerBooking GetbookingByUUID(string bookinguuid)
        {
            ContainerBooking booking = new ContainerBooking();
            if (!string.IsNullOrEmpty(bookinguuid))
            {
                booking.booking = _repo.GetbookingByUUID(bookinguuid);
                booking.bookingSummary = _repo.GetBookingSummaryInfo(bookinguuid);
                GetVoyageInfo(booking);
                PrefillShipperConsignee(booking);
                var allServices = _repo.GetBookingAdditionalServices(bookinguuid);
                if (allServices != null && allServices.Count > 0)
                {
                    // POL
                    booking.booking.additionalservices = allServices.Where(x => x.type == 1).OrderBy(x => x.order).ToList();
                    // POD
                    booking.booking.PODadditionalservices = allServices.Where(x => x.type == 2).OrderBy(x => x.order).ToList();
                }
            }
            SessionManager.Booking = booking;
            return booking;
        }

        private void GetVoyageInfo(ContainerBooking booking)
        {
            try
            {
                if (!string.IsNullOrEmpty(booking.booking.location.voyageuuid))
                {
                    booking.voyage = _VoyageRepository.GetVoyageByUUID(booking.booking.location.voyageuuid);
                }
            }
            catch (Exception)
            { }
        }

        private void PrefillShipperConsignee(ContainerBooking booking)
        {
            try
            {
                var customer = booking.booking.customer;
                var client = customer.client;
                var location = booking.booking.location;

                if (client.clientdetailid.NumericValue > 0)
                {
                    switch (customer.customertype.NumericValue)
                    {
                        case 1: // Shipper
                            if (string.IsNullOrEmpty(location.shippername))
                            {
                                location.shipperdetailid = client.clientdetailid;
                                location.shipperpic = location.shipperpic;
                                location.shipperpiccustom = location.shipperpiccustom;
                                location.shippername = client.clientname;
                                location.shipperemail = client.email;
                                location.shipperphone = client.phone;
                                location.shipperaddress = client.address;
                            }
                            break;

                        case 2: // Consignee
                            if (string.IsNullOrEmpty(location.consigneename))
                            {
                                location.consigneedetailid = client.clientdetailid;
                                location.consigneepic = location.consigneepic;
                                location.shipperpiccustom = location.shipperpiccustom;
                                location.consigneename = client.clientname;
                                location.consigneeemail = client.email;
                                location.consigneephone = client.phone;
                                location.consigneeaddress = client.address;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            { RecordException(ex); }
        }

        public BookingCustomerDTO GetBookingCustomerInfo(string bookinguuid)
        {
            BookingCustomerDTO booking = new BookingCustomerDTO();
            if (string.IsNullOrEmpty(bookinguuid))
                booking = _repo.GetBookingCustomerInfo(bookinguuid);
            return booking;
        }

        public BookingLocationDTO GetBookingLocationInfo(string bookinguuid)
        {
            BookingLocationDTO location = new BookingLocationDTO();
            if (string.IsNullOrEmpty(bookinguuid))
                location = _repo.GetBookingLocationInfo(bookinguuid);
            return location;
        }

        public void SaveCustomer(int bookingid, BookingCustomerDTO customer)
        {
            try
            {
                result = _repo.SaveCustomer(bookingid, customer);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public void SaveLocation(string bookinguuid, BookingLocationDTO location)
        {
            try
            {
                result = _repo.SaveLocation(bookinguuid, location);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public List<BookingList> GetbookingList(BookingListFilter filter)
        {
            List<ContainerBookingListDTO> list = new List<ContainerBookingListDTO>();
            filter.filters.pol = Common.Decrypt(filter.filters.polencrypt);
            filter.filters.pod = Common.Decrypt(filter.filters.podencrypt);
            filter.filters.vesselid = filter.filters.vesseldetailid
                                      .Select(x => Common.Decrypt(x))
                                      .ToList();
            try
            {
                list = _repo.GetbookingList(filter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }

            return list.Select(x => new BookingList()
            {
                containerbookinglist = x,
                menu = new MasterMenus()
                {
                    edit = true
                }
            }).ToList();
        }

        public void SaveContainers(string bookingid, ContainerBookingDetailDTO containers)
        {
            try
            {
                containers.services = containers.services ?? new List<ContainerBookingDetailServicesDTO>();
                result = _repo.SaveContainers(bookingid, containers);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public List<ShuntingServiceDTO> GetShuntingServices()
        {
            List<ShuntingServiceDTO> service = new List<ShuntingServiceDTO>();
            return service = _repo.GetShuntingServices();
        }
        public ContainerBooking GetContainerBookingDetailByBookingUUId(string bookingId)
        {
            ContainerBooking booking = new ContainerBooking();
            if (!string.IsNullOrEmpty(bookingId))
            {
                booking.booking.details = _repo.GetContainerBookingDetailByBookingUUId(bookingId);
            }
            SessionManager.Booking = booking;
            return booking;
        }
        public void DeleteBookingContainerByUUID(string bookingdetailuuid)
        {
            try
            {
                result = _repo.DeleteBookingContainerByUUID(bookingdetailuuid);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public void SaveContainerBookingPickSelection(ContainerBooking model)
        {
            try
            {
                result = _repo.SaveContainerBookingPickSelection(model.booking.details);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public ContainerBooking GetContainerBookingDetailByPickUUID(string pickuuid)
        {
            ContainerBooking Booking = new ContainerBooking();
            ContainerBookingDTO bookingDto = new ContainerBookingDTO();
            if (!string.IsNullOrEmpty(pickuuid))
            {
                var containerDetails = _repo.GetContainerBookingDetailByPickUUID(pickuuid);
                if (containerDetails != null && containerDetails.Count > 0)
                {
                    bookingDto.details = containerDetails;
                    string bookingUuid = containerDetails.FirstOrDefault().bookinguuid;
                    if (!string.IsNullOrEmpty(bookingUuid))
                    {
                        bookingDto.customer = _repo.GetBookingCustomerInfo(bookingUuid, bookingDto);
                    }
                    Booking.booking = bookingDto;
                }
            }
            return Booking;
        }

        public List<ContainerAdditionalServicesDTO> GetContainerAdditionalServices()
        {
            List<ContainerAdditionalServicesDTO> service = new List<ContainerAdditionalServicesDTO>();
            return service = _repo.GetContainerAdditionalServices();
        }
        public ContainerBooking GetBookingAdditionalServices(string bookinguuid)
        {
            ContainerBooking booking = new ContainerBooking();
            if (!string.IsNullOrEmpty(bookinguuid))
            {
                booking.booking.additionalservices = _repo.GetBookingAdditionalServices(bookinguuid);
            }
            SessionManager.Booking = booking;
            return booking;
        }

        public void SaveBookingAdditionalServices(string bookingid, ContainerBooking model)
        {
            try
            {
                var mergedServices = new List<BookingAdditionalServicesDTO>();
                if (model.booking.additionalservices != null)
                    mergedServices.AddRange(model.booking.additionalservices.Where(x => !string.IsNullOrEmpty(x.uom)));
                if (model.booking.PODadditionalservices != null)
                    mergedServices.AddRange(model.booking.PODadditionalservices.Where(x => !string.IsNullOrEmpty(x.uom)));
                result = _repo.SaveBookingAdditionalServices(bookingid, mergedServices);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public OtherFeesDTO GetOtherFeesLineItem(string otherfeeuuid)
        {
            var session = SessionManager.Booking ?? new ContainerBooking();
            OtherFeesDTO detail = session.bookingSummary.otherfees.FirstOrDefault(x => x.otherfeeuuid == otherfeeuuid) ?? new OtherFeesDTO();
            return detail;
        }

        public ContainerBooking SaveOtherFeesLineItem(OtherFeesDTO model)
        {
            var session= SessionManager.Booking;
            var index = session.bookingSummary.otherfees.FindIndex(x => x.otherfeeuuid == model.otherfeeuuid);
            if (index < 0)
            {
                model.otherfeeuuid = Guid.NewGuid().ToString();
                session.bookingSummary.otherfees.Add(model);
            }
            else
            {
                session.bookingSummary.otherfees[index] = model;
            }
            SessionManager.Booking = session;
            return session;
        }

        public void SaveQuotation(string bookinguuid, QuotationHeaderDTO model)
        {
            result = Common.ErrorMessage("Cannot save Quotation details");
            try
            {
                var oldDetails = SessionManager.QuotationDetails ?? new List<QuotationDetailDTO>();
                var uiDetails = model.details ?? new List<QuotationDetailDTO>();
                var finalDetails = new List<QuotationDetailDTO>();
                foreach (var oldItem in oldDetails)
                {
                    bool existsInUI = uiDetails.Any(x => x.quotationdetailuuid == oldItem.quotationdetailuuid);
                    if (!existsInUI)
                    {
                        oldItem.isdeleted = true;
                        finalDetails.Add(oldItem);
                    }
                }
                foreach (var uiItem in uiDetails)
                {
                    uiItem.isdeleted = false;
                    finalDetails.Add(uiItem);
                }
                model.details = finalDetails;

                // type and size 
                foreach (var detail in model.details)
                {
                    var typesize = detail.TypeSizeCombinedValue;
                    if (!string.IsNullOrEmpty(typesize))
                    {
                        var splitarray = typesize.Split(
                            new[] { "@@" },
                            StringSplitOptions.None
                        );

                        if (splitarray.Length > 1)
                        {
                            detail.containertypeid = new EncryptedData
                            {
                                EncryptedValue = splitarray[0]
                            };
                            detail.containersizeid = new EncryptedData
                            {
                                EncryptedValue = splitarray[1]
                            };
                        }
                        //else
                        //{
                        //    detail.containertypeid = null;
                        //    detail.containersizeid = null;
                        //}
                    }
                }
                result = _repo.SaveQuotation(bookinguuid, model);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
        }

        public BookingQuotationList GetQuotationListBookingUUID(string bookinguuid)
        {
            BookingQuotationList booking = new BookingQuotationList();
            try
            {
                if (!string.IsNullOrEmpty(bookinguuid))
                {
                    booking.bookinguuid = bookinguuid;
                    booking.quotation = _repo.GetQuotationListBookingUUID(bookinguuid);
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
            return booking;
        }

        public BookingQuotation GetQuotationByUUID(string quotationuuid)
        {
            BookingQuotation booking = new BookingQuotation();
            try
            {
                if (!string.IsNullOrEmpty(quotationuuid))
                {
                    booking.quotation = _repo.GetQuotationByUUID(quotationuuid);

                    /* -- Comments -- */
                    IChatRepository chatRepository = new ChatRepository();
                    ChatService _chatService = new ChatService(chatRepository);
                    booking.comments = _chatService.GetChatList(booking.quotation.quotationid.NumericValue, ChatType.Quotation);
                    /* -- Comments -- */

                    if (!string.IsNullOrEmpty(booking.quotation.bookinguuid))
                    {
                        booking.booking = _repo.GetbookingByUUID(booking.quotation.bookinguuid);
                        try
                        {
                            if (!string.IsNullOrEmpty(booking.booking.location.voyageuuid))
                            {
                                booking.voyage = _VoyageRepository.GetVoyageByUUID(booking.booking.location.voyageuuid);
                            }
                        }
                        catch (Exception)
                        { }
                    }

                    if (booking.quotation.details != null)
                    {
                        foreach (var detail in booking.quotation.details)
                        {
                            if (detail.containertypeid.NumericValue > 0 && detail.containersizeid.NumericValue > 0)
                            {
                                detail.TypeSizeCombinedValue = detail.containertypeid.EncryptedValue + "@@" + detail.containersizeid.EncryptedValue;
                                detail.TypeSizeCombinedText = detail.containersizename + " " + detail.containertypename;
                            }
                        }
                    }
                    //string voyageuuid = booking.booking.location.voyageuuid;
                    //if (booking.booking != null)
                    //{
                    //    if (voyageuuid != null && voyageuuid != "")
                    //    {
                    //        booking.Voyage = _VoyageRepository.GetVoyageByUUID(voyageuuid) ?? new VoyageDTO();
                    //    }
                    //}
                }
                if (booking.quotation != null && booking.quotation.quotationid.NumericValue > 0)
                {
                    SessionManager.Quotation = booking;
                    SessionManager.QuotationDetails = booking.quotation.details != null ? booking.quotation.details.Select(x => Cloner.DeepClone(x)).ToList() : new List<QuotationDetailDTO>();
                }
                else
                {
                    booking = new BookingQuotation();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return booking;
        }

        public BookingQuotation PrePopulateQuotation(string bookingUuid)
        {
            ContainerBooking ContainerBooking = new ContainerBooking();
            ContainerBooking.booking = _repo.GetbookingByUUID(bookingUuid);
            return PrePopulationDataInternal(bookingUuid, ContainerBooking);
        }

        private BookingQuotation PrePopulationDataInternal(string bookingUuid, ContainerBooking bookingWrapper)
        {
            string termsText = "";
            //var termsandconditions = _TermsandConditionsRepository.GetTermsAndConditionsbyTypeandAgency("ContainerQuotation", bookingWrapper.booking.customer.agencydetailid.EncryptedValue);
            if (_TermsandConditionsRepository != null &&
        bookingWrapper?.booking?.customer?.agencydetailid != null)
            {
                var termsandconditions =
                    _TermsandConditionsRepository
                        .GetTermsAndConditionsbyTypeandAgency(
                            "ContainerQuotation",
                            bookingWrapper.booking.customer.agencydetailid.EncryptedValue);

                termsText = termsandconditions?.TermsText ?? "";
            }
            var quotation = new BookingQuotation
            {
                quotation = new QuotationHeaderDTO
                {
                    bookinguuid = bookingUuid,
                    agencydetailid = bookingWrapper.booking.customer.agencydetailid,
                    agencyname = bookingWrapper.booking.customer.agencyname,
                    termsandconditions = termsText,
                    details = new List<QuotationDetailDTO>()
                },
                booking = bookingWrapper.booking
            };

            var client = GetBillToAddressByClient(bookingWrapper.booking.customer.client.clientdetailid.EncryptedValue);

            quotation.quotation.currency = client.preferredcurrency;
            quotation.quotation.quotationfor = bookingWrapper.booking.customer.client.clientdetailid;
            quotation.quotation.quotationforname = client.clientname;
            quotation.quotation.billto = client.address;

            var pricinglist = _PricingRepository.GetPricingByBookingUUID(bookingUuid, client.preferredcurrency, bookingWrapper.booking.customer.client.clientdetailid.EncryptedValue);
            quotation.quotation.details = Common.PopulateQuotationItemsFromPricing(pricinglist);
            SessionManager.Quotation = quotation;
            return quotation;
        }

        public string GetBillToAddressByCustomerType(string customertype)
        {
            var customerTypeId = Common.Decrypt(customertype);
            var session = SessionManager.Quotation;

            if (session.booking.location == null)
                return string.Empty;

            var location = session.booking.location;
            var lines = new List<string>();
            if (customerTypeId == 1) // Shipper
            {
                lines.Add(location.shippername);
                lines.Add(location.shipperaddress);
                var contact = Common.BuildContactLine(location.shipperemail, location.shipperphone);
                if (!string.IsNullOrEmpty(contact))
                    lines.Add(contact);
            }
            else if (customerTypeId == 2) // Consignee
            {
                lines.Add(location.consigneename);
                lines.Add(location.consigneeaddress);
                var contact = Common.BuildContactLine(location.consigneeemail, location.consigneephone);
                if (!string.IsNullOrEmpty(contact))
                    lines.Add(contact);
            }
            return Common.BuildMultilineText(lines);
        }

        public ClientDTO GetBillToAddressByClient(string clientdetailid)
        {
            ClientRepository clientRepo = new ClientRepository();
            var client = clientRepo.GetClientByDetailID(Common.Decrypt(clientdetailid));
            if (!string.IsNullOrEmpty(client.clientuuid)) // Get ClientInfo from ClientUUID because to load latest data in case of any changes
                client = clientRepo.GetClientByUUID(client.clientuuid);

            var lines = new List<string>();
            lines.Add(client.clientname);
            lines.Add(client.address);
            if (!string.IsNullOrEmpty(client.email))
                lines.Add(client.email);
            if (!string.IsNullOrEmpty(client.phone))
                lines.Add(client.phone);

            client.address = Common.BuildMultilineText(lines);
            return client;
        }
        public QuotationDetailDTO GetQuotationLineItem(string quotationdetailuuid)
        {
            var sessionQuotation = SessionManager.Quotation ?? new BookingQuotation();
            QuotationDetailDTO detail = sessionQuotation.quotation.details.FirstOrDefault(x => x.quotationdetailuuid == quotationdetailuuid) ?? new QuotationDetailDTO();
            return detail;
        }

        public BookingQuotation SaveQuotationLineItem(QuotationDetailDTO model)
        {
            var sessionQuotation = SessionManager.Quotation;

            var index = sessionQuotation.quotation.details.FindIndex(x => x.quotationdetailuuid == model.quotationdetailuuid);
            if (index < 0)
            {
                model.quotationdetailuuid = Guid.NewGuid().ToString();
                model.isdeleted = false;
                sessionQuotation.quotation.details.Add(model);
            }
            else
            {
                sessionQuotation.quotation.details[index] = model;
            }
            SessionManager.Quotation = sessionQuotation;
            return sessionQuotation;
        }

        public void DeleteQuotationByUUID(string quotationuuid)
        {
            result = Common.ErrorMessage("Cannot Delete Quotation");
            try
            {
                result = _repo.DeleteQuotationByUUID(quotationuuid);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public ContainerAllocation GetContainerAllocations(string bookingdetailuuid, string bookinguuid)
        {
            ContainerBookingDTO booking = _repo.GetbookingByUUID(bookinguuid);
            return new ContainerAllocation()
            {
                Container = booking.details.Where(d => d.bookingdetailuuid == bookingdetailuuid).FirstOrDefault() ?? new ContainerBookingDetailDTO(),
                Allocations = _repo.GetContainerAllocations(bookingdetailuuid, bookinguuid),
                Booking = booking
            };
        }
        public void SaveContainerAllocation(string bookingid, string bookingdetailid, ContainerAllocate allocation)
        {
            try
            {
                ContainerAllocationDTO allocations = new ContainerAllocationDTO()
                {
                    LocationId = new EncryptedData() { EncryptedValue = allocation.LocationID },
                    Models = new List<ContainerAllocationModelDTO>()
                    {
                        new ContainerAllocationModelDTO()
                        {
                            ReservationID = new EncryptedData() { EncryptedValue = allocation.ReserveID },
                            ContainerModelId = new EncryptedData() { EncryptedValue = allocation.ModelID },
                            SelectedCount = allocation.Qty
                        }
                    }
                };
                result = _repo.SaveContainerAllocation(bookingid, bookingdetailid, new List<ContainerAllocationDTO>() { allocations });
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public void DeleteContainerAllocation(string bookingid, string bookingdetailid, string locationid, string modelid)
        {
            try
            {
                result = _repo.DeleteContainerAllocation(bookingid, bookingdetailid, locationid, modelid);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public ContainerSelection GetContainerSelection(string bookinguuid)
        {
            ContainerBookingDTO booking = _repo.GetbookingByUUID(bookinguuid);
            var selections = _repo.GetContainerSelection(bookinguuid);
            var allotted = _repo.GetContainerAllotment(bookinguuid);
            selections.SelectMany(l => l.Details).SelectMany(d => d.Containers).ToList()
            .ForEach(c =>
            {
                c.Locked = !string.IsNullOrEmpty(c.AllocationBookingUUID) && c.AllocationBookingUUID != booking.bookinguuid;
                c.Allotted = c.AllocationBookingUUID == booking.bookinguuid || allotted.Exists(x => x.containerid.NumericValue == c.ContainerID.NumericValue);
            });

            ContainerSelection seletion = new ContainerSelection()
            {
                Selections = selections,
                Booking = booking,
                Allotted = allotted
            };
            SessionManager.CurrentContainerSelection = seletion;
            return seletion;
        }
        public List<Quotationlist> GetQuotationList(QuotationListFilter filter)
        {
            List<Quotationlist> list = new List<Quotationlist>();
            try
            {
                List<QuotationList> quotations = new List<QuotationList>();
                filter.filters.hodapprover = Common.Decrypt(filter.filters.hodapprover_enc);
                quotations = _repo.GetQuotationList(filter);
                quotations.ForEach(x => x.quotationno = Codes.GetCodes(x.quotationid.NumericValue, x.createdat.Value, "QTN", x.quotationno));

                return quotations.Select(x => new Quotationlist()
                {
                    QuotationLists = x,
                    menu = new QuoteMenus()
                    {
                        edit = true,
                        delete = x.status.NumericValue == QuoteStatus.Draft,
                        approve = x.status.NumericValue == QuoteStatus.Awaiting_HOD_approval && Common.RoleID == UserRols.Approver_HOD,
                    }
                }).ToList();
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<Quotationlist>();
            }
        }
        private void ProcessBookingFilters(BookingListFilter filter)
        {
            if (filter.filters == null)
                filter.filters = new BookingFilters();

            if (!string.IsNullOrEmpty(filter.filters.polencrypt))
            {
                int id = Common.Decrypt(filter.filters.polencrypt);
                if (id > 0) filter.filters.pol = id;
            }

            if (!string.IsNullOrEmpty(filter.filters.podencrypt))
            {
                int id = Common.Decrypt(filter.filters.podencrypt);
                if (id > 0) filter.filters.pod = id;
            }

            if (filter.filters.status.Count > 0)
                filter.filters.status = filter.filters.status;
            else
                filter.filters.status = new List<int>();

            if (filter.filters.agencyuuids == null)
                filter.filters.agencyuuids = new List<string>();

            if (filter.filters.clientuuids == null)
                filter.filters.clientuuids = new List<string>();

            if (filter.filters.vesseldetailid != null && filter.filters.vesseldetailid.Any())
            {
                filter.filters.vesselid = filter.filters.vesseldetailid
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => Common.Decrypt(x))
                    .Where(id => id > 0)
                    .ToList();
            }

            if (filter.filters.createdby == null)
                filter.filters.createdby = new List<int>();
        }
        public void PopulateBookingStatusCounts(BookingListFilter filter)
        {
            try
            {
                ProcessBookingFilters(filter);
                string json = JsonConvert.SerializeObject(filter);
                BookingListFilter countFilter = JsonConvert.DeserializeObject<BookingListFilter>(json);
                filter.StatusCount = _repo.GetBookingStatusCount(countFilter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
        }
        public List<BookingStatusCountDTO> GetBookingStatusCount(BookingListFilter filter)
        {
            try
            {
                ProcessBookingFilters(filter);
                return _repo.GetBookingStatusCount(filter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<BookingStatusCountDTO>();
            }
        }
        private void ProcessFilters(QuotationListFilter filter)
        {
            if (filter.filters == null)
                filter.filters = new QuotationFilter();
            if (filter.filters.status > 0)
                filter.filters.status_list = new List<int> { filter.filters.status };
            else
                filter.filters.status_list = new List<int>();
        }

        public void PopulateStatusCounts(QuotationListFilter filter)
        {
            try
            {
                ProcessFilters(filter);
                string json = JsonConvert.SerializeObject(filter);
                QuotationListFilter countFilter = JsonConvert.DeserializeObject<QuotationListFilter>(json);
                filter.StatusCount = _repo.GetQuotationStatusCount(countFilter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }

        }
        public List<QuotationStatusCountDTO> GetQuotationStatusCount(QuotationListFilter filter)
        {
            try
            {
                ProcessFilters(filter);
                return _repo.GetQuotationStatusCount(filter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<QuotationStatusCountDTO>();
            }
        }

        public List<BookingAllocationAcquireDTO> AcquireContainer(string bookinguuid, List<AcquireDTO> bookingids, string locationuuid, string modeluuid)
        {
            List<BookingAllocationAcquireDTO> allocations = new List<BookingAllocationAcquireDTO>();
            try
            {
                allocations = _repo.AcquireContainer(bookinguuid, bookingids, locationuuid, modeluuid);
                var hub = GlobalHost.ConnectionManager.GetHubContext<ContainerHub>();
                string groupName = $"LOC_{locationuuid}_MOD_{modeluuid}";
                foreach (var item in allocations)
                {
                    hub.Clients.Group(groupName).containerBooked(item.ContainerID.EncryptedValue, item.AllocationBookingUUID);
                }
                // Released Containers
                foreach (var item in bookingids.Where(x => x.isdeleted))
                {
                    hub.Clients.Group(groupName).containerReleased(item.containerid);
                }

            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return allocations;
        }
        public void SaveContainerSelection(string bookingid, List<ContainerSelectionDTO> selections)
        {
            try
            {
                selections = selections ?? new List<ContainerSelectionDTO>();
                var seletion = SessionManager.CurrentContainerSelection ?? new ContainerSelection();

                var latestContainerIds = selections
                                        .SelectMany(l => l.Details)
                                        .SelectMany(d => d.Containers)
                                        .Where(x => x.Selected)
                                        .Select(c => c.ContainerID.EncryptedValue)
                                        .ToHashSet();

                foreach (var oldLocation in seletion.Selections)
                {
                    var newLocation = selections
                        .FirstOrDefault(x => x.LocationUuid == oldLocation.LocationUuid);

                    if (newLocation == null)
                    {
                        selections.Add(new ContainerSelectionDTO
                        {
                            LocationUuid = oldLocation.LocationUuid,
                        });
                    }

                    foreach (var oldModel in oldLocation.Details)
                    {
                        var newModel = newLocation.Details
                            .FirstOrDefault(x => x.ContainerModelUuid == oldModel.ContainerModelUuid);

                        if (newModel == null)
                        {
                            newLocation.Details.Add(new ContainerSelectionDetailDTO
                            {
                                ContainerModelUuid = oldModel.ContainerModelUuid,
                            });
                        }

                        foreach (var oldCont in oldModel.Containers)
                        {
                            if (!latestContainerIds.Contains(oldCont.ContainerID.EncryptedValue))
                            {
                                newModel.Containers.Add(new SelectionItemDTO
                                {
                                    ContainerID = oldCont.ContainerID,
                                    IsDeleted = true,
                                    Selected = true
                                });
                            }
                        }
                    }
                }

                result = _repo.SaveContainerSelection(bookingid, selections);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public void ConfirmContainerSelection(string bookingid)
        {
            try
            {
                result = _repo.ConfirmContainerSelection(bookingid);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public BookedContainer GetBookedContainers(string bookinguuid, BookedContainerFilter filter, bool GetShipmentInfo = false)
        {
            BookedContainer bookedContainer = _GetBookedContainerList(bookinguuid, filter, GetShipmentInfo);
            return bookedContainer;
        }

        private BookedContainer _GetBookedContainerList(string bookinguuid, BookedContainerFilter filter, bool GetShipmentInfo)
        {
            BookedContainer bookedContainer = new BookedContainer();
            try
            {
                bookedContainer.booking = _repo.GetbookingByUUID(bookinguuid);
                bookedContainer.containers = _repo.GetBookedContainers(bookinguuid, filter);
                if (GetShipmentInfo)
                {
                    var list = _repo.GetShipmentConfimation(bookinguuid);
                    var joined = from d in bookedContainer.containers
                                 join c in list
                                 on d.ContainerUuid equals c.ContainerUuid
                                 select new { d, c };

                    foreach (var item in joined)
                    {
                        item.d.Stamp = item.c.Stamp;
                        item.d.Weight = item.c.Weight;
                    }
                }
                GetBookedContainerVoyageInfo(bookedContainer);
                /* Container Alteration*/
                bookedContainer.containers = bookedContainer.containers.OrderBy(x => x.ContainerTypeID + "," + x.ContainerSizeID).ThenBy(x => x.ContainerNo).ToList();
                var groupdata = bookedContainer.booking.details.GroupBy(x => x.containertypeid.NumericValue + "," + x.sizeid.NumericValue).ToList();
                foreach (var detail in groupdata)
                {
                    bookedContainer.containers
                        .Where(x => x.ContainerSizeID == detail.First().sizeid.NumericValue
                                && x.ContainerTypeID == detail.First().containertypeid.NumericValue
                                && x.Weight <= 0)
                        .ToList()
                        .ForEach(x => x.Weight = Common.ToDecimal(detail.First().grossweight.ToString("0.00")));
                }
                /* Container Alteration*/
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return bookedContainer;
        }

        private void GetBookedContainerVoyageInfo(BookedContainer booking)
        {
            try
            {
                if (!string.IsNullOrEmpty(booking.booking.location.voyageuuid))
                {
                    booking.voyage = _VoyageRepository.GetVoyageByUUID(booking.booking.location.voyageuuid);
                }
            }
            catch (Exception)
            { }
        }

        public void SendQuotationForApproval(SendApproval approval)
        {
            try
            {
                result = _repo.SendQuotationForApproval(approval);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public void SaveSummaryInfo(BookingSummaryDTO summary)
        {
            result = Common.ErrorMessage("Cannot save Summary");
            try
            {
                result = _repo.SaveSummaryInfo(summary);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public BookingSummaryDTO GetBookingSummaryInfo(string bookinguuid)
        {
            BookingSummaryDTO model = new BookingSummaryDTO();
            try
            {
                model = _repo.GetBookingSummaryInfo(bookinguuid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return model;
        }
        public void SaveShipmentConfimation(string bookinguuid, List<BookedContainerDTO> containers)
        {
            try
            {
                result = _repo.SaveShipmentConfimation(bookinguuid, containers);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public void SaveCabotage(string bookinguuid, List<string> ContainerUuids)
        {
            try
            {
                result = _repo.SaveCabotage(bookinguuid, ContainerUuids);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public BookedContainer GetCabotage(string cabotageuuid)
        {
            BookedContainer bookedContainer = new BookedContainer();
            try
            {
                var cablist = _repo.GetCabotage(cabotageuuid);
                if (cablist.Count > 0)
                {
                    bookedContainer = _GetBookedContainerList(cablist[0].BookingUUID, new BookedContainerFilter(), true);
                    bookedContainer.containers.RemoveAll(c => cablist.All(cl => cl.ContainerUuid != c.ContainerUuid));
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return bookedContainer;
        }
        public Cabotage GetCabotageList(string bookinguuid)
        {
            Cabotage cabotage = new Cabotage();
            try
            {
                cabotage.booking = _repo.GetbookingByUUID(bookinguuid);
                if (!string.IsNullOrEmpty(cabotage.booking.location.voyageuuid))
                {
                    cabotage.voyage = _VoyageRepository.GetVoyageByUUID(cabotage.booking.location.voyageuuid);
                }
                cabotage.cabatages = _repo.GetCabotageList(bookinguuid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return cabotage;
        }
        public void ValidateContainerQty(string bookinguuid, string bookingdetailuuid, int qty)
        {
            try
            {
                result = _repo.ValidateContainerQty(bookinguuid, bookingdetailuuid, qty);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public void DeleteContainerFromBooking(string bookinguuid, string containeruuid)
        {
            try
            {
                result = _repo.DeleteContainerFromBooking(bookinguuid, containeruuid);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

    }
}