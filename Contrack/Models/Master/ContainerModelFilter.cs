using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class ContainerModelFilter
    {
        public int startindex { get; set; }
        public int noofrows { get; set; }
        public string sortby { get; set; } = "createdat";
        public string sortdirection { get; set; } = "DESC";
        public string searchstr { get; set; } = "";
        public ContainerModelFilters filters { get; set; } = new ContainerModelFilters();
        public int GetFilterCount()
        {
            int count = 0;
            if (filters.containertypeid > 0)
            {
                count++;
            }
            if (filters.sizeid > 0)
            {
                count++;
            }
            return count;
        }
    }

    public class ContainerModelFilters
    {
        public int containertypeid { get; set; } = 0;
        public string containertypeenc { get; set; } = "";
        public int sizeid { get; set; } = 0;
        public string sizeenc { get; set; } = "";
    }
}