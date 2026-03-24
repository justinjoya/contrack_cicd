using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IPricingService
    {
        void SaveHeader(PricingHeaderDTO header);
        void CloneHeader(PricingHeaderDTO header);
        void SaveType(PricingTypeDTO type);
        void CloneType(PricingTypeDTO type);
        void SaveDetail(PricingDetailDTO detail);
        PricingDetailDTO GetDetail(string typeid, string detailid);
        void SaveCustomer(PricingCustomerDTO customer);
        void DeleteCustomer(PricingCustomerDTO customer);
        void DeleteDetail(string typeuuid, string detailuuid);
        QuotationPricing GetPricingByHeader(string pricinguuid, string typeuuid, bool nodetail);
        List<PricingHeaderDTO> GetPricingList(PricingListFilter filter);
        List<PricingCustomerDTO> GetClientsList(string pricinguuid);
        PricingTypeDTO GetPricingByBookingUUID(string bookinguuid, string currency, string clientdetailid);
        PricingDetailDTO GetPricingByLineItemUUID(string bookinguuid, string currency, string clientdetailid, string lineitemuuid, string typeid, string sizeid, string fullempty);
        List<QuotationDetailDTO> GetPricingListByBookingUUID(string bookinguuid, string currency, string clientdetailid);
    }
}
