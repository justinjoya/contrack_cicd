using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class QuotationPricing
    {
        public PricingHeaderDTO pricing { get; set; } = new PricingHeaderDTO();
        public PricingTypeDTO transfertype { get; set; } = new PricingTypeDTO();
        public List<PricingCustomerDTO> customers { get; set; } = new List<PricingCustomerDTO>();
    }
}