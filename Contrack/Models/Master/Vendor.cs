using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class Vendor
    {
        public VendorDTO vendor { get; set; } = new VendorDTO();
        public MasterMenus menus { get; set; } = new MasterMenus();
        public Result result { get; set; }
    }
    public class VendorLog
    {
        public VendorDTO Info { get; set; } = new VendorDTO();
        public List<VendorDTO> Logs { get; set; } = new List<VendorDTO>();
    }
}