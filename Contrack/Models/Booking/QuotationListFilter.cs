using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contrack
{
    public class QuotationListFilter
    {
        public string sortby { get; set; } = "createdat";
        public string sortdirection { get; set; } = "DESC";
        public int startindex { get; set; } = 0;
        public int noofrows { get; set; } = 10;
        public string searchstr { get; set; } = "";
        public QuotationFilter filters { get; set; } = new QuotationFilter();
        // public int ActiveTab { get; set; } = 0;
        public List<QuotationStatusCountDTO> StatusCount { get; set; } = new List<QuotationStatusCountDTO>();
        public int GetFilterCount()
        {
            int count = 0;
            if (!string.IsNullOrWhiteSpace(filters.bookingno))
            {
                count++;
            }
            if (filters.agencyuuids.Count > 0)
                count++;
            if (filters.createdby.Count > 0)
                count++;
            if (filters.status > 0) count++;
            if (Common.Decrypt(filters.hodapprover_enc) > 0) count++;

            return count;
        }
    }
    public class QuotationFilter
    {
        public string startdate { get; set; } = "";
        public string enddate { get; set; } = "";
        public List<string> agencydetailid { get; set; } = new List<string>();
        public string bookingno { get; set; } = "";
        public List<string> agencyuuids { get; set; } = new List<string>();
        // public List<int> status { get; set; } = new List<int>();
        public List<long> createdby { get; set; } = new List<long>();
        public int status { get; set; }
        public List<int> status_list { get; set; } = new List<int>();
        public int hodapprover { get; set; } = 0;
        public string hodapprover_enc { get; set; } = "";

    }
    public class QuotationStatusCountDTO
    {
        public int status_code { get; set; }
        public string status_name { get; set; }
        public long status_count { get; set; }
    }

}