using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class QuotationWrapper
    {
        public ContainerBooking Booking { get; set; } = new ContainerBooking();
        public BookingQuotation Quotation { get; set; } = new BookingQuotation();
    }
}