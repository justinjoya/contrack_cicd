using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class VendorFilter
    {
        public string AgencyID { get; set; } = "";
        public string Search { get; set; } = "";
        public string sorting { get; set; } = "createdat";
        public string sortingorder { get; set; } = "desc";
        public int limit { get; set; } = 10;
        public int offset { get; set; } = 0;
    }
}