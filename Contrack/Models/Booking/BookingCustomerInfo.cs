using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class ContainerBooking
    {
        public ContainerBookingDTO booking { get; set; } = new ContainerBookingDTO();
        public VoyageDTO voyage { get; set; } = new VoyageDTO();
        public BookingSummaryDTO bookingSummary { get; set; } = new BookingSummaryDTO();
        public Result result { get; set; } = new Result();
    }

}