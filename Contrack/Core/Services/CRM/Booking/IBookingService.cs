using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    internal interface IBookingService
    {
        ContainerBooking GetbookingByUUID(string bookinguuid);
        BookingCustomerDTO GetBookingCustomerInfo(string bookinguuid);
        void SaveCustomer(int bookingid, BookingCustomerDTO customer);
        void SaveLocation(string bookinguuid, BookingLocationDTO location);
        void SaveContainers(string bookingid, ContainerBookingDetailDTO containers);
        List<ShuntingServiceDTO> GetShuntingServices();
        List<BookingList> GetbookingList(BookingListFilter filter);
        ContainerBooking GetContainerBookingDetailByBookingUUId(string bookingId);
        void SaveContainerBookingPickSelection(ContainerBooking model);
        ContainerBooking GetContainerBookingDetailByPickUUID(string pickuuid);
        void DeleteBookingContainerByUUID(string bookingdetailuuid);
        List<ContainerAdditionalServicesDTO> GetContainerAdditionalServices();
        ContainerBooking GetBookingAdditionalServices(string bookinguuid);
        void SaveBookingAdditionalServices(string bookingid, ContainerBooking model);
        void SaveQuotation(string bookinguuid, QuotationHeaderDTO model);
        BookingQuotationList GetQuotationListBookingUUID(string bookinguuid);
        BookingQuotation GetQuotationByUUID(string quotationuuid);
        void DeleteQuotationByUUID(string quotationuuid);
        ContainerAllocation GetContainerAllocations(string bookingdetailuuid, string bookinguuid);
        void SaveContainerAllocation(string bookingid, string bookingdetailid, ContainerAllocate allocation);
        void DeleteContainerAllocation(string bookingid, string bookingdetailid, string locationid, string modelid);
        ContainerSelection GetContainerSelection(string bookinguuid);
        List<Quotationlist> GetQuotationList(QuotationListFilter filter);
        List<QuotationStatusCountDTO> GetQuotationStatusCount(QuotationListFilter filter);
        List<BookingAllocationAcquireDTO> AcquireContainer(string bookinguuid, List<AcquireDTO> bookingids, string locationuuid, string modeluuid);
        List<BookingStatusCountDTO> GetBookingStatusCount(BookingListFilter filter);
        void SaveContainerSelection(string bookingid, List<ContainerSelectionDTO> selections);
        void ConfirmContainerSelection(string bookingid);
        BookedContainer GetBookedContainers(string bookinguuid, BookedContainerFilter filter, bool GetShipmentInfo = false);
        void SendQuotationForApproval(SendApproval approval);
        void SaveSummaryInfo(BookingSummaryDTO summary);
        BookingSummaryDTO GetBookingSummaryInfo(string bookinguuid);
        void SaveShipmentConfimation(string bookinguuid, List<BookedContainerDTO> containers);
        void SaveCabotage(string bookinguuid, List<string> ContainerUuids);
        BookedContainer GetCabotage(string cabotageuuid);
        Cabotage GetCabotageList(string bookinguuid);
        void ValidateContainerQty(string bookinguuid, string bookingdetailuuid, int qty);
        void DeleteContainerFromBooking(string bookinguuid, string containeruuid);


    }
}
