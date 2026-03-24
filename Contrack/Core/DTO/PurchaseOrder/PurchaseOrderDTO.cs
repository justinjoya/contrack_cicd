using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Contrack
{
    public class ContainerPODTO
    {
        public PurchaseOrderDTO PO { get; set; } = new PurchaseOrderDTO();
        public PurchaseOrderExtendedDTO POExtended { get; set; } = new PurchaseOrderExtendedDTO();
    }
    public class PurchaseOrderExtendedDTO
    {
        public string bookingno { get; set; } = "";
    }
    public class PurchaseOrderDTO
    {
        public EncryptedData dpoid { get; set; } = new EncryptedData();
        public string dpouuid { get; set; } = "";
        public EncryptedData poid { get; set; } = new EncryptedData();
        public string pouuid { get; set; } = "";
        public string pocode { get; set; } = "";
        public EncryptedData agencydetailid { get; set; } = new EncryptedData();
        public string agencyname { get; set; } = "";
        public EncryptedData vendordetailid { get; set; } = new EncryptedData();
        public string vendor_name { get; set; } = "";
        public string vendorcurrency { get; set; } = "USD";
        [AllowHtml] public string comments { get; set; } = "";
        [AllowHtml] public string terms { get; set; } = "";
        [AllowHtml] public string invoiceinstruction { get; set; } = "";
        public int appid { get; set; } = 0;
        public string status_desc { get; set; } = "";
        public string createdby_username { get; set; } = "";
        public string vendoraddress { get; set; } = "";
        public string billto { get; set; } = "";
        public string deliveryaddress { get; set; } = "";
        public bool isdirectpo { get; set; } = true;
        public FormattedValue<DateTime> createdat { get; set; } = FormatConvertor.ToDateTimeFormat(DateTime.MinValue);
        public FormattedValue<DateTime> issuedate { get; set; } = FormatConvertor.ToDateTimeFormat(DateTime.MinValue);
        public Result result { get; set; } = new Result();
        public List<PurchaseOrderDetailDTO> Details { get; set; } = new List<PurchaseOrderDetailDTO>();
    }
    public class PurchaseOrderDetailDTO
    {
        public EncryptedData podetailid { get; set; } = new EncryptedData();
        public string podetailuuid { get; set; } = "";
        public decimal quantity { get; set; } = 0;
        public decimal tax { get; set; } = 0;
        public int sortorder { get; set; } = 0;
        public decimal vendorprice { get; set; } = 0;
        public string itemname { get; set; } = "";
        public string article_impacode { get; set; } = "";
        public string partno { get; set; } = "";
        public string make { get; set; } = "";
        public string model { get; set; } = "";
        public string uom { get; set; } = "";
        public string remarks { get; set; } = "";
        public int jobtype { get; set; } = 0;
        public string jobtypedetailuuid { get; set; } = "";
        public int Type { get; set; } = 1;
        public bool isdeleted { get; set; } = false;
    }
}