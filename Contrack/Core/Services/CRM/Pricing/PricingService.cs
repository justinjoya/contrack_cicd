using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class PricingService : CustomException, IPricingService
    {
        public Result result = new Result();
        private readonly IPricingRepository _repo;
        public PricingService(IPricingRepository repo)
        {
            _repo = repo;
        }
        public void SaveHeader(PricingHeaderDTO header)
        {
            result = _repo.SaveHeader(header);
        }
        public void CloneHeader(PricingHeaderDTO header)
        {
            result = _repo.CloneHeader(header);
        }
        public void SaveType(PricingTypeDTO type)
        {
            result = _repo.SaveType(type);
        }
        public void CloneType(PricingTypeDTO type)
        {
            result = _repo.CloneType(type);
        }
        public void DeleteDetail(string typeuuid, string detailuuid)
        {
            result = _repo.DeleteDetail(typeuuid, detailuuid);
        }

        public void SaveDetail(PricingDetailDTO detail)
        {
            var typesize = detail.TypeSizeCombinedValue;
            if (!string.IsNullOrEmpty(typesize))
            {
                var splitarray = typesize.Split(new string[] { "@@" }, StringSplitOptions.None);
                if (splitarray.Length > 1)
                {
                    detail.ContainerTypeID = new EncryptedData()
                    {
                        EncryptedValue = splitarray[0],
                    };
                    detail.ContainerSizeID = new EncryptedData()
                    {
                        EncryptedValue = splitarray[1],
                    };
                }
            }
            result = _repo.SaveDetail(detail);
        }
        public void SaveCustomer(PricingCustomerDTO customer)
        {
            result = _repo.SaveCustomer(customer);
        }
        public void DeleteCustomer(PricingCustomerDTO customer)
        {
            result = _repo.DeleteCustomer(customer);
        }
        public QuotationPricing GetPricingByHeader(string pricinguuid, string typeuuid, bool nodetail = false)
        {
            QuotationPricing pricing = new QuotationPricing();
            try
            {
                pricing.pricing = _repo.GetHeader(pricinguuid);
                if (!nodetail)
                {
                    if (string.IsNullOrEmpty(typeuuid))
                        typeuuid = pricing.pricing.Types[0]?.TypeUUID;
                    pricing.transfertype = _repo.GetTransferType(typeuuid);
                    pricing.pricing.Types.RemoveAll(x => string.IsNullOrEmpty(x.TypeUUID));
                    pricing.transfertype.Details.RemoveAll(x => string.IsNullOrEmpty(x.DetailUUID));
                    pricing.pricing.Types.ForEach(x => x.IsCurrent = x.TypeUUID == typeuuid);

                    pricing.customers = _repo.GetClientsList(pricinguuid);
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return pricing;
        }
        private void ProcessFilters(PricingListFilter filter)
        {
            if (filter.filters == null) filter.filters = new PricingFilter();
            if (!string.IsNullOrEmpty(filter.filters.pol_encry))
            {
                int id = Common.Decrypt(filter.filters.pol_encry);
                if (id > 0) filter.filters.pols = new List<int> { id };
            }
            if (!string.IsNullOrEmpty(filter.filters.pod_encry))
            {
                int id = Common.Decrypt(filter.filters.pod_encry);
                if (id > 0) filter.filters.pods = new List<int> { id };
            }
            if (!string.IsNullOrEmpty(filter.filters.transfertype_encry))
            {
                int id = Common.ToInt(Common.Decrypt(filter.filters.transfertype_encry));
                if (id > 0) filter.filters.transfertypes = new List<int> { id };
            }
            if (!string.IsNullOrEmpty(filter.filters.client_encry))
            {
                int id = Common.Decrypt(filter.filters.client_encry);
                if (id > 0) filter.filters.clients = new List<int> { id };
            }
        }
        public List<PricingHeaderDTO> GetPricingList(PricingListFilter filter)
        {
            try
            {
                ProcessFilters(filter);
                return _repo.GetPricingList(filter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<PricingHeaderDTO>();
            }
        }
        public List<PricingCustomerDTO> GetClientsList(string pricinguuid)
        {
            try
            {
                return _repo.GetClientsList(pricinguuid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<PricingCustomerDTO>();
            }
        }
        public PricingDetailDTO GetDetail(string typeid, string detailid)
        {
            PricingDetailDTO detail = new PricingDetailDTO();
            detail = _repo.GetDetail(typeid, detailid);
            detail.TypeSizeCombinedValue = detail.ContainerTypeID.EncryptedValue + "@@" + detail.ContainerSizeID.EncryptedValue;
            return detail;
        }
        public PricingTypeDTO GetPricingByBookingUUID(string bookinguuid, string currency, string clientdetailid)
        {
            return _repo.GetPricingByBookingUUID(bookinguuid, currency, clientdetailid);
        }
        public PricingDetailDTO GetPricingByLineItemUUID(string BookingUUID, string ClientDetailID, string Currency, string LineItemUUID, string TypeID, string SizeID, string FullEmpty)
        {
            return _repo.GetPricingByLineItemUUID(BookingUUID, ClientDetailID, Currency, LineItemUUID, TypeID, SizeID, FullEmpty);
        }

        public List<QuotationDetailDTO> GetPricingListByBookingUUID(string bookinguuid, string currency, string clientdetailid)
        {
            List<QuotationDetailDTO> list = new List<QuotationDetailDTO>();
            var pricinglist = _repo.GetPricingByBookingUUID(bookinguuid, currency, clientdetailid);
            list = Common.PopulateQuotationItemsFromPricing(pricinglist);
            return list;
        }
    }
}