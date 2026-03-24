using System.Collections.Generic;

namespace Contrack
{
    public interface IPurchaseOrderRepository
    {
        Result SaveDirectPurchaseOrder(PurchaseOrderDTO po);
        PurchaseOrderDTO GetPurchaseOrderByID(string poid);
        PurchaseOrderDTO GetPurchaseOrderByUUID(string pouuid);
        List<PurchaseOrderListDTO> GetPurchaseOrderList(PurchaseOrderFilterPage filter);
        List<PurchaseOrderStatusCountDTO> GetPurchaseOrderStatusCount(PurchaseOrderFilterPage filter);
    }
}