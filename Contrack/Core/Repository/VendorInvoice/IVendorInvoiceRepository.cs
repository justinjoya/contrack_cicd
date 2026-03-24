using System.Collections.Generic;
using System.Data;

namespace Contrack
{
    public interface IVendorInvoiceRepository
    {
        Result SaveVendorInvoice(string headerJson, string detailsJson);
        Result DeleteVendorInvoice(string invoiceUuid);
        Result QuoteApproval(string invoiceUuid, int approvalStatus, string approver);
        Result QuoteApprovalBulk(List<string> invoiceUuids);
        Result BulkApproveReject(string invoiceUuid, bool isAccept, string comments);
        Result SendBulkApproval(string invoiceUuid, string approver, string message);
        Result RecordPayment(string invoiceUuid, decimal paidAmount);
        Result CancelVendorInvoice(string invoiceUuid, int invoiceId);
        void UpdateDocumentUUID(string invoiceUuid, string documentUuid);
        VendorInvoiceDTO GetVendorInvoiceByUUID(string uuid);
        List<VIPaymentSummary> GetPaymentSummary(string invoiceUuid);
        List<string> GetAutoApproveInvoices(List<string> invoiceUuids);
        List<InvoiceCurrencies> GetCurrencies(string invoiceUuid);
    }
}