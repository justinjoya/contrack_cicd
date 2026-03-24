using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Contrack
{

    public class VoyageFilter
    {
        public int startindex { get; set; }
        public int noofrows { get; set; }
        public string sortby { get; set; } = "";
        public string sortdirection { get; set; } = "";
        public string searchstr { get; set; } = "";
        public InnerFilters filters { get; set; } = new InnerFilters();
        public List<voyageStatusCountDTO> StatusCount { get; set; } = new List<voyageStatusCountDTO>();

        public int GetFilterCount()
        {
            int count = 0;
            if (!string.IsNullOrEmpty(filters.startdate) &&
                !string.IsNullOrEmpty(filters.enddate))
            {
                count++;
            }
            if (!string.IsNullOrEmpty(filters.vesselassignmentencrypted)) count++;
            if (!string.IsNullOrEmpty(filters.origin_encry)) count++;
            if (!string.IsNullOrEmpty(filters.dest_encry)) count++;
            if (!string.IsNullOrWhiteSpace(filters.voyagenumber))
            {
                count++;
            }
            if (filters.islive)
            {
                count++;
            }
            if (filters.status > 0) count++;

            return count;
        }
    }

    public class InnerFilters
    {
        public int originid { get; set; } = 0;
        public int destinationid { get; set; } = 0;
        public string origin_encry { get; set; }
        public string dest_encry { get; set; }
        public string vesselassignmentencrypted { get; set; }
        public int vesselassignmentid { get; set; } = 0;
        public string startdate { get; set; } = "";
        public string voyagenumber { get; set; } = "";
        public string enddate { get; set; } = "";
        public bool islive { get; set; }
        public int status { get; set; }
        public List<int> status_list { get; set; } = new List<int>();

    }

    public class voyageStatusCountDTO
    {
        public int status_code { get; set; }
        public string status_name { get; set; }
        public long status_count { get; set; }
    }
}