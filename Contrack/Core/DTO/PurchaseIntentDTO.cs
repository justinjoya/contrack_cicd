using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Contrack
{
    public class PurchaseIntentDTO
    {
        public string PIDateTime { get; set; } = Common.GetCurrentDateTime().ToString("yyyy-MM-dd HH:mm");
        public int PurchaseIntentID { get; set; } = 0;
        public string PurchaseIntentIDInc { get; set; } = "";
        public string PurchaseIntentUUIDID { get; set; } = "";
        public string PurchaseIntentCode { get; set; } = "";
        public EncryptedData AgencyDetailID { get; set; } = new EncryptedData();
        public EncryptedData Port { get; set; } = new EncryptedData();
        public EncryptedData ClientDetailID { get; set; } = new EncryptedData();
        public List<string> VesselAssignmentID { get; set; } = new List<string>();
        public List<ListItem> VesselList { get; set; } = new List<ListItem>();
        public string DepartureDate { get; set; } = "";
        public string ArrivalDate { get; set; } = "";
        public string DeliveryDate { get; set; } = "";
        public string Currency { get; set; } = "USD";
        public string Priority { get; set; } = "";
        public string Salutation { get; set; } = "";
        public string Requester { get; set; } = "";
        public string BillTo { get; set; } = "";
        public string Remarks { get; set; } = "";
        public string AgencyName { get; set; } = "";
        public string ClientName { get; set; } = "";
        public string VesselName { get; set; } = "";
        public string PortName { get; set; } = "";
        public string StatusName { get; set; } = "";
        public string CreatedUser { get; set; } = "";
        public string JobID { get; set; } = "";
        public int Status { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = new DateTime();
        public List<int> JobTypes { get; set; } = new List<int>();
        public List<DocumentDTO> Documents { get; set; } = new List<DocumentDTO>();
        public List<KeyValuePair> MiscData { get; set; } = new List<KeyValuePair>();
        public List<PurchaseIntentDetailJob> JobDetails { get; set; } = new List<PurchaseIntentDetailJob>();
        public List<PICount> Stats { get; set; } = new List<PICount>();

        public Result result = new Result();
    }

    public class PurchaseIntentDetailJob
    {
        public PIJobType JobType { get; set; } = new PIJobType();
        public List<PurchaseIntentDetail> Details { get; set; } = new List<PurchaseIntentDetail>();
    }
    public class PurchaseIntentDetail
    {
        public int JobTypeID { get; set; } = 0;
        public int JobTypeOrder { get; set; } = 0;
        public int PIDetailID { get; set; } = 0;
        public string PIDetailIDEnc { get; set; } = "";
        public string PIDetailUUID { get; set; } = Guid.NewGuid().ToString();
        public string ServiceName { get; set; } = "";
        public string Description { get; set; } = "";
        public string ArticleIMPACode { get; set; } = "";
        public string PartNo { get; set; } = "";
        public string Make { get; set; } = "";
        public string Model { get; set; } = "";
        public string Remarks { get; set; } = "";
        public string UOM { get; set; } = "";
        public decimal Qty { get; set; } = 0;
        public bool Mandatory { get; set; } = false;
        public int Order { get; set; } = 0;
        public int TempOrder { get; set; } = 0;
        public bool IsEdited { get; set; } = false;
        public bool IsDelete { get; set; } = false;
        public string JobTypeDetailUUID { get; set; } = "";
    }
    public class PIJobType
    {
        public int JobTypeID { get; set; } = 0;
        public string JobName { get; set; } = "";
        public string JobCode { get; set; } = "";
        public bool UseMaster { get; set; } = false;
        public int Order { get; set; } = 0;
    }
    public class PurchaseIntentDetailSingle
    {
        public PurchaseIntentDetail Detail { get; set; } = new PurchaseIntentDetail();
        public PIJobType JobType { get; set; } = new PIJobType();
        public bool ServiceChecked { get; set; } = false;
        public bool MandatoryChecked { get; set; } = false;
        public bool QuantityChecked { get; set; } = false;
        public bool UOMChecked { get; set; } = false;
    }
    public class PIBulkDelete
    {
        public int JobTypeID { get; set; } = 0;
        public string PIDetailUUID { get; set; } = "";
    }
    public class PIBulkUpdate
    {
        public PurchaseIntentDetailSingle Bulk { get; set; } = new PurchaseIntentDetailSingle();
        public List<PIBulkDelete> List { get; set; } = new List<PIBulkDelete>();
    }
    public class RFQDetailModal
    {
        public int purchaseintentdetailid { get; set; } = 0;
        public int sortorder { get; set; } = 0;
    }
    public class RFQModal
    {
        public string PurchaseIntentID { get; set; } = "";
        public string PurchaseIntentUUID { get; set; } = "";
        public List<string> SupplierList { get; set; } = new List<string>();
        public string TermsAndConditions { get; set; } = "";
        public bool ShowRFQList { get; set; } = true;
        public List<string> SelectedIDs { get; set; } = new List<string>();
        public Result result = new Result();
    }
    public class PICount
    {
        public string document_type { get; set; } = "";
        public int count { get; set; } = 0;
    }


    public class StatusCount
    {
        public int StatusID { get; set; } = 0;
        public int Count { get; set; } = 0;
        public int UserCount { get; set; } = 0;
    }
}