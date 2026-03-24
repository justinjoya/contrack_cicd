using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class LocationListViewModel
    {
        public List<Location> Locations { get; set; } = new List<Location>();
        public LocationListFilter Filter { get; set; } = new LocationListFilter();
        public int GetFilterCount()
        {
            int count = 0;
            if (!string.IsNullOrWhiteSpace(Filter.PortID) && Filter.PortID != "0")
                count++;
            if (!string.IsNullOrWhiteSpace(Filter.LocationTypeID) && Filter.LocationTypeID != "0")
            {
                var types = Filter.LocationTypeID.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (types.Any(t => t != "0" && !string.IsNullOrWhiteSpace(t)))
                {
                    count++;
                }
            }
            return count;
        }
    }
}