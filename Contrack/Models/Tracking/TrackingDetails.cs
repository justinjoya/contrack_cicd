using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class TrackingDetails
    {
        public List<TrackingDetailsDTO> Trackingdetails { get; set; } = new List<TrackingDetailsDTO>();
        public ContainerBooking booking { get; set; } = new ContainerBooking();
    }
}