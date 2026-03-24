using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Contrack
{
    public class PurchaseOrderService : CustomException, IPurchaseOrderService
    {
        public Result result { get; set; } = new Result();
        private readonly IPurchaseOrderRepository _repo;
        public PurchaseOrderService(IPurchaseOrderRepository repo)
        {
            _repo = repo;
        }
        public List<ListItem> GetVendorFilterDropdown(List<string> agencyuuids)
        {
            return Dropdowns.GetVendorDropdownForFilter(agencyuuids, true);
        }

        public void SaveDirectPurchaseOrder(PurchaseOrderModel model)
        {
            try
            {
                var dto = model.ContainerPO.PO;
                var DetailsList = SessionManager.CurrentPODetails;
                foreach (var eachdetail in DetailsList)
                {
                    if (dto.Details.All(x => x.podetailuuid != eachdetail.podetailuuid))
                    {
                        eachdetail.isdeleted = true;
                        dto.Details.Add(eachdetail);
                    }
                }
                result = _repo.SaveDirectPurchaseOrder(dto);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
        }
        public PurchaseOrderModel GetPurchaseOrderByUUID(string pouuid)
        {
            try
            {
                var model = new PurchaseOrderModel();
                if (!string.IsNullOrEmpty(pouuid))
                {
                    model.ContainerPO.PO = _repo.GetPurchaseOrderByUUID(pouuid);
                }
                SessionManager.CurrentPurchaseOrderModel = model;
                SessionManager.CurrentPODetails = model.ContainerPO.PO.Details.Select(x => Cloner.DeepClone(x)).ToList();
                return model;
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new PurchaseOrderModel();
            }
        }
        public PurchaseOrderDetailDTO GetPOLineItem(string podetailuuid)
        {
            var model = SessionManager.CurrentPurchaseOrderModel;
            if (!string.IsNullOrEmpty(podetailuuid))
            {
                int index = model.ContainerPO.PO.Details.FindIndex(x => x.podetailuuid == podetailuuid);
                if (index >= 0)
                {
                    return model.ContainerPO.PO.Details[index];
                }
            }
            return new PurchaseOrderDetailDTO();
        }
        public PurchaseOrderModel SavePOLineItem(PurchaseOrderDetailDTO lineItem)
        {
            var model = SessionManager.CurrentPurchaseOrderModel;
            int index = model.ContainerPO.PO.Details.FindIndex(y => y.podetailuuid == lineItem.podetailuuid);
            if (index < 0)
            {
                if (string.IsNullOrEmpty(lineItem.podetailuuid))
                {
                    lineItem.podetailuuid = Guid.NewGuid().ToString();
                }
                lineItem.Type = string.IsNullOrEmpty(lineItem.jobtypedetailuuid) ? 2 : 1;
                model.ContainerPO.PO.Details.Add(lineItem);
            }
            else
            {
                var existingItem = model.ContainerPO.PO.Details[index];
                lineItem.podetailid = existingItem.podetailid;
                model.ContainerPO.PO.Details[index] = lineItem;
            }
            SessionManager.CurrentPurchaseOrderModel = model;
            return model;
        }
        public PurchaseOrderModel DeletePOLineItem(string podetailuuid)
        {
            var model = SessionManager.CurrentPurchaseOrderModel;
            model.ContainerPO.PO.Details.RemoveAll(x => x.podetailuuid == podetailuuid);
            SessionManager.CurrentPurchaseOrderModel = model;
            return model;
        }
        private void ProcessFilters(PurchaseOrderFilterPage filter)
        {
            filter.filters.appid = Common.MyAppID;
            filter.filters.createdby = new List<int>();
            if (filter.filters.createdby_encry.Count > 0)
            {
                foreach (var enc in filter.filters.createdby_encry)
                {
                    if (!string.IsNullOrEmpty(enc))
                    {
                        try
                        {
                            int id = Common.ToInt(Common.Decrypt(enc).ToString());
                            if (id > 0)
                            {
                                filter.filters.createdby.Add(id);
                            }
                            else
                            {
                                filter.filters.createdby.Add(Common.ToInt(enc));
                            }
                        }
                        catch
                        {
                            filter.filters.createdby.Add(Common.ToInt(enc));
                        }
                    }
                }
            }

            if (filter.filters.status > 0)
            {
                filter.filters.status_list = new List<int> { filter.filters.status };
            }
        }
        public void PopulateStatusCounts(PurchaseOrderFilterPage filter)
        {
            try
            {
                ProcessFilters(filter);
                PurchaseOrderFilterPage countFilter = Cloner.DeepClone(filter);
                countFilter.filters.status = 0;
                countFilter.filters.status_list = new List<int>();
                filter.StatusCount = _repo.GetPurchaseOrderStatusCount(countFilter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
        }
        public List<PurchaseOrderListDTO> GetPurchaseOrderList(PurchaseOrderFilterPage filter)
        {
            try
            {
                ProcessFilters(filter);
                return _repo.GetPurchaseOrderList(filter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<PurchaseOrderListDTO>();
            }
        }
    }
}