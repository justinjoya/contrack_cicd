using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class TermsandConditions
    {
        public MasterMenus menu { get; set; } = new MasterMenus();
        public TermsandConditionsDTO termsandConditions { get; set; } = new TermsandConditionsDTO();
        public Result result { get; set; }

    }
}