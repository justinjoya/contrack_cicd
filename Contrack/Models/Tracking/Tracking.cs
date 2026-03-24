using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class ContainerSide
    {
        public int SideNo { get; set; } = 0;
        public string SideName { get; set; } = "";
    }
    public class Tracking
    {
        public TrackingDTO Trackingmodel { get; set; } = new TrackingDTO();
        public List<TrackingSelectionDTO> ContainerBookingSelection { get; set; } = new List<TrackingSelectionDTO>();
        public List<Moves> Moves { get; set; } = new List<Moves>();
        public List<ContainerAvailableDTO> AvailList = new List<ContainerAvailableDTO>();
        public Result Result { get; set; }
    }
    public class TrackingDamage
    {
        public ContainerSide Side { get; set; } = new ContainerSide();
        public TrackingDamageDTO Damage { get; set; } = new TrackingDamageDTO();
    }
}