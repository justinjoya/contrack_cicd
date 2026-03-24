using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IPricingRepository
    {
        Result SaveHeader(PricingHeaderDTO header);
        Result CloneHeader(PricingHeaderDTO header);
        Result SaveType(PricingTypeDTO Type);
        Result CloneType(PricingTypeDTO Type);
        Result SaveDetail(PricingDetailDTO detail);
        Result SaveCustomer(PricingCustomerDTO customer);
        Result DeleteCustomer(PricingCustomerDTO customer);
        Result DeleteDetail(string typeuuid, string detailuuid);
        PricingHeaderDTO GetHeader(string pricinguuid);
        PricingTypeDTO GetTransferType(string typeuuid);
        PricingDetailDTO GetDetail(string typeid, string detailid);
        List<PricingHeaderDTO> GetPricingList(PricingListFilter filter);
        List<PricingCustomerDTO> GetClientsList(string pricinguuid);
        PricingTypeDTO GetPricingByBookingUUID(string bookinguuid, string currency, string clientdetailid);
        PricingDetailDTO GetPricingByLineItemUUID(string BookingUUID, string ClientDetailID, string Currency, string LineItemUUID, string TypeID, string SizeID, string FullEmpty);
    }
}
