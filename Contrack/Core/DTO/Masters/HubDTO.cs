using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class HubDTO
    {
        public int hubid { get; set; } = 0;
        public string uuid { get; set; } = "";
        public DateTime hcreatedt { get; set; } = new DateTime();
        public string hubname { get; set; } = "";
        public string imono { get; set; } = "";
        public string address { get; set; } = "";
        public string email { get; set; } = "";
        public string phone { get; set; } = "";
    }
}