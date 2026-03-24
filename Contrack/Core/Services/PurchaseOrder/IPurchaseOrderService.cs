using System.Collections.Generic;

namespace Contrack
{
    public interface IPurchaseOrderService
    {
        void SaveDirectPurchaseOrder(PurchaseOrderModel model);
        PurchaseOrderModel GetPurchaseOrderByUUID(string pouuid);
        List<PurchaseOrderListDTO> GetPurchaseOrderList(PurchaseOrderFilterPage filter);
        void PopulateStatusCounts(PurchaseOrderFilterPage filter);
    }
}