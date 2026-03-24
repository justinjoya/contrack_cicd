using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class BookingQuotation
    {
        public QuotationHeaderDTO quotation { get; set; } = new QuotationHeaderDTO();
        public ContainerBookingDTO booking { get; set; } = new ContainerBookingDTO();
        public VoyageDTO voyage { get; set; } = new VoyageDTO();
        public List<ChatDTO> comments { get; set; } = new List<ChatDTO>();
        public Result result { get; set; } = new Result();
    }
    public class BookingQuotationList
    {
        public string bookinguuid { get; set; }
        public List<QuotationList> quotation { get; set; } = new List<QuotationList>();
        public Result result { get; set; } = new Result();
    }
}