using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class ContainerModel
    {
        public ContainerModelDTO containermodel { get; set; } = new ContainerModelDTO();
        public Result result { get; set; }
    }
    public class ContainerModelExtended
    {
        public MasterMenus menu { get; set; } = new MasterMenus();
        public ContainerModelExtendedDTO model { get; set; } = new ContainerModelExtendedDTO();
        public Result result { get; set; }
    }
}