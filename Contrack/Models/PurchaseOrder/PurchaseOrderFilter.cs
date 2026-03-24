using System.Collections.Generic;
using Newtonsoft.Json;

namespace Contrack
{
    public class PurchaseOrderFilterPage
    {
        public int startindex { get; set; } = 0;
        public int noofrows { get; set; } = 20;
        public string sortby { get; set; } = "issuedate";
        public string sortdirection { get; set; } = "DESC";
        public string searchstr { get; set; } = "";
        public PurchaseOrderFilter filters { get; set; } = new PurchaseOrderFilter();
        public List<PurchaseOrderStatusCountDTO> StatusCount { get; set; } = new List<PurchaseOrderStatusCountDTO>();
        public int GetFilterCount()
        {
            int count = 0;
            if (!string.IsNullOrEmpty(filters.startdate)) count++;
            if (!string.IsNullOrEmpty(filters.enddate)) count++;
            if (filters.agencyuuids.Count > 0) count++;
            if (filters.vendoruuids.Count > 0) count++;
            if (filters.status_list.Count > 0) count++;
            if (filters.createdby_encry.Count > 0) count++;
            if (!string.IsNullOrEmpty(filters.jobid)) count++;
            return count;
        }
    }
    public class PurchaseOrderFilter
    {
        public string startdate { get; set; } = "";
        public string enddate { get; set; } = "";
        public string jobid { get; set; } = "";
        public List<string> agencyuuids { get; set; } = new List<string>();
        public List<string> vendoruuids { get; set; } = new List<string>();
        public List<string> createdby_encry { get; set; } = new List<string>();
        public List<long> agencydetailids { get; set; } = new List<long>();
        public List<long> vendordetailids { get; set; } = new List<long>();
        public List<int> createdby { get; set; } = new List<int>();
        public int appid { get; set; }
        [JsonProperty("status_selected")]
        public int status { get; set; }
        [JsonProperty("statuslist")]
        public List<int> status_list { get; set; } = new List<int>();
    }
    public class PurchaseOrderStatusCountDTO
    {
        public int status_code { get; set; }
        public string status_name { get; set; }
        public long status_count { get; set; }
    }
}