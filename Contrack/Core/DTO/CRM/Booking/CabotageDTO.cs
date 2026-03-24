using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class CabotageDTO
    {
        public string cabotageuuid { get; set; } = "";
        public FormattedValue<DateTime> createdat { get; set; } = new FormattedValue<DateTime>();
        public string fullname { get; set; } = "";
        public int containercount { get; set; } = 0;
    }
}