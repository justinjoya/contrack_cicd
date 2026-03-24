using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class Vessel
    {
        public VesselDTO vessel { get; set; } = new VesselDTO();
        public MasterMenus menus { get; set; } = new MasterMenus();
        public Result result { get; set; }
    }
}