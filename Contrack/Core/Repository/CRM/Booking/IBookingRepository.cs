using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IBookingRepository
    {
        Result SaveCustomer(int bookingid, BookingCustomerDTO customer);
        Result SaveLocation(string bookinguuid, BookingLocationDTO location);
        Result SaveContainers(string bookingid, ContainerBookingDetailDTO containers);
        Result SaveBooking(ContainerBookingDTO booking); // Will be created later
        ContainerBookingDTO GetbookingByUUID(string bookinguuid);
        List<ContainerBookingListDTO> GetbookingList(BookingListFilter filter); // Filter will be created later
        BookingCustomerDTO GetBookingCustomerInfo(string bookinguuid, ContainerBookingDTO parent = null);
        BookingLocationDTO GetBookingLocationInfo(string bookinguuid, ContainerBookingDTO parent = null);
        List<ShuntingServiceDTO> GetShuntingServices();
        List<ContainerBookingDetailDTO> GetContainerBookingDetailByBookingUUId(string bookingId);
        Result DeleteBookingContainerByUUID(string bookingdetailuuid);
        List<ContainerBookingDetailDTO> GetContainerBookingDetailByPickUUID(string pickuuid);
        Result SaveContainerBookingPickSelection(List<ContainerBookingDetailDTO> selections);
        List<ContainerAdditionalServicesDTO> GetContainerAdditionalServices();
        List<BookingAdditionalServicesDTO> GetBookingAdditionalServices(string bookinguuid);
        Result SaveBookingAdditionalServices(string bookingid, List<BookingAdditionalServicesDTO> services);
        Result SaveQuotation(string bookinguuid, QuotationHeaderDTO model);
        List<QuotationList> GetQuotationListBookingUUID(string bookinguuid);
        QuotationHeaderDTO GetQuotationByUUID(string quotationuuid);
        Result DeleteQuotationByUUID(string quotationuuid);
        List<ContainerAllocationDTO> GetContainerAllocations(string bookingdetailuuid, string bookinguuid);
        List<BookingStatusCountDTO> GetBookingStatusCount(BookingListFilter filter);
        Result SaveContainerAllocation(string bookingid, string bookingdetailid, List<ContainerAllocationDTO> allocations);
        Result DeleteContainerAllocation(string bookingid, string bookingdetailid, string locationid, string modelid);
        List<ContainerSelectionDTO> GetContainerSelection(string bookinguuid);
        List<QuotationList> GetQuotationList(QuotationListFilter filter);
        List<QuotationStatusCountDTO> GetQuotationStatusCount(QuotationListFilter filter);
        List<BookingAllocationAcquireDTO> AcquireContainer(string bookinguuid, List<AcquireDTO> bookingids, string locationuuid, string modeluuid);
        Result SaveContainerSelection(string bookingid, List<ContainerSelectionDTO> selections);
        List<ContainerAllottedDTO> GetContainerAllotment(string bookinguuid);
        Result ConfirmContainerSelection(string bookingid);
        List<BookedContainerDTO> GetBookedContainers(string bookinguuid, BookedContainerFilter filters);
        Result SendQuotationForApproval(SendApproval approval);
        Result SaveSummaryInfo(BookingSummaryDTO summary);
        BookingSummaryDTO GetBookingSummaryInfo(string bookinguuid);
        Result SaveShipmentConfimation(string bookinguuid, List<BookedContainerDTO> containers);
        List<ShipmentConfirmationDTO> GetShipmentConfimation(string bookinguuid);
        Result SaveCabotage(string bookinguuid, List<string> ContainerUuids);
        List<ShipmentConfirmationDTO> GetCabotage(string cabotageuuid);
        List<CabotageDTO> GetCabotageList(string bookinguuid);
        Result ValidateContainerQty(string bookinguuid, string bookingdetailuuid, int qty);
        Result DeleteContainerFromBooking(string bookinguuid, string containeruuid);

    }
}
