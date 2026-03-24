using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class Cabotage
    {
        public ContainerBookingDTO booking { get; set; } = new ContainerBookingDTO();
        public List<CabotageDTO> cabatages { get; set; } = new List<CabotageDTO>();
        public VoyageDTO voyage { get; set; } = new VoyageDTO();
    }
}