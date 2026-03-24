using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class VoyageExtention
    {
        public VoyageExtendedDTO VoyageExtendedDTO { get; set; } = new VoyageExtendedDTO();
        public Result result { get; set; }

    }
    public class Voyage
    {
        public VoyageDTO VoyageDTO { get; set; } = new VoyageDTO();
        public Result result { get; set; }
    }
}