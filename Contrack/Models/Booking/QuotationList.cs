using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class QuoteMenus
    {
        public bool edit { get; set; } = true;
        public bool delete { get; set; } = true;
        public bool approve { get; set; } = true;
    }
    public class Quotationlist
    {
        public QuoteMenus menu { get; set; } = new QuoteMenus();
        public QuotationList QuotationLists { get; set; } = new QuotationList();
        public Result result { get; set; }
    }
}