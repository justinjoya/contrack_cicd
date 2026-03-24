using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class ContainerAllocation
    {
        public List<ContainerAllocationDTO> Allocations { get; set; } = new List<ContainerAllocationDTO>();
        public ContainerBookingDetailDTO Container { get; set; } = new ContainerBookingDetailDTO();
        public ContainerBookingDTO Booking { get; set; } = new ContainerBookingDTO();
    }
}