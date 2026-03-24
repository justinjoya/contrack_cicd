using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class PricingListFilter
    {
        public int startindex { get; set; } = 0;
        public int noofrows { get; set; } = 20;
        public string sortby { get; set; } = "createdat";
        public string sortdirection { get; set; } = "DESC";
        public string searchstr { get; set; } = "";
        public PricingFilter filters { get; set; } = new PricingFilter();
        public int GetFilterCount()
        {
            int count = 0;
            if (!string.IsNullOrEmpty(filters.pol_encry)) count++;
            if (!string.IsNullOrEmpty(filters.pod_encry)) count++;
            if (!string.IsNullOrEmpty(filters.transfertype_encry)) count++;
            if (!string.IsNullOrEmpty(filters.client_encry)) count++; 
            return count;
        }
    }
    public class PricingFilter
    {
        public string pol_encry { get; set; }
        public string pod_encry { get; set; }
        public string transfertype_encry { get; set; }
        public string client_encry { get; set; }
        public List<int> pols { get; set; }
        public List<int> pods { get; set; }
        public List<int> transfertypes { get; set; }
        public List<int> clients { get; set; }
    }
}