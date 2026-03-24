using System;
using System.Collections.Generic;

namespace Contrack
{
    public class VendorInvoiceDTO
    {
        public PurchaseIntentDTO PI { get; set; } = new PurchaseIntentDTO();
        public EncryptedData AgencyDetailID { get; set; } = new EncryptedData();
        public EncryptedData VendorDetailID { get; set; } = new EncryptedData();
        public List<string> VesselAssignmentID { get; set; } = new List<string>();
        public List<System.Web.UI.WebControls.ListItem> VesselList { get; set; } = new List<System.Web.UI.WebControls.ListItem>();

        public string InvoiceDate { get; set; } = "";
        public string DueDate { get; set; } = "";
        public EncryptedData InvoiceID { get; set; } = new EncryptedData();
        public string InvoiceUUID { get; set; } = "";
        public string InvoiceNo { get; set; } = "";
        public string Currency { get; set; } = "USD";
        public decimal InvoiceAmount { get; set; } = 0;
        public decimal PaidAmount { get; set; } = 0;
        public string StatusName { get; set; } = "";
        public string JobID { get; set; } = "";
        public string PurchaseIntentIDInc { get; set; } = "";
        public int Status { get; set; } = 1;

        public string VendorName { get; set; } = "";
        public string AgencyName { get; set; } = "";
        public string POUUID { get; set; } = "";
        public string POCode { get; set; } = "";
        public string Remarks { get; set; } = "";
        public string comments { get; set; } = "";

        public string HODApprover { get; set; } = "";
        public string FinanceApprover { get; set; } = "";
        public string POApprover { get; set; } = "";
        public string CreatedUser { get; set; } = "";
        public int Approvedby1 { get; set; } = 0;

        //public List<Chat> CommentsList { get; set; } = new List<Chat>();
        public DocumentDTO InvoiceDocument { get; set; } = new DocumentDTO();
        public DateTime CreatedAt { get; set; } = new DateTime();
        public List<VendorInvoiceDetail> Details { get; set; } = new List<VendorInvoiceDetail>();
        public List<VIPaymentSummary> PaymentSummary { get; set; } = new List<VIPaymentSummary>();
        public List<DocumentDTO> Documents { get; set; } = new List<DocumentDTO>();
        public Result result { get; set; } = new Result();
        //public Rating Ratings { get; set; } = new Rating();
    }

    public class VendorInvoiceDetail
    {
        public int InvoiceDetailID { get; set; } = 0;
        public string InvoiceDetailUUID { get; set; } = Guid.NewGuid().ToString();
        public int InvoiceID { get; set; } = 0;
        public string ServiceName { get; set; } = "";
        public string Description { get; set; } = "";
        public string ArticleIMPACode { get; set; } = "";
        public string PartNo { get; set; } = "";
        public string Make { get; set; } = "";
        public string Model { get; set; } = "";
        public string UOM { get; set; } = "";
        public int JobTypeID { get; set; } = 0;
        public int Order { get; set; } = 0;
        public decimal POQty { get; set; } = 0;
        public decimal POPrice { get; set; } = 0;
        public decimal InvoiceQty { get; set; } = 0;
        public decimal InvoicePrice { get; set; } = 0;
        public int PODetailID { get; set; } = 0;
        public int POID { get; set; } = 0;
        public bool IsDeleted { get; set; } = false;
        public string JobTypeDetailUUID { get; set; } = "";
        public int Type { get; set; } = 1;
        public string Remarks { get; set; } = "";
    }

    public class vendor_invoice_header
    {
        public int vendorinvoiceid { get; set; } = 0;
        public string vendorinvoiceuuid { get; set; } = "";
        public string vendorinvoiceno { get; set; } = "";
        public int agencydetailid { get; set; } = 0;
        public int vendordetailid { get; set; } = 0;
        public int piid { get; set; } = 0;
        public string invoicedate { get; set; } = "";
        public string duedate { get; set; } = "";
        public string jobid { get; set; } = "";
        public string currency { get; set; } = "";
        public decimal invoiceamount { get; set; } = 0;
        public List<int> vesseldetailid { get; set; } = new List<int>();
        public string pouuid { get; set; } = "";
        public string documentuuid { get; set; } = "";
        public string remarks { get; set; } = "";
        public bool savefew { get; set; } = false;
    }

    public class vendor_invoice_detail
    {
        public int vendorinvoicedetailid { get; set; } = 0;
        public string vendorinvoicedetailuuid { get; set; }
        public int vendorinvoiceid { get; set; } = 0;
        public string description { get; set; } = "";
        public string articlecode { get; set; } = "";
        public string make { get; set; } = "";
        public string model { get; set; } = "";
        public int jobtypeid { get; set; } = 0;
        public int sortorder { get; set; } = 0;
        public string operation { get; set; } = "";
        public string partno { get; set; } = "";
        public string uom { get; set; } = "";
        public decimal poquantity { get; set; } = 0;
        public decimal poprice { get; set; } = 0;
        public decimal invoicequantity { get; set; } = 0;
        public decimal invoiceprice { get; set; } = 0;
        public long podetailid { get; set; } = 0;
        public bool isdeleted { get; set; } = false;
        public string jobtypedetailuuid { get; set; } = "";
        public string itemremarks { get; set; } = "";
    }

    public class VIApprovaReject
    {
        public string invoiceuuid { get; set; } = "";
        public bool isaccept { get; set; } = false;
        public int invoicecount { get; set; } = 0;
        public string comments { get; set; } = "";
        public string invoiceid { get; set; } = "";
        public List<InvoiceCurrencies> invoices { get; set; } = new List<InvoiceCurrencies>();
    }

    public class InvoiceCurrencies
    {
        public string currency { get; set; } = "";
        public int invoicecount { get; set; } = 0;
        public decimal amount { get; set; } = 0;
    }
    public class VendorInvoiceListPage
    {
        public PurchaseIntentDTO PI { get; set; } = new PurchaseIntentDTO();
        public List<PIVendorInvoice> VIList { get; set; } = new List<PIVendorInvoice>();
    }
    public class PIVendorInvoice
    {
        public int VendorInvoiceID { get; set; }
        public string VendorInvoiceUUID { get; set; }
        public string VendorInvoiceNo { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; }
        public string VendorName { get; set; }
        public string StatusName { get; set; }
    }

    public class VIPaymentSummary
    {
        public string BatchNo { get; set; } = "";
        public int BatchID { get; set; } = 0;
        public string BatchUUID { get; set; } = "";
        public DateTime BatchCreatedAt { get; set; } = new DateTime();
        public DateTime CreatedAt { get; set; } = new DateTime();
        public decimal PaidAmount { get; set; } = 0;
        public decimal BalanceAmount { get; set; } = 0;
        public decimal ConversionRate { get; set; } = 0;
        public string BatchCurrency { get; set; } = "";
        public string Status { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
    }
}