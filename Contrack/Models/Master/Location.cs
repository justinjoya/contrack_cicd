using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class Location
    {
        public MasterMenus menu { get; set; } = new MasterMenus();
        public LocationDTO location { get; set; } = new LocationDTO();
        public Result result { get; set; }
    }
}