using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class PortDTO
    {
        public EncryptedData PortID { get; set; } = new EncryptedData();
        public string PortName { get; set; } = "";
        public string PortCode { get; set; } = "";
        public string CountryCode { get; set; } = "";
        public string CountryName { get; set; } = "";
        public string Flag { get; set; } = "";
    }
}